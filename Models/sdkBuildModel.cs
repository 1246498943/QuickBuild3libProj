using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
namespace XPloteQuickBuidProj
{
    public class sdkBuildModel:ObservableObject
    {
		public sdkBuildModel()
		{
		}

        private string? mSdkDir;
		public string? gSdkDir
		{
			get => mSdkDir;
			set
			{
				if (mSdkDir != value)
				{
					mSdkDir = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// 配置文件名字.
		/// </summary>
		private string? mSettingName;
		public string? gSettingName
		{
			get => mSettingName;
			set
			{
				if (mSettingName != value)
				{
					mSettingName = value;
					this.OnPropertyChanged();
				}
			}
		}



		private string? mSettingPath;
		/// <summary>
		/// 配置文件夹路径
		/// </summary>
		public string? gSettingPath
		{
			get => mSettingPath;
			set
			{
				if (mSettingPath != value)
				{
					mSettingPath = value;
					this.OnPropertyChanged();
				}
			}
		}


		public ObservableCollection<CompelierItem>? gSdkSource { get; set; } = new ObservableCollection<CompelierItem>();
		public ObservableCollection<sdkModelItem>? gBuildSdkSource { get; set; } = new ObservableCollection<sdkModelItem>();


	}
}
