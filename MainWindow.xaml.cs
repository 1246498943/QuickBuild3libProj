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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;
using ListBox = System.Windows.Controls.ListBox;
using UserControl = System.Windows.Controls.UserControl;

namespace XPloteQuickBuidProj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            XPloteQucikConfigs.InitSource();
            InitializeComponent();
            this.DataContext = this;
            this.Closed+=XPloteMainWidow_Closed;
            GlobalSingleHelper.SendLogInfoHandler+=GlobalSingleHelper_SendLogInfoHandler;
        }

        private void GlobalSingleHelper_SendLogInfoHandler(string? obj)
        {
            //LogName.Text =obj;
            ListLogBoxName.Items.Add($"{ListLogBoxName.Items.Count.ToString("D5")}:  {obj}");
            ListLogBoxName.ScrollIntoView(ListLogBoxName.Items[ListLogBoxName.Items.Count-1]);
        }

        private void XPloteMainWidow_Closed(object? sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        public SdkBuildViewModel gModel => GlobalVMHelper.gBuildViewModel;
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem? curMenu = sender as MenuItem;
            ContextMenu? curMeuParen = curMenu?.Parent as ContextMenu;
            ListBox curListbox = curMeuParen?.PlacementTarget as ListBox;
            if (curListbox!=null)
            {
                var usrCont = curListbox.SelectedItem as UserControl;
                if (usrCont!=null)
                {
                    var cur32or64Dll = usrCont.Content as  X64Or32DllItem;
                    if (cur32or64Dll!=null)
                    {
                        ShowDllContent.Content = cur32or64Dll;
                    }
                }

            }
        }


        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e == null) return;
            var mListBoxItem = VisualUpwardSearch<ListBoxItem>(e.OriginalSource as DependencyObject) as ListBoxItem;
            if (mListBoxItem != null)
            {
                mListBoxItem.Focus();
            }


        }
        private childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }


        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);
            return source;
        }

        private void MenuItem_Click_log(object sender, RoutedEventArgs e)
        {
            ListLogBoxName.Items.Clear();
        }
    }
}
