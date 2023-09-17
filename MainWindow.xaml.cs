using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace XPloteQuickBuidProj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Closed+=XPloteMainWidow_Closed;
        }


        private void XPloteMainWidow_Closed(object? sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        public SdkBuildViewModel gModel => GlobalVMHelper.gBuildViewModel;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
