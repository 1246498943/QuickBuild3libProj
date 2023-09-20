using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Xml;

namespace XPloteQuickBuidProj
{
    public enum PlateForm
    {
        x64,
        Win32
    }
    public enum DebugType
    {
        Debug,
        Release
    }

    public enum DllType
    {
        None,
        Include,
        Lib,
        LibLists,
        Dll,
    }

    /// <summary>
    /// 搜索类
    /// 注意: include, lib, dll的内容后面,都要跟随对应的 AddDependXXXX等内容; 而且其每个条块内容,是以 ; 隔离.
    /// </summary>
    public class SearchNode
    {
        public string NodeName;
        public string AttribConditionValue;//属性条件
        public bool IsContition= false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mNodeName"></param>
        /// <param name="mCondition"></param>
        /// <param name="mIsCondition">是否搜索条件变量</param>
        public SearchNode(string mNodeName,string mCondition,bool mIsCondition) 
        {
            NodeName = mNodeName;
            AttribConditionValue = mCondition;
            IsContition = mIsCondition;
        }
        public SearchNode(string mNodeName, string mCondition)
        {
            NodeName = mNodeName;
            AttribConditionValue = mCondition;
            IsContition = mCondition.Length>1?true:false;
        }
        public SearchNode(string mNodeName)
        {
            NodeName = mNodeName;
            IsContition = false;
        }

    }
    public class XmlNodeHelper
    {
        public XmlNodeHelper()
        {

        }

