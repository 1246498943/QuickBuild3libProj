using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPloteQuickBuidProj
{
    /// <summary>
    ///按照目录来说,使用结构名字,创建子库的文件夹目录.
    /// </summary>
    public class SdkDirHelper
    {

        /// <summary>
        /// 从parentDir 下,创建名字为 mLibDirName 的文件夹组织结构.
        /// 要有编译器版本号
        /// </summary>
        /// <param name="mLibDirName">库名字</param>
        /// <param name="mCompilerName">编译器版本名字: vs2019/vs2022/minWin等等</param>
        /// <param name="parentDir">SDK所在文件夹</param>
        public static bool MakeDllDirPaths1(string mLibDirName, string mCompilerName, string version, string parentDir)
        {
            if (mLibDirName.Length<1 || mCompilerName.Length<1 || parentDir.Length<1) return false;
            string[] mEnFramePlate = new string[2] { "x32", "x64" };
            string[] mDRs = new string[2] { "Debug", "Release" };
            string[] mLibs = new string[3] { "include", "lib", "dll" };
            for (int i = 0; i < mEnFramePlate.Length; i++)
            {
                for (int j = 0; j<mDRs.Length; j++)
                {
                    for (int k = 0; k<mLibs.Length; k++)
                    {
                        string temDir = $"{parentDir}\\{mCompilerName}\\{mLibDirName}-{version}\\{mEnFramePlate[i]}\\{mDRs[j]}\\{mLibs[k]}";
                        if (!Directory.Exists(temDir))
                        {
                            Directory.CreateDirectory(temDir);
                        }
                    }
                }
            }
            return true;
        }
        public static bool MakeDllDirPaths2(string mLibDirName, string mCompilerName, string version, string parentDir)
        {
            if (mLibDirName.Length<1 || mCompilerName.Length<1 || parentDir.Length<1) return false;
            string[] mEnFramePlate = new string[2] { "x32", "x64" };
            string[] mDRs = new string[2] { "Debug", "Release" };
            string[] mLibs = new string[3] { "include", "lib", "dll" };
            for (int i = 0; i < mEnFramePlate.Length; i++)
            {
                for (int j = 0; j<mDRs.Length; j++)
                {
                    for (int k = 0; k<mLibs.Length; k++)
                    {
                        string temDir = "";
                        if (mLibs[k].ToUpper().Contains("INCLUDE"))
                        {
                            temDir = $"{parentDir}\\{mCompilerName}\\{mLibDirName}-{version}\\{mEnFramePlate[i]}\\{mLibs[k]}";
                        }
                        else
                        {
                            temDir = $"{parentDir}\\{mCompilerName}\\{mLibDirName}-{version}\\{mEnFramePlate[i]}\\{mLibs[k]}\\{mDRs[j]}";
                        }
                        if (!Directory.Exists(temDir))
                        {
                            Directory.CreateDirectory(temDir);
                        }
                    }
                }
            }
            return true;
        }
    }
}
