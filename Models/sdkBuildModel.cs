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
			mSettingPath = $"{XPloteQucikConfigs.CurSettingDir}";
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
		private string? mSettingName = "XPloteSettingConfig";
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


		private string mVcxprojFile;	
		public string gVcxprojFile
		{
			get => mVcxprojFile;
			set
			{
				if (mVcxprojFile != value)
				{
					mVcxprojFile = value;
					this.OnPropertyChanging();
				}
			}
		}


		/// <summary>
		/// 覆盖原始内容.
		/// </summary>
		private bool mIsCover;
		public bool gIsCover
		{
			get => mIsCover;
			set
			{
				if (mIsCover != value)
				{
					mIsCover = value;
					this.OnPropertyChanging();
				}
			}
		}



		public CompelierItem? gCurCompelier { get; set; }

		/// <summary>
		/// 编译器下的选项.
		/// </summary>
        public ObservableCollection<CompelierItem>? gCompelierSource { get; set; } = new ObservableCollection<CompelierItem>();

		/// <summary>
		/// 选中之后的库数据
		/// </summary>
		public ObservableCollection<sdkModelItem>? gBuildSdkSource { get; set; } = new ObservableCollection<sdkModelItem>();
		private Dictionary<string,sdkModelItem> mBuildKeyMap = new Dictionary<string,sdkModelItem>();
		public void AddSdkModelItem(sdkModelItem item)
		{
			if (item==null) return;
			var curKey = item.gItemName;
            if (!mBuildKeyMap.ContainsKey(curKey))
			{
				mBuildKeyMap.Add(curKey,item);
				gBuildSdkSource.Add(item);
            }
		}
		public void ClearnSkModelItems()
		{
			gBuildSdkSource.Clear();
			mBuildKeyMap.Clear();
        }

		public void RemoveSdkModelItems(string sdkItemName)
		{
			if(mBuildKeyMap.ContainsKey(sdkItemName))
			{
                mBuildKeyMap.Remove(sdkItemName);
				var curItem = mBuildKeyMap[sdkItemName];
				if (curItem!=null) gBuildSdkSource.Remove(curItem);
            }

        }


	}
}