        /// <summary>
        /// 从指定层级中,获取指定的数据.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mNodeNameLists"></param>
        /// <param name="ResultStr"></param>
        /// <param name="mIndex"></param>
        public static void GetContentValueFromNodeLists(XmlNode node,List<SearchNode> mNodeNameLists,ref string ResultStr,ref int mIndex)
        {
            if (node==null) return ;
            if (mIndex>mNodeNameLists.Count-1) return;
            if (node.HasChildNodes)
            {
                var curSearchNode = mNodeNameLists[mIndex];
                foreach (XmlNode nodeItem in node.ChildNodes)
                {
                    if(nodeItem.Name== curSearchNode.NodeName)
                    {
                        ResultStr = nodeItem.InnerText;
                        if (curSearchNode.IsContition==true) //一层是判断属性条件 / 一层树不需要判断.
                        {
                            string conditionStr = nodeItem.Attributes["Condition"].Value;
                            if(conditionStr == curSearchNode.AttribConditionValue)
                            {
                                //递归到了下一层节点
                               mIndex+=1;
                               GetContentValueFromNodeLists(nodeItem, mNodeNameLists,ref ResultStr,ref mIndex);
                                
                            }
                        }
                        else
                        {
                            mIndex+=1;
                            GetContentValueFromNodeLists(nodeItem, mNodeNameLists, ref ResultStr,ref mIndex);
                        }
                        return;
                    }
                    else
                    {
                         GetContentValueFromNodeLists(nodeItem, mNodeNameLists,ref ResultStr,ref mIndex);
                    }

                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 通过配置属性,获取输出的Node内容.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mNodeNameLists"></param>
        /// <param name="outNode"></param>
        /// <param name="mIndex"></param>
        public static void GetXmlNodeValueFromNodeLists(XmlNode node, List<SearchNode> mNodeNameLists, ref XmlNode outNode, ref int mIndex)
        {
            if (node==null) return;
            if (mIndex>mNodeNameLists.Count-1) return;
            if (node.HasChildNodes)
            {
                var curSearchNode = mNodeNameLists[mIndex];
                foreach (XmlNode nodeItem in node.ChildNodes)
                {
                    if (nodeItem.Name== curSearchNode.NodeName)
                    {
                        outNode = nodeItem;
                        if (curSearchNode.IsContition==true) //一层是判断属性条件 / 一层树不需要判断.
                        {
                            string conditionStr = nodeItem.Attributes["Condition"].Value;
                            if (conditionStr == curSearchNode.AttribConditionValue)
                            {
                                //递归到了下一层节点
                                mIndex+=1;
                                GetXmlNodeValueFromNodeLists(nodeItem, mNodeNameLists, ref outNode, ref mIndex);

                            }
                        }
                        else
                        {
                            mIndex+=1;
                            GetXmlNodeValueFromNodeLists(nodeItem, mNodeNameLists, ref outNode, ref mIndex);
                        }
                        return;
                    }
                    else
                    {
                        GetXmlNodeValueFromNodeLists(nodeItem, mNodeNameLists, ref outNode, ref mIndex);
                    }

                }
            }
            else
            {
                return;
            }
        }


        /// <summary>
        /// 修改指定内容;
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mNodeNameLists"></param>
        /// <param name="ContentStr"></param>
        /// <param name="IsOverride">是否覆盖? True:覆盖; False:在后面补充</param>
        /// <param name="mIndex"></param>
        public static void ChangeContent2XmlNode(XmlNode node, List<SearchNode> mNodeNameLists, string ContentStr, bool IsOverride,ref int mIndex)
        {
            if (node==null) return;
            if (mIndex>mNodeNameLists.Count-1) return;
            if (node.HasChildNodes)
            {
                var curSearchNode = mNodeNameLists[mIndex];
                foreach (XmlNode nodeItem in node.ChildNodes)
                {
                    if (nodeItem.Name== curSearchNode.NodeName)
                    {
                        //修改内容...
                        if (mIndex == mNodeNameLists.Count-1)
                        {
                            if(IsOverride==true)
                            {
                                nodeItem.InnerText = $"{ContentStr}";
                            }
                            else
                            {
                                nodeItem.InnerText += $"{ContentStr}";
                            }
                          
                        }

                        if (curSearchNode.IsContition==true) //一层是判断属性条件 / 一层树不需要判断.
                        {
                            string conditionStr = nodeItem.Attributes["Condition"].Value;
                            if (conditionStr == curSearchNode.AttribConditionValue)
                            {
                                //递归到了下一层节点
                                mIndex+=1;
                                ChangeContent2XmlNode(nodeItem, mNodeNameLists,  ContentStr, IsOverride, ref mIndex);

                            }
                        }
                        else
                        {
                            mIndex+=1;
                            ChangeContent2XmlNode(nodeItem, mNodeNameLists,  ContentStr, IsOverride, ref mIndex);
                        }
                        return;
                    }
                    else
                    {
                        GetContentValueFromNodeLists(nodeItem, mNodeNameLists, ref ContentStr, ref mIndex);
                    }

                }
            }
            else
            {
                return;
            }
        }

        private static void IsExitsXmlNode(XmlNode node, List<SearchNode> mNodeNameLists,List<bool> SearchStatus,ref int mIndex)
        {
            if (node==null) return;
            if (mIndex>mNodeNameLists.Count-1) return;
            if (node.HasChildNodes)
            {
                var curSearchNode = mNodeNameLists[mIndex];
                SearchStatus[mIndex] = false;
                foreach (XmlNode nodeItem in node.ChildNodes)
                {
                    if (nodeItem.Name== curSearchNode.NodeName)
                    {
                        SearchStatus[mIndex] = true;
                        if (curSearchNode.IsContition==true) //一层是判断属性条件 / 一层树不需要判断.
                        {
                            bool isConditionExits = false;
                            foreach (XmlNode condiAttirbu in nodeItem.Attributes)
                            {
                                if (condiAttirbu.Name.Contains("Condition"))
                                {
                                    isConditionExits = true;
                                    break;
                                }
                            }
                            if(isConditionExits==true)
                            {
                                string conditionStr = nodeItem.Attributes["Condition"].Value;
                                if (conditionStr == curSearchNode.AttribConditionValue)
                                {
                                    //递归到了下一层节点
                                    mIndex+=1;
                                    IsExitsXmlNode(nodeItem, mNodeNameLists, SearchStatus, ref mIndex);

                                }
                            }
                            
                        }
                        else
                        {
                            mIndex+=1;
                            IsExitsXmlNode(nodeItem, mNodeNameLists, SearchStatus, ref mIndex);
                        }
                        return;
                    }
                    else
                    {
                        IsExitsXmlNode(nodeItem, mNodeNameLists, SearchStatus, ref mIndex);
                    }

                }
            }
            else
            {
                return;
            }
        }

        public static bool IsExitsXmlNode(XmlNode node, List<SearchNode> mNodeNameLists)
        {
            bool status = false;
            List<bool> listStatus = new List<bool>();
            for(int i =0;i<mNodeNameLists.Count; i++)
            {
                listStatus.Add(false);
            }
            int index = 0;
            IsExitsXmlNode(node, mNodeNameLists, listStatus, ref index);
            for(int i=0;i<listStatus.Count;i++)
            {
                if (listStatus[i] == false)
                {
                    status = false;
                    break;
                }
            }
            return status;
        }

        /// <summary>
        /// 记录其第几层是没有节点的...
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mNodeNameLists"></param>
        /// <param name="SearchStatus"></param>
        /// <param name="mIndex"></param>
        private static void CheckIsAddXmlNode(XmlNode node, List<SearchNode> mNodeNameLists, List<bool> SearchStatus,ref List<XmlNode> outNodeList, ref int mIndex)
        {
            if (node==null) return;
            if (mIndex>mNodeNameLists.Count-1) return;
            if (node.HasChildNodes)
            {
                var curSearchNode = mNodeNameLists[mIndex];
                SearchStatus[mIndex] = false;
                foreach (XmlNode nodeItem in node.ChildNodes)
                {
                    if (nodeItem.Name== curSearchNode.NodeName)
                    {
                        SearchStatus[mIndex] = true;
                        outNodeList.Add(nodeItem);
                        if (curSearchNode.IsContition==true) //一层是判断属性条件,有判断条件的这一层.
                        {
                            bool isConditionExits = false;
                            foreach (XmlNode condiAttirbu in nodeItem.Attributes)
                            {
                                if(condiAttirbu.Name.Contains("Condition"))
                                {
                                    isConditionExits = true;
                                    break;
                                }
                            }
                            if(isConditionExits==true)
                            {
                                string conditionStr = nodeItem.Attributes["Condition"].Value;
                                if (conditionStr == curSearchNode.AttribConditionValue)
                                {
                                    //递归到了下一层节点
                                    mIndex+=1;
                                    CheckIsAddXmlNode(nodeItem, mNodeNameLists, SearchStatus,ref outNodeList, ref mIndex);

                                }
                            }
                           
                        }
                        else
                        {
                            mIndex+=1;
                            CheckIsAddXmlNode(nodeItem, mNodeNameLists, SearchStatus,ref outNodeList, ref mIndex);
                        }
                        return;
                    }
                    else
                    {
                        CheckIsAddXmlNode(nodeItem, mNodeNameLists, SearchStatus,ref outNodeList, ref mIndex);
                    }

                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 如果检测到没有该模块,会自动创建...
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mNodeNameLists"></param>
        public static void AutoCreateAndAddXmlNode(XmlNode node, List<SearchNode> mNodeNameLists)
        {
            List<bool> listStatus = new List<bool>();
            List<XmlNode> listNodes = new List<XmlNode>();
            for (int i = 0; i<mNodeNameLists.Count; i++)
            {
                listStatus.Add(false);
            }
            int index = 0;
            XmlNode curNode = null;
            CheckIsAddXmlNode(node, mNodeNameLists, listStatus,ref listNodes, ref index); //这里,这个ListNodes,
            if (listNodes.Count>0) curNode = listNodes[0];
            else curNode = node;
            for (int i=0;i<listStatus.Count;i++)
            {
                //添加循环遍历,在第几层.
                var curSearchNode = mNodeNameLists[i];//当前的模块节点.
                if (curNode.HasChildNodes)
                {
                    //在最后添加.
                    if (listStatus[i]==false)//当前没有该节点,需要在当前节点下创建一个新的节点.
                    {
                        ////循环遍历这一层节点.判断是否已经存在该节点.
                        bool isExitCurSearchNodeName = false;
                        if (curNode.HasChildNodes)
                        {
                            foreach (XmlNode item in curNode.ChildNodes)
                            {
                                if(item.Name == curSearchNode.NodeName)
                                {
                                    isExitCurSearchNodeName = true;
                                    curNode =item;
                                    break;
                                }
                            }
                        }

                        if(isExitCurSearchNodeName==false)
                        {
                            //才会去创建新的节点.
                            // var nRoot = curNode.OwnerDocument.CreateNode(XmlNodeType.Element, curSearchNode.NodeName, "");
                            var nRoot = curNode.OwnerDocument.CreateElement(curSearchNode.NodeName,null);
                            curNode.AppendChild(nRoot);
                            if (curSearchNode.IsContition==true)
                            {
                                //添加一个属性.
                                nRoot.Attributes["Condition"].Value = curSearchNode.AttribConditionValue;
                            }
                            curNode = nRoot;
                        }


                    }
                    else
                    {
                        if (i<listNodes.Count) curNode =listNodes[i];
                    }
                }

            }
        }

        /// <summary>
        /// 通过枚举组合写入对应的内容 -> 最终是重写写成配置文件.
        /// </summary>
        public static void WriteContent2VcxProjXmlNode(XmlNode node, string ContentStr, bool IsOverride, PlateForm mX64Or32, DebugType mDebug, DllType mDllType)
        {
            var Params = GetLevelNodeDes(mX64Or32, mDebug, mDllType);
            var listSearch = Params.Item1;
            var postFix = Params.Item2;

            XmlNodeHelper.AutoCreateAndAddXmlNode(node, listSearch);//自动构建
            //
            //在获取到模块之后,如果检测到没有的话...就给其添加...
            bool status = IsExitsXmlNode(node, listSearch);
            if(status==false)
            {
                GlobalSingleHelper.SendLogInfo("没有找到对应的模块,请在原始vcxproj中,手动设置对应的x64-Debug/Release等模式");
                return;
            }

            //注意,这个contentStr,要去掉换行符文;然后在最后,添加一个依赖后缀.
            string output = ContentStr.Replace("\r", "").Replace("\n", "");
            output+=postFix;
            int index = 0;
            ChangeContent2XmlNode(node,listSearch, output, IsOverride,ref index);

        }





        //依据枚举,获取对应的List<SearchNodel> 内容+后缀;
        public static Tuple<List<SearchNode>,string> GetLevelNodeDes(PlateForm mX64Or32, DebugType mDebug, DllType mDllType)
        {
            List<SearchNode> result = new List<SearchNode>();//描述符文.
            string postFixDes = string.Empty;//后缀.


            switch (mDllType)
            {
                case DllType.None:
                    {
                        break;
                    }
                case DllType.Include:
                    {
                        SearchNode T1 = new SearchNode(@"ItemDefinitionGroup", $"'$(Configuration)|$(Platform)'=='{mDebug.ToString()}|{mX64Or32.ToString()}'");
                        SearchNode T2 = new SearchNode(@"ClCompile");
                        SearchNode T3 = new SearchNode(@"AdditionalIncludeDirectories");
                        result.Add(T1);
                        result.Add(T2);
                        result.Add(T3);
                        postFixDes = @";%(AdditionalIncludeDirectories)";
                        break;
                    }
                case DllType.Lib:
                    {
                        SearchNode T1 = new SearchNode(@"ItemDefinitionGroup", $"'$(Configuration)|$(Platform)'=='{mDebug.ToString()}|{mX64Or32.ToString()}'");
                        SearchNode T2 = new SearchNode(@"Link");
                        SearchNode T3 = new SearchNode(@"AdditionalLibraryDirectories");
                        result.Add(T1);
                        result.Add(T2);
                        result.Add(T3);
                        postFixDes = @";%(AdditionalLibraryDirectories)";
                        break;
                    }
                case DllType.LibLists:
                    {
                        SearchNode T1 = new SearchNode(@"ItemDefinitionGroup", $"'$(Configuration)|$(Platform)'=='{mDebug.ToString()}|{mX64Or32.ToString()}'");
                        SearchNode T2 = new SearchNode(@"Link");
                        SearchNode T3 = new SearchNode(@"AdditionalDependencies");
                        result.Add(T1);
                        result.Add(T2);
                        result.Add(T3);
                        postFixDes = @";%(AdditionalDependencies)";
                        break;
                    }
                case DllType.Dll:
                    {
                        //暂时不添加....要不要添加? //修改的是  xxx.vcxproj.user 这个文件夹....
                        break;
                    }
                default:
                    break;
            }

            return new(result, postFixDes);
        }


        public static void TestMain()
        {
#if false
           ///////////案例以及使用方式....

            string vcxProjFile = "";
            XmlDocument doc = new XmlDocument();
            // 加载 vcxproj 文件
            doc.Load(vcxProjFile);
            // 获取根元素 Project
            XmlElement root = doc.DocumentElement;


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

#endif
        }

    }
}
