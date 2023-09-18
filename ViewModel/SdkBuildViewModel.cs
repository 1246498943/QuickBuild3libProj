using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Dialogs;
using XPloteQuickBuidProj.Views;
using System.Windows.Forms;
using System.IO;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors.Core;
using System.Collections.ObjectModel;

namespace XPloteQuickBuidProj
{

    /// <summary>
    /// 提供和前面数据的交互功能.
    /// </summary>
    public class SdkBuildViewModel : ObservableObject,IDisposable
    {
        public SdkBuildViewModel()
        {
            gBuildModel = new sdkBuildModel();
            gCreateDirModel = new sdkCreateDirModel();//创建文件夹.
            InitCommands();
            GlobalSingleHelper.SendCheckHandler+=GlobalSingleHelper_SendCheckHandler;
        }


        private void UpdateSdkDllLists()
        {
            if (gBuildModel != null)
            {
                gBuildModel.ClearnSkModelItems();
                var compelierLists = gBuildModel.gCompelierSource;
                foreach (var compeLierItem in compelierLists) 
                {
                    foreach (var sdkItem in compeLierItem.gSdkItemList)
                    {
                        if(sdkItem.gIsChecked==true)
                        {
                            gBuildModel.AddSdkModelItem(sdkItem);
                        }
                    }
                }
            }
        }

        private void GlobalSingleHelper_SendCheckHandler(bool? obj)
        {
            //一些关键信号触发的时候,在这里检测
            UpdateSdkDllLists();
        }

        public ICommand? gSetSdkDir { get; set; }      //设置目录.
        public ICommand? gImportSdk2List { get; set; } //导入库
        public ICommand? gCreateStructDirs { get; set; }//在当前的Dir下,创建新的结构目录.

        public ICommand? gImportSettingConfig { get; set; }//导入配置文件.
        public ICommand? gCheckIsSucessSetConfig { get; set; }//校验配置文件,将配置文件中-sdk目录中的比较,不符合的,设置层红色.

        public ICommand? gSaveSettingConfig { get; set; }//保存并导出配置文件.

        public ICommand? gShowSettingManageWnd { get; set; }//打开配置文件管理器窗口,进行配置文件快速加载.


        public ICommand? IReUpdate { get; set; }//重新从文件夹中读取文件数据.并将数据加载将来.
        public ICommand? IReadFileHistory { get; set; }
     

        private sdkBuildModel mBuildModel;

