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
        public static event Action<sdkModelItem>? SendCheckHandler;

        public static void SendCheckStatus(sdkModelItem model)
        {
            if(SendCheckHandler!=null)
            {
                SendCheckHandler(model);
            }
        }

    }
}
