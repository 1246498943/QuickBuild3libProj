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
    /// ShowContent.xaml 的交互逻辑
    /// </summary>
    public partial class ShowContent : Window
    {
        public ShowContent()
        {
            InitializeComponent();
        }

        public void ShowWnd()
        {
            this.Visibility = Visibility.Visible;
            this.Activate();
            this.Show();
        }

        public void SetContentStr(string str)
        {
            FlowDocumentName.Text = str;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.Visibility = Visibility.Collapsed;
            e.Cancel = true;
        }
    }
}