        /// <summary>
        /// 数据模块.
        /// </summary>
        public sdkBuildModel gBuildModel
        {
            get => mBuildModel;
            set
            {
                if (mBuildModel != value)
                {
                    mBuildModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 历史配置.
        /// </summary>
        public ObservableCollection<SettingJsonModelItem> gJsonLists { get; set; } = new ObservableCollection<SettingJsonModelItem>();
        public SettingJsonModelItem gSelectedJsonModelItem { get; set; }

        private  SettingHistory? mSetHistoryWnd { get; set; }


        //读取c++文件,并解析重新写入
        public ICommand gOpenCcProjFile { get; set; }//读取vcProj配置文件.

        public ICommand gLookWriteContent { get; set; }
        public ICommand gBuildAndWriteConfig2VCProj { get; set; }//构建VcProj配置项目.

        private ShowContent? mShowContentWnd { get; set; }


        private sdkCreateDirModel mCreateDirModel;
        public sdkCreateDirModel gCreateDirModel
        {
            get => mCreateDirModel;
            set
            {
                if (mCreateDirModel != value)
                {
                    mCreateDirModel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private CreateDir? mCreateDirWnd { get; set; }

        private void InitCommands()
        {
            gSetSdkDir = new RelayCommand(() =>
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                folderDialog.Description = "选择文件夹";
                if (folderDialog.ShowDialog()== DialogResult.OK)
                {
                    gBuildModel.gSdkDir = gCreateDirModel.gSdkDir  =  folderDialog.SelectedPath;
                }
            });

            gImportSdk2List = new RelayCommand(() => { ImportDllContent2Lists(); });

            gCreateStructDirs = new RelayCommand(() => {
            
            
                if(mCreateDirWnd==null)
                {
                    mCreateDirWnd = new CreateDir();
                }
                mCreateDirWnd.ShowWnd();

            });

            gImportSettingConfig = new RelayCommand(() => {

                ReadConfigFileFromDialog();
            });

            gSaveSettingConfig = new RelayCommand(() => {

                //写入数据到文件中.
                var curSettingFilePath = $"{gBuildModel.gSettingPath}{gBuildModel.gSettingName}.xml";
                GlobalGetErrorHelper.GetError(() => {

                    var dataList = gBuildModel.gBuildSdkSource;
                    if (dataList != null && dataList.Count>0)
                    {
                        JsonSerialFileHelper.Write2File(dataList, curSettingFilePath);
                    }
                    else
                    {
                        throw new Exception("数据为空,导出失败");
                    }

                },$"导出文件 {curSettingFilePath}");
           
            });

            //////////////历史
            ///刷新文件夹.
            IReUpdate = new RelayCommand(() => {

                gJsonLists.Clear();
                var curJsonDir = XPloteQucikConfigs.CurSettingDir;
                var curJsonFiles = System.IO.Directory.GetFiles(curJsonDir, "", System.IO.SearchOption.AllDirectories);
                foreach (var file in curJsonFiles)
                {
                    SettingJsonModelItem jsonModel = new SettingJsonModelItem();
                    jsonModel.gFilePath = file;
                    jsonModel.gFileName = System.IO.Path.GetFileNameWithoutExtension(file);
                    var dataList = JsonSerialFileHelper.ReadDataFromFile<ObservableCollection<sdkModelItem>>(file);
                    jsonModel.gSdkModelLists = dataList;
                    gJsonLists.Add(jsonModel);
                }
            });


            IReadFileHistory = new RelayCommand(() => {

                ReadConfigFileFromHistory();
            });

            gShowSettingManageWnd = new RelayCommand(() => { 
            
                if(mSetHistoryWnd==null)
                {
                    mSetHistoryWnd = new SettingHistory();
                }
                mSetHistoryWnd.ShowWnd();


            });


            //c++构建

            //获取文件路径....
            gOpenCcProjFile = new RelayCommand(() => {

                GlobalGetErrorHelper.GetError(() => { 
                
                OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "*.vcxproj|*.vcxproj|*.*|*.*";
                    if(openFileDialog.ShowDialog()== DialogResult.OK)
                    {
                        gBuildModel.gVcxprojFile = openFileDialog.FileName;
                    }
                
                });
            
            });

            gLookWriteContent = new RelayCommand(() => {

                if(mShowContentWnd==null)
                {
                    mShowContentWnd = new ShowContent();
                }

                //设置内容.
                var curStr = GetWriteContentSt();
                mShowContentWnd.SetContentStr(curStr);
                mShowContentWnd.ShowWnd();

            });


            gBuildAndWriteConfig2VCProj = new RelayCommand(() => { 
            
            
            
            });

   

        }

        private string GetWriteContentSt()
        {
            //分层4组: 
            //opencv/
            //x64/Debug/include + lib + dll;
            //x64/Release/include + lib + dll;
            //x32/Debug/include + lib + dll;
            //x32/Release/include + lib + dll;
            //string libStr_Debug = "", includeStr_Debug = "", dllStr_Debug = ""
            //    ,;

            string[] StrDes = new string[12];

            foreach (var sdkItem in gBuildModel?.gBuildSdkSource)
            {
                //64/debug/
                StrDes[0]+=$"{sdkItem.DllName_64_Debug_Lib_String()};\r\n";
                StrDes[1]+=$"{sdkItem.DllName_64__Debug_Dll_String()};\r\n";
                StrDes[2]+=$"{sdkItem.DllName_64__Debug_Include_String()};\r\n";

                //64/Release/
                StrDes[3]+=$"{sdkItem.DllName_64_Release_Lib_String()};\r\n";
                StrDes[4]+=$"{sdkItem.DllName_64__Release_Dll_String()};\r\n";
                StrDes[5]+=$"{sdkItem.DllName_64__Release_Include_String()};\r\n";


                StrDes[6]+=$"{sdkItem.DllName_32_Debug_Lib_String()};\r\n";
                StrDes[7]+=$"{sdkItem.DllName_32__Debug_Dll_String()};\r\n";
                StrDes[8]+=$"{sdkItem.DllName_32__Debug_Include_String()};\r\n";

                StrDes[9]+=$"{sdkItem.DllName_32_Release_Lib_String()};\r\n";
                StrDes[10]+=$"{sdkItem.DllName_32__Release_Dll_String()};\r\n";
                StrDes[11]+=$"{sdkItem.DllName_32__Release_Include_String()};\r\n";

            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < StrDes.Length; i++)
            {
                if(i>0 && i%3==0)
                {
                    sb.AppendLine($"{"----------------------------------"}\r\n\r\n");
                }
                sb.AppendLine(StrDes[i]);
            }
            return sb.ToString();
        }

        private void ReadConfigFileFromHistory()
        {

            if(gSelectedJsonModelItem!=null)
            {
                var curFile = gSelectedJsonModelItem.gFilePath;
                ReadConfigFileFromFile(curFile);
                //更新配置文件名字.
                gBuildModel.gSettingName = System.IO.Path.GetFileNameWithoutExtension(curFile);
            }

        }

        private void ReadConfigFileFromFile(string curFile)
        {

            GlobalGetErrorHelper.GetError(() => {

                var datalist = JsonSerialFileHelper.ReadDataFromFile<ObservableCollection<sdkModelItem>>(curFile);
                gBuildModel.ClearnSkModelItems();
                foreach (var item in datalist)
                {
                    gBuildModel.AddSdkModelItem(item);
                   
                }
            });
            
        }

        private void ReadConfigFileFromDialog()
        {
            //打开文件夹.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*.xml|*.xml";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var curFile = openFileDialog.FileName;
                ReadConfigFileFromFile(curFile);
            }
        }


        private void ImportDllContent2Lists2()
        {

            //开始层层解析文件夹
            //第一层为编译器名字.
            //第二层为库名字
            //第三层为64 / 32 
            //第四层为 lib / include / dll
            //里面就是Debug / Release
            ///
        }

        private void ImportDllContent2Lists()
        {
            if (mBuildModel?.gSdkDir?.Length<1) return;

            //使用第二种解析方式.
            //第一层编译器版本
            //第二层为库名字
            //第三层为 64/32
            //第四层为 Debug / Release
            //第五层为dll / lib /  include

            ////实际上,从第三层之后,后面的解析结果基本就一样了.

            var curSdkDir = gBuildModel.gSdkDir;
            if (curSdkDir == null) return;

            var curDllSourceList = gBuildModel?.gCompelierSource;
            curDllSourceList?.Clear();

            ///从一个路径中,返回最后的文件夹名字.
            var GetDirNameFromPath = (string str) => {

                return System.IO.Path.GetFileNameWithoutExtension(str);
            };

            //先从这里获取第一层文件夹.区分遍历器库版本.
            var compilerDirs = Directory.GetDirectories(curSdkDir,"",SearchOption.TopDirectoryOnly);
            foreach ( var compilerDirItem in compilerDirs )
            {
                var comPelierName = GetDirNameFromPath(compilerDirItem);
                CompelierItem compelierModel = new CompelierItem(comPelierName);
                curDllSourceList.Add(compelierModel);

                //获取第二层库名字;
                var dllDirs = Directory.GetDirectories(compilerDirItem, "", SearchOption.TopDirectoryOnly);
                foreach (var dllDirItem in dllDirs)
                {
                    var dllName =GetDirNameFromPath(dllDirItem).Split('-').ToList().First();//使用"-"分割文字,获取第一个;;
                    sdkModelItem sdkModel = new sdkModelItem(dllName);
                    compelierModel.gSdkItemList.Add(sdkModel);
 
                    //这一层,获取到所有的库目录
                    var m64Or32Dir = Directory.GetDirectories(dllDirItem, "", SearchOption.TopDirectoryOnly);
                    foreach (var m63Or32Item in m64Or32Dir)
                    {
                        var m32or64Name = GetDirNameFromPath(m63Or32Item);
                        //32位
                        if (m63Or32Item.ToUpper().Contains("x32"))
                        {
                            //区分Debug/or Release;
                            var mDebugOrReleaseDir = Directory.GetDirectories(m63Or32Item, "", SearchOption.TopDirectoryOnly);
                            foreach (var mDebugOrReleaseDirItem in mDebugOrReleaseDir)
                            {

                                var mDebugOrRelease = GetDirNameFromPath(mDebugOrReleaseDirItem);//区分Debug / Release;
                                if (mDebugOrRelease.ToUpper().Contains("DEBUG"))
                                {
                                    //64 - Debug
                                    //64 - Release
                                    //获取到dll / lib /  include 
                                    var m_lib_inc_dlls = Directory.GetDirectories(mDebugOrReleaseDirItem, "", SearchOption.TopDirectoryOnly);
                                    foreach (var mLib_inc_dlls_item in m_lib_inc_dlls)
                                    {
                                        ///文件夹的路径已经获取完毕
                                        var curLibName = mDebugOrReleaseDirItem.ToUpper();
                                        switch (curLibName)
                                        {
                                            case "LIB":
                                                {
                                                    //获取当前所有的文件,而且是.lib的文件.
                                                    var curLibLists = Directory.GetDirectories(mLib_inc_dlls_item, "", SearchOption.AllDirectories);
                                                    var m64ReleaseLibDll = sdkModel?.gDll32?.gDebug.gLibDir.gLibSources;
                                                    foreach (var curLibItem in curLibLists)
                                                    {
                                                        if (curLibItem.ToUpper().Contains(".LIB"))
                                                        {
                                                            m64ReleaseLibDll.Add(GetDirNameFromPath(curLibItem));
                                                        }
                                                    }
                                                    break;
                                                }
                                            case "DEBUG":
                                                {
                                                    break;
                                                }
                                            case "INCLUDE":
                                                {
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                    }
                                }
                                else if (mDebugOrRelease.ToUpper().Contains("RELEASE"))
                                {
                                    //64 - Release
                                    //获取到dll / lib /  include 
                                    var m_lib_inc_dlls = Directory.GetDirectories(mDebugOrReleaseDirItem, "", SearchOption.TopDirectoryOnly);
                                    foreach (var mLib_inc_dlls_item in m_lib_inc_dlls)
                                    {
                                        ///文件夹的路径已经获取完毕
                                        var curLibName = mDebugOrReleaseDirItem.ToUpper();
                                        switch (curLibName)
                                        {
                                            case "LIB":
                                                {
                                                    //获取当前所有的文件,而且是.lib的文件.
                                                    var curLibLists = Directory.GetDirectories(mLib_inc_dlls_item, "", SearchOption.AllDirectories);
                                                    var m64ReleaseLibDll = sdkModel?.gDll32?.gRelease.gLibDir.gLibSources;
                                                    foreach (var curLibItem in curLibLists)
                                                    {
                                                        if (curLibItem.ToUpper().Contains(".LIB"))
                                                        {
                                                            m64ReleaseLibDll.Add(GetDirNameFromPath(curLibItem));
                                                        }
                                                    }
                                                    break;
                                                }
                                            case "DEBUG":
                                                {
                                                    break;
                                                }
                                            case "INCLUDE":
                                                {
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                    }

                                }

                            }

                        }
                        else if(m63Or32Item.ToUpper().Contains("x64"))//64位
                        {
                            //区分Debug/or Release;
                            var mDebugOrReleaseDir = Directory.GetDirectories(m63Or32Item, "", SearchOption.TopDirectoryOnly);
                            foreach (var mDebugOrReleaseDirItem in mDebugOrReleaseDir)
                            {

                                var mDebugOrRelease = GetDirNameFromPath(mDebugOrReleaseDirItem);//区分Debug / Release;
                                if(mDebugOrRelease.ToUpper().Contains("DEBUG"))
                                {
                                    //64 - Debug
                                    //64 - Release
                                    //获取到dll / lib /  include 
                                    var m_lib_inc_dlls = Directory.GetDirectories(mDebugOrReleaseDirItem, "", SearchOption.TopDirectoryOnly);
                                    foreach (var mLib_inc_dlls_item in m_lib_inc_dlls)
                                    {
                                        ///文件夹的路径已经获取完毕
                                        var curLibName = mDebugOrReleaseDirItem.ToUpper();
                                        switch (curLibName)
                                        {
                                            case "LIB":
                                                {
                                                    //获取当前所有的文件,而且是.lib的文件.
                                                    var curLibLists = Directory.GetDirectories(mLib_inc_dlls_item, "", SearchOption.AllDirectories);
                                                    var m64ReleaseLibDll = sdkModel?.gDll64?.gDebug.gLibDir.gLibSources;
                                                    foreach (var curLibItem in curLibLists)
                                                    {
                                                        if (curLibItem.ToUpper().Contains(".LIB"))
                                                        {
                                                            m64ReleaseLibDll.Add(GetDirNameFromPath(curLibItem));
                                                        }
                                                    }
                                                    break;
                                                }
                                            case "DEBUG":
                                                {
                                                    break;
                                                }
                                            case "INCLUDE":
                                                {
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                    }
                                }
                                else if(mDebugOrRelease.ToUpper().Contains("RELEASE"))
                                {
                                    //64 - Release
                                    //获取到dll / lib /  include 
                                    var m_lib_inc_dlls = Directory.GetDirectories(mDebugOrReleaseDirItem, "", SearchOption.TopDirectoryOnly);
                                    foreach (var mLib_inc_dlls_item in m_lib_inc_dlls)
                                    {
                                        ///文件夹的路径已经获取完毕
                                        var curLibName = mDebugOrReleaseDirItem.ToUpper();
                                        switch (curLibName)
                                        {
                                            case "LIB":
                                                {
                                                    //获取当前所有的文件,而且是.lib的文件.
                                                    var curLibLists = Directory.GetDirectories(mLib_inc_dlls_item,"", SearchOption.AllDirectories);
                                                    var m64ReleaseLibDll = sdkModel?.gDll64?.gRelease.gLibDir.gLibSources;
                                                    foreach (var curLibItem in curLibLists)
                                                    {
                                                        if(curLibItem.ToUpper().Contains(".LIB"))
                                                        {
                                                            m64ReleaseLibDll.Add(GetDirNameFromPath(curLibItem));
                                                        }
                                                    }
                                                    break;
                                                }
                                            case "DEBUG":
                                                {
                                                    break;
                                                }
                                            case "INCLUDE":
                                                {
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                    }

                                }

                            }
                        }

                    }
                }

            }

        }

        public void Dispose()
        {
            mCreateDirWnd?.Close();
        }
    }
}
