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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XPloteQuickBuidProj.Views
{
    /// <summary>
    /// CreateDir.xaml 的交互逻辑
    /// </summary>
    public partial class CreateDir : Window
    {
        public CreateDir()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        public sdkCreateDirModel gModel => GlobalVMHelper.gBuildViewModel.gCreateDirModel;
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

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton  rbt = sender as RadioButton;
            if(rbt!=null)
            {
                gModel.gSetType = int.Parse(rbt.Tag.ToString());
            }
        }
    }
}
