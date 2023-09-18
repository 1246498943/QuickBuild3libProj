using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPloteQuickBuidProj
{
    /// <summary>
    /// 项目配置.
    /// </summary>
    public static class XPloteQucikConfigs
    {
        public static string CurExeDir => AppDomain.CurrentDomain.BaseDirectory;    //程序执行文件夹.
        public static string CurSettingDir => $"{CurExeDir}\\QuickSettingConfigs\\";  //快速构建的配置文件夹/方便读取现有配置项目.

        public static void InitSource()
        {
            Directory.CreateDirectory(CurSettingDir);
        }
    }
}
