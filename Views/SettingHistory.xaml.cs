using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XPloteQuickBuidProj
{
    /// <summary>
    /// SettingHistory.xaml 的交互逻辑
    /// 配置文件的历史操作目录...
    /// </summary>
    public partial class SettingHistory : Window
    {
        public SettingHistory()
        {
            InitializeComponent();
            gModel = GlobalVMHelper.gBuildViewModel;
            this.DataContext = this;
            gModel.IReUpdate.Execute(null);
        }
        public SdkBuildViewModel gModel { get; set; }

        public void ShowWnd()
        {
            this.Visibility = Visibility.Visible;
            this.Activate();
            this.Show();
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.Visibility = Visibility.Collapsed;
            e.Cancel = true;
        }
    }
}
