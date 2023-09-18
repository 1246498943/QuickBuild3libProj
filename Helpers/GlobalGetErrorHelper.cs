using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPloteQuickBuidProj
{
    public static class GlobalGetErrorHelper
    {
        public static void GetError(Action action,string str = "")
        {
            try
            {
                action();
                GlobalSingleHelper.SendLogInfo($"成功: {str}");
            }
            catch(Exception ex) 
            {
                GlobalSingleHelper.SendLogInfo($"异常: {ex.Message}");
            
            }
        }
    }
}
