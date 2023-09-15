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
            gCreateDirModel = new sdkCreateDirModel();
            InitCommands();
            GlobalSingleHelper.SendCheckHandler+=GlobalSingleHelper_SendCheckHandler;
        }

        private void GlobalSingleHelper_SendCheckHandler(sdkModelItem obj)
        {
            //一些关键信号触发的时候,在这里检测
            if (obj!=null)
            {
                if (obj.gIsChecked==true)
                {

                }
                else
                {

                }

            }
        }

        public ICommand? gSetSdkDir { get; set; }      //设置目录.
        public ICommand? gImportSdk2List { get; set; } //导入库
        public ICommand? gCreateStructDirs { get; set; }//在当前的Dir下,创建新的结构目录.




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

            var curDllSourceList = gBuildModel?.gSdkSource;
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
