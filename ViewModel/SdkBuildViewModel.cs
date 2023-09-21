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
using System.Xml;
using System.Xml.Linq;
using System.Windows;

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

                var curVcxProjFile = gBuildModel.gVcxprojFile;


                //WriteChangedVcxProj(gBuildModel.gVcxprojFile); //数据写入
                //解析内容.

                var curStr = BuildVcxprojFile(curVcxProjFile);

                if (mShowContentWnd==null)
                {
                    mShowContentWnd = new ShowContent();
                }
                //var content = ReadVcxProj(curVcxProjFile);//读取到的内容.
                mShowContentWnd.SetVcxProjContentStr(curStr);
                mShowContentWnd.ShowWnd();

            });

   

        }

        /// <summary>
        /// 设置一个新功能,输入标记的节点,返回一个Dictionary<string,list<string>> node-Name: noe-Value;的数值.
        /// </summary>
        /// <param name="vcxProjFile"></param>
        /// <returns></returns>
        private string ReadVcxProj(string vcxProjFile)
        {
            StringBuilder sb = new StringBuilder();

            GlobalGetErrorHelper.GetError(() => {

 
                XmlDocument doc = new XmlDocument();
                // 加载 vcxproj 文件
                doc.Load(vcxProjFile);
                // 获取根元素 Project
                XmlElement root = doc.DocumentElement;

                {
                    string resultStr = string.Empty;
                    int index = 0;
                    List<SearchNode> searchList = new List<SearchNode>();
                    {
                        //写入搜索内容.
                        SearchNode T1 = new SearchNode(@"ItemDefinitionGroup", "'$(Configuration)|$(Platform)'=='Debug|x64'");
                        SearchNode T2 = new SearchNode(@"ClCompile");
                        SearchNode T3 = new SearchNode(@"AdditionalIncludeDirectories");

                        searchList.Add(T1);
                        searchList.Add(T2);
                        searchList.Add(T3);
                    }
                    XmlNodeHelper.GetContentValueFromNodeLists(root, searchList, ref resultStr, ref index);


                    sb.AppendLine($"附加文件目录:Debug|x64 \r\n");
                    sb.AppendLine(resultStr);
                    sb.AppendLine("\r\n");
                }

                {
                    string resultStr = string.Empty;
                    int index = 0;
                    List<SearchNode> searchList = new List<SearchNode>();
                    {
                        //写入搜索内容.
                        SearchNode T1 = new SearchNode(@"ItemDefinitionGroup", "'$(Configuration)|$(Platform)'=='Debug|x64'");
                        SearchNode T2 = new SearchNode(@"Link");
                        SearchNode T3 = new SearchNode(@"AdditionalLibraryDirectories");

                        searchList.Add(T1);
                        searchList.Add(T2);
                        searchList.Add(T3);
                    }
                    XmlNodeHelper.GetContentValueFromNodeLists(root, searchList, ref resultStr, ref index);


                    sb.AppendLine($"附加库目录:Debug|x64 \r\n");
                    sb.AppendLine(resultStr);
                    sb.AppendLine("\r\n");
                }

                {
                    string resultStr = string.Empty;
                    int index = 0;
                    List<SearchNode> searchList = new List<SearchNode>();
                    {
                        //写入搜索内容.
                        SearchNode T1 = new SearchNode(@"ItemDefinitionGroup", "'$(Configuration)|$(Platform)'=='Debug|x64'");
                        SearchNode T2 = new SearchNode(@"Link");
                        SearchNode T3 = new SearchNode(@"AdditionalDependencies");

                        searchList.Add(T1);
                        searchList.Add(T2);
                        searchList.Add(T3);
                    }
                    XmlNodeHelper.GetContentValueFromNodeLists(root, searchList, ref resultStr, ref index);


                    sb.AppendLine($"附加依赖项:Debug|x64 \r\n");
                    sb.AppendLine(resultStr);
                    sb.AppendLine("\r\n");
                }

            });
            return sb.ToString();
        }

        private void WriteChangedVcxProj(string vcxProjFile)
        {
            XmlDocument doc = new XmlDocument();
            // 加载 vcxproj 文件
            doc.Load(vcxProjFile);
            // 获取根元素 Project
            XmlElement root = doc.DocumentElement;

            //修改的内容.
            {

                string resultStr = "----------我被修改了------------";
                int index = 0;
                List<SearchNode> searchList = new List<SearchNode>();
                {
                    //写入搜索内容.
                    SearchNode T1 = new SearchNode(@"ItemDefinitionGroup", "'$(Configuration)|$(Platform)'=='Debug|x64'");
                    SearchNode T2 = new SearchNode(@"ClCompile");
                    SearchNode T3 = new SearchNode(@"AdditionalIncludeDirectories");

                    searchList.Add(T1);
                    searchList.Add(T2);
                    searchList.Add(T3);
                }
                XmlNodeHelper.ChangeContent2XmlNode(root, searchList,  resultStr,true, ref index);
            }

            //修改之后,重新保存为配置文件.
            var vcxProjDir = System.IO.Path.GetDirectoryName(vcxProjFile);
            var NewVcxProjFile = $"{vcxProjDir}\\newPro.vcxproj";
            doc.Save(NewVcxProjFile);
        }

        private string ReadVcxProj_标准(string vcxProjFile)
        {

            StringBuilder sb = new StringBuilder();

            GlobalGetErrorHelper.GetError(() => {
                // 创建一个 XmlDocument 对象
                XmlDocument doc = new XmlDocument();
                // 加载 vcxproj 文件
                doc.Load(vcxProjFile);
                // 获取根元素 Project
                XmlElement root = doc.DocumentElement;
                // 遍历 Project 的子节点
                foreach (XmlNode node in root.ChildNodes)
                {
                    // 输出节点的名称
                    Console.WriteLine("Module: {0}", node.Name);
                    // 输出节点的内容
                    Console.WriteLine("Content: {0}", node.InnerXml);
                    // 输出空行
                    Console.WriteLine();

                    // 判断节点是否是 ItemDefinitionGroup 元素
                    if (node.Name == "ItemDefinitionGroup")
                    {
                        // 获取节点的 Condition 属性
                        string condition = node.Attributes["Condition"].Value; //输出 Condition 属性的值
                        Console.WriteLine("Condition: {0}", condition);//判断 Condition 属性是否等于 "'$(Configuration)|$(Platform)'=='Debug|x64'"
                        if (condition == "'$(Configuration)|$(Platform)'=='Debug|x64'")
                        {
                            sb.AppendLine("添加 -------- Debug| x64");
                            //到这一步,获取到ClCompile + Link 两个子节点.

                            //从这两个子节点中:clCompile AdditionalIncludeDirectories (include文件夹) 

                            //Line: AdditionalLibraryDirectories (lib文件夹) + AdditionalDependencies (输入依赖项)

                            //Console.WriteLine("Content: {0}", node.InnerXml);
                            //Console.WriteLine();

                            //sb.AppendLine(node.Name);
                            //sb.AppendLine(node.InnerXml);
                            if(node.HasChildNodes==true)
                            {
                                foreach (XmlNode nodeItem in node.ChildNodes)
                                {
                              
                                    if (nodeItem.Name == "ClCompile")
                                    {
                                        sb.AppendLine(nodeItem.Name);
                                        nodeItem.InnerText+=";改变内容;";
                                        sb.AppendLine(nodeItem.InnerText);

                                    }
                                    else if (nodeItem.Name == "Link")
                                    {
                                        sb.AppendLine(nodeItem.Name);
                                        if(nodeItem.HasChildNodes)
                                        {
                                            foreach (XmlNode node2 in nodeItem.ChildNodes)
                                            {
                                                sb.AppendLine(node2.Name);
                                                sb.AppendLine(node2.InnerText);
                                            }
                                        }
                                      
                                    }
                                }
                            }

                        }
                        else if (condition == "'$(Configuration)|$(Platform)'=='Release|x64'")
                        {
                            sb.AppendLine("添加 -------- Release| x64");
                        }
                        else if (condition == "'$(Configuration)|$(Platform)'=='Debug|Win32'")
                        {
                            sb.AppendLine("添加 -------- Debug|Win32");
                        }
                        else if (condition == "'$(Configuration)|$(Platform)'=='Release|Win32'")
                        {
                            sb.AppendLine("添加 -------- Release|Win32");
                        }
                    }

                  
                }

            });
            return sb.ToString();
        }


        private string BuildVcxprojFile(string vcxProjFile)
        {
            string str = "";
            if (System.Windows.MessageBox.Show("在修改之前,请保存当前配置文件的副本","",MessageBoxButton.OKCancel)== MessageBoxResult.OK)
            {

                if(!File.Exists(vcxProjFile))
                {
                    System.Windows.MessageBox.Show("当前配置文件路径为空,请设置 .vcxproj 文件");
                    return str;
                }

                XmlDocument doc = new XmlDocument();
                // 加载 vcxproj 文件
                doc.Load(vcxProjFile);
                // 获取根元素 Project
                XmlElement root = doc.DocumentElement;


              
                //return;
                //获取查阅的内容.
                UpdateSdkStr();

                //获取内容.写入内容.
                //拼接字符串后->转换成枚举->循环写入数据.
                var isCover = gBuildModel.gIsCover;
                for (int i = 0;i<(int)linkType.Link_Numbers;i++)
                {
                    var contentStr = GetLinkStr((linkType)i);
                    //解析三个模式.
                    PlateForm plateForm = PlateForm.x64;
                    DebugType mDebug = DebugType.Debug;
                    DllType mDlltype = DllType.None;

                    //转换数据.
                    var curLinkStr = ((linkType)i).ToString();
                    var curLinkLists = curLinkStr.Split('_');
                    if (curLinkLists.Length!=3) continue;
                    Enum.TryParse(curLinkLists[0], out plateForm);
                    Enum.TryParse(curLinkLists[1], out mDebug);
                    Enum.TryParse(curLinkLists[2], out mDlltype);
                    XmlNodeHelper.WriteContent2VcxProjXmlNode(root, contentStr, isCover, plateForm,mDebug,mDlltype);
                }
                doc.Save(vcxProjFile);


                //格式化内容:
                //string formattedXml = root.OuterXml;
                //formattedXml = string.Replace(formattedXml, "<", "<\n");
                //formattedXml = System.Text.RegularExpressions.Regex.Replace(formattedXml, "(?<=>)[^<]*(?=<)", "\t");
                //str = formattedXml;
               
                // 创建一个 StringBuilder 对象用于存储输出的内容
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                XmlTextWriter writer = new XmlTextWriter(sw);
                // 设置 XmlTextWriter 的格式选项
                writer.Formatting = Formatting.Indented; // 使用缩进格式
                writer.Indentation = 4; // 设置缩进空格数为 4
                writer.IndentChar = ' '; // 设置缩进字符为空格
                  // 将 XmlDocument 的内容写入 XmlTextWriter
                doc.WriteContentTo(writer);
                // 关闭 XmlTextWriter 和 StringWriter
                writer.Close();
                sw.Close();
                str = sb.ToString();
            }

            return str;
        }

        public enum linkType : int
        {
            x64_Debug_Lib = 0,
            x64_Debug_Dll,
            x64_Debug_Include,
            x64_Debug_LibLists,
            
            x64_Release_Lib,
            x64_Release_Dll,
            x64_Release_Include,
            x64_Release_LibLists,

            Win32_Debug_Lib,
            Win32_Debug_Dll,
            Win32_Debug_Include,
            Win32_Debug_LibLists,

            Win32_Release_Lib,
            Win32_Release_Dll,
            Win32_Release_Include,
            Win32_Release_LibLists,

            Link_Numbers

        }

        /// <summary>
        /// 获取link的内容.
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        private string GetLinkStr(linkType mtype)
        {
            return SdkStrDes[(int)mtype];
        }

        /// <summary>
        /// 
        /// </summary>
        private string[] SdkStrDes = new string[16];
        private void UpdateSdkStr()
        {

            for (int i = 0; i < SdkStrDes.Length; i++)
            {
                SdkStrDes[i] = "";
            }
            string postFix = "\r\n";
            var curSdkDir = $"{gBuildModel.gSdkDir}\\";
            foreach (var sdkItem in gBuildModel?.gBuildSdkSource)
            {
                //64/debug/
                if (sdkItem.gDll64.gIsChecked==true)
                {
                    if (sdkItem.gDll64.gDebug.gIsChecked==true)
                    {
                        SdkStrDes[0]+=$"{curSdkDir}{sdkItem.DllName_64_Debug_Lib_String()};{postFix}";
                        SdkStrDes[1]+=$"{curSdkDir}{sdkItem.DllName_64_Debug_Dll_String()};{postFix}";
                        SdkStrDes[2]+=$"{curSdkDir}{sdkItem.DllName_64_Debug_Include_String()};{postFix}";
                        foreach (var libName in sdkItem.gDll64.gDebug.gLibDir.gLibSources)
                        {
                            SdkStrDes[3]+=$"{libName}{postFix}";
                        }
                    }
                    if (sdkItem.gDll64.gRelease.gIsChecked==true)
                    {
                        //64/Release/
                        SdkStrDes[4]+=$"{curSdkDir}{sdkItem.DllName_64_Release_Lib_String()};{postFix}";
                        SdkStrDes[5]+=$"{curSdkDir}{sdkItem.DllName_64_Release_Dll_String()};{postFix}";
                        SdkStrDes[6]+=$"{curSdkDir}{sdkItem.DllName_64_Release_Include_String()};{postFix}";
                        foreach (var libName in sdkItem.gDll64.gRelease.gLibDir.gLibSources)
                        {
                            SdkStrDes[7]+=$"{libName}{postFix}";
                        }
                    }

                }

                if (sdkItem.gDll32.gIsChecked==true)
                {
                    if (sdkItem.gDll32.gDebug.gIsChecked==true)
                    {

                        SdkStrDes[8]+=$"{curSdkDir}{sdkItem.DllName_32_Debug_Lib_String()};{postFix}";
                        SdkStrDes[9]+=$"{curSdkDir}{sdkItem.DllName_32_Debug_Dll_String()};{postFix}";
                        SdkStrDes[10]+=$"{curSdkDir}{sdkItem.DllName_32_Debug_Include_String()};{postFix}";
                        foreach (var libName in sdkItem.gDll32.gDebug.gLibDir.gLibSources)
                        {
                            SdkStrDes[11]+=$"{libName}{postFix}";
                        }

                    }
                    if (sdkItem.gDll32.gRelease.gIsChecked==true)
                    {
                        SdkStrDes[12]+=$"{curSdkDir}{sdkItem.DllName_32_Release_Lib_String()};{postFix}";
                        SdkStrDes[13]+=$"{curSdkDir}{sdkItem.DllName_32_Release_Dll_String()};{postFix}";
                        SdkStrDes[14]+=$"{curSdkDir}{sdkItem.DllName_32_Release_Include_String()};{postFix}";
                        foreach (var libName in sdkItem.gDll32.gRelease.gLibDir.gLibSources)
                        {
                            SdkStrDes[15]+=$"{libName}{postFix}";
                        }
                    }
                }

            }
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

            UpdateSdkStr();

            var strFormat = (string str) =>
            {

                return $"------------------------{str}---------------------------\r\n";
            };

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < SdkStrDes.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        {
                            sb.AppendLine(strFormat("x64-Debug"));
                            break;
                        }
                    case 4:
                        {
                            sb.AppendLine(strFormat("x64-Release"));
                            break;
                        }
                    case 8:
                        {
                            sb.AppendLine(strFormat("x32-Debug"));
                            break;
                        }
                    case 12:
                        {
                            sb.AppendLine(strFormat("x32-Release"));
                            break;
                        }
                    default:
                        break;
                }
                sb.AppendLine(SdkStrDes[i]);
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
            var GetFileNameWithExtension = (string str) =>
            {

                return System.IO.Path.GetFileName(str);
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
                    var nameList =GetDirNameFromPath(dllDirItem).Split('-').ToList();//使用"-"分割文字,获取第一个;;
                    var dllName = nameList.FirstOrDefault();
                    var dllVersion = "";
                    if(nameList.Count>1) dllVersion= nameList[1];
                    sdkModelItem sdkModel = new sdkModelItem(dllName);
                    sdkModel.gDllVersion = dllVersion; 
                    compelierModel.gSdkItemList.Add(sdkModel);
 
                    //这一层,获取到所有的库目录
                    var m64Or32Dir = Directory.GetDirectories(dllDirItem, "", SearchOption.TopDirectoryOnly);
                    foreach (var m63Or32Item in m64Or32Dir)
                    {
                        var m32or64Name = GetDirNameFromPath(m63Or32Item);
                        //32位
                        if (m63Or32Item.ToUpper().Contains("X32"))
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
                                        var curLibName = GetDirNameFromPath(mLib_inc_dlls_item).ToUpper();
                                        switch (curLibName)
                                        {
                                            case "LIB":
                                                {
                                                    //获取当前所有的文件,而且是.lib的文件.
                                                    var curLibLists = Directory.GetFiles(mLib_inc_dlls_item, "", SearchOption.AllDirectories);
                                                    var mLibLists = sdkModel?.gDll32?.gDebug.gLibDir.gLibSources;
                                                    foreach (var curLibItem in curLibLists)
                                                    {
                                                        if (curLibItem.ToUpper().Contains(".LIB"))
                                                        {
                                                            mLibLists.Add(GetFileNameWithExtension(curLibItem));
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
                                        var curLibName = GetDirNameFromPath(mLib_inc_dlls_item).ToUpper();
                                        switch (curLibName)
                                        {
                                            case "LIB":
                                                {
                                                    //获取当前所有的文件,而且是.lib的文件.
                                                    var curLibLists = Directory.GetFiles(mLib_inc_dlls_item, "", SearchOption.AllDirectories);
                                                    var mLibLists = sdkModel?.gDll32?.gRelease.gLibDir.gLibSources;
                                                    foreach (var curLibItem in curLibLists)
                                                    {
                                                        if (curLibItem.ToUpper().Contains(".LIB"))
                                                        {
                                                            mLibLists.Add(GetFileNameWithExtension(curLibItem));
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
                                        var curLibName = GetDirNameFromPath(mLib_inc_dlls_item).ToUpper();
                                        switch (curLibName)
                                        {
                                            case "LIB":
                                                {
                                                    //获取当前所有的文件,而且是.lib的文件.
                                                    var curLibLists = Directory.GetFiles(mLib_inc_dlls_item, "", SearchOption.AllDirectories);
                                                    var mLibLists = sdkModel?.gDll64?.gDebug.gLibDir.gLibSources;
                                                    foreach (var curLibItem in curLibLists)
                                                    {
                                                        if (curLibItem.ToUpper().Contains(".LIB"))
                                                        {
                                                            mLibLists.Add(GetFileNameWithExtension(curLibItem));
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
                                        var curLibName = GetDirNameFromPath(mLib_inc_dlls_item).ToUpper();
                                        switch (curLibName)
                                        {
                                            case "LIB":
                                                {
                                                    //获取当前所有的文件,而且是.lib的文件.
                                                    var curLibLists = Directory.GetFiles(mLib_inc_dlls_item,"", SearchOption.AllDirectories);
                                                    var mLibLists = sdkModel?.gDll64?.gRelease.gLibDir.gLibSources;
                                                    foreach (var curLibItem in curLibLists)
                                                    {
                                                        if(curLibItem.ToUpper().Contains(".LIB"))
                                                        {
                                                            mLibLists.Add(GetFileNameWithExtension(curLibItem));
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
