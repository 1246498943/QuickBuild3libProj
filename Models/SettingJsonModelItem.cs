using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XPloteQuickBuidProj
{

    public class SettingJsonModelItem:ObservableObject
    {

		/// <summary>
		/// 文件名称.
		/// </summary>
		private string mFileName;
		public string gFileName
		{
			get => mFileName;
			set
			{
				if (mFileName != value)
				{
					mFileName = value;
					this.OnPropertyChanging();
				}
			}
		}

		/// <summary>
		/// 文件全路径.
		/// </summary>
		private string mFilePath;
		public string gFilePath
		{
			get => mFilePath;
			set
			{
				if (mFilePath != value)
				{
					mFilePath = value;
					this.OnPropertyChanging();
				}
			}
		}

		public ObservableCollection<sdkModelItem> gSdkModelLists { get; set; } = new ObservableCollection<sdkModelItem>();


	}

}
