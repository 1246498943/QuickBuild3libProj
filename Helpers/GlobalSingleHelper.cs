using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPloteQuickBuidProj
{

    /// <summary>
    /// 全局信号关联类
    /// </summary>
    public class GlobalSingleHelper
    {
        /// <summary>
        /// 发送某个对象的一些全局信号对象.
        /// </summary>
        public static event Action<bool?>? SendCheckHandler;

        public static void SendCheckStatus(bool? model)
        {
            if(SendCheckHandler!=null)
            {
                SendCheckHandler(model);
            }
        }

        public static event Action<string?>? SendLogInfoHandler;
        public static void SendLogInfo(string? str)
        {
            if(SendLogInfoHandler!=null)
            {
                SendLogInfoHandler(str);
            }
        }
    }
}
