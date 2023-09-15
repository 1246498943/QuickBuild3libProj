using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessageBox = System.Windows.MessageBox;

namespace XPloteQuickBuidProj
{
    public class sdkCreateDirModel:ObservableObject
    {

		public sdkCreateDirModel()
		{
			gCreateStructDir = new RelayCommand(() => {

				var status = false;
                switch (gSetType)
				{
					case 0:
						{
                            status= SdkDirHelper.MakeDllDirPaths1(gDllName, gCompiler, gVersion, gSdkDir);
                            break;
						}
					case 1:
						{
                            status= SdkDirHelper.MakeDllDirPaths2(gDllName, gCompiler, gVersion, gSdkDir);
                            break;
						}
					default:
						break;
				}
				
				if(status==false)
				{
					MessageBox.Show("创建失败,请检测是否填写了库名字或者编译器号");
				}

			});

            gSetSdkDir = new RelayCommand(() =>
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                folderDialog.Description = "选择文件夹";
                if (folderDialog.ShowDialog()== DialogResult.OK)
                {
                    gSdkDir  =   folderDialog.SelectedPath;
                }
            });


        }

        public ICommand gCreateStructDir { get; set; }

        public ICommand gSetSdkDir { get; set; }
       
		private string mDllName="";
		public string gDllName
		{
			get => mDllName;
			set
			{
				if (mDllName != value)
				{
					mDllName = value;
					this.OnPropertyChanged();
				}
			}
		}


		private int mSetType = 0;
		public int gSetType
		{
			get => mSetType;
			set
			{
				if (mSetType != value)
				{
					mSetType = value;
					this.OnPropertyChanged();
				}
			}
		}



		private string mVersion = "0.0.0.1";
		public string gVersion
		{
			get => mVersion;
			set
			{
				if (mVersion != value)
				{
					mVersion = value;
					this.OnPropertyChanged();
				}
			}
		}



		/// <summary>
		/// 编译器名字.
		/// </summary>
		private string mCompiler = "vs2019";
		public string gCompiler
		{
			get => mCompiler;
			set
			{
				if (mCompiler != value)
				{
					mCompiler = value;
					this.OnPropertyChanged();
				}
			}
		}


		private string mSDkDir;
		public string gSdkDir
		{
			get => mSDkDir;
			set
			{
				if (mSDkDir != value)
				{
					mSDkDir = value;
					this.OnPropertyChanged();
				}
			}
		}

	}
}
