using System;
using System.Collections.Generic;
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
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace XPloteQuickBuidProj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class XPloteMainWidow : Window
    {
        public XPloteMainWidow()
        {
            InitializeComponent();
            //TestGroupBox();
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

        #region 测试分组

#if false
        public class ModelFile
        {
            public int Num { get; set; }
            public bool IsChecked { get; set; }
            public string FilterCond { get; set; }
            public double FilterValue { get; set; }
            public string FilterCondValue
            {
                get
                {
                    return $"{FilterCond}:{FilterValue}";
                }
            }
        }
        public ObservableCollection<ModelFile> CollectionModelFile = new ObservableCollection<ModelFile>();

        private void TestGroupBox()
        {
            //CollectionModelFile.Add(new ModelFile() { Num = 0, IsChecked=true, FilterCond="Width", FilterValue=512 });
            //CollectionModelFile.Add(new ModelFile() { Num = 0, IsChecked=true, FilterCond="Height", FilterValue=7 });
            //CollectionModelFile.Add(new ModelFile() { Num = 0, IsChecked=true, FilterCond="Length", FilterValue=3 });

            //CollectionModelFile.Add(new ModelFile() { Num = 1, IsChecked=true, FilterCond="Width", FilterValue=50 });
            //CollectionModelFile.Add(new ModelFile() { Num = 2, IsChecked=true, FilterCond="height", FilterValue=50 });

            CollectionModelFile.Add(new ModelFile() { Num = 1, IsChecked = true, FilterCond = "Width", FilterValue = 50 });
            CollectionModelFile.Add(new ModelFile() { Num = 2, IsChecked = true, FilterCond = "height", FilterValue = 50 });

            lbMain.ItemsSource = CollectionModelFile;
            ICollectionView cv = CollectionViewSource.GetDefaultView(lbMain.ItemsSource);
            cv.GroupDescriptions.Add(new PropertyGroupDescription("Num"));
            cv.GroupDescriptions.Add(new PropertyGroupDescription("FilterValue"));
        } 

            ////xml样式版本.
           //<ListBox Name = "lbMain" HorizontalContentAlignment="Stretch">
           //                     <ListBox.ItemTemplate>
           //                         <DataTemplate>
           //                             <TextBlock Text = "{Binding FilterCond}" Margin="20,0,0,0"/> </DataTemplate>
           //                     </ListBox.ItemTemplate>
           //                     <ListBox.GroupStyle>
           //                         <GroupStyle>
           //                             <GroupStyle.ContainerStyle>
           //                                 <Style TargetType = "{x:Type GroupItem}">
           //                                     <Setter Property="Template">
           //                                         <Setter.Value>
           //                                             <ControlTemplate TargetType = "{x:Type GroupItem}">
           //                                                 <Expander IsExpanded="True" ExpandDirection="Down" Margin="25,0">
           //                                                     <Expander.Header>
           //                                                         <StackPanel Orientation = "Horizontal">
           //                                                             <TextBlock  Text="{Binding Path=Name}" VerticalAlignment="Center" /> <TextBlock Text = "{Binding Path=ItemCount, StringFormat=数量: {0}}" VerticalAlignment="Center" Margin="5,0,0,0" /> </StackPanel> </Expander.Header> <ItemsPresenter /> </Expander>
           //                                             </ControlTemplate>
           //                                         </Setter.Value>
           //                                     </Setter>
           //                                 </Style>
           //                             </GroupStyle.ContainerStyle>
           //                         </GroupStyle>
           //                     </ListBox.GroupStyle>
           //                 </ListBox>

#endif

        #endregion

    }
}
