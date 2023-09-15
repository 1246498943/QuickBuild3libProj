using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;

namespace XPloteQuickBuidProj
{
   
	public class PathDllItem:ObservableObject
	{

		public PathDllItem(string mName)
		{
			gItemName = mName;

        }

        private string? mItemName;
		public string? gItemName
		{
			get => mItemName;
			set
			{
				if (mItemName != value)
				{
					mItemName = value;
					this.OnPropertyChanged();
				}
			}
		}

	}


    /// <summary>
    /// 而整个模块,却是包含一些具体的文件内容.
    /// </summary>
    public class dllItem : PathDllItem
    {

		public dllItem(string mName):base(mName)
		{

		}
        /// <summary>
        /// 包含编译的内容各种 opencv.lib  pcl.lib等等内容....
        /// </summary>
        public ObservableCollection<string> gLibSources { get; set; } = new ObservableCollection<string>();

    }

	public class DebugReleaseDllItem: PathDllItem
    {
		public DebugReleaseDllItem(string mName):base(mName)
		{
			mIncludeDir = new dllItem("include");
			mLibDir = new dllItem("lib");
			mDllDir = new dllItem("dll");
        }


		private dllItem mIncludeDir;
		public dllItem gIncludeDir
		{
			get => mIncludeDir;
			set
			{
				if (mIncludeDir != value)
				{
					mIncludeDir = value;
					this.OnPropertyChanged();
				}
			}
		}


		private dllItem mLibDir;
		public dllItem gLibDir
		{
			get => mLibDir;
			set
			{
				if (mLibDir != value)
				{
					mLibDir = value;
					this.OnPropertyChanged();
				}
			}
		}


		private dllItem mDllDir;
		public dllItem gDllDir
		{
			get => mDllDir;
			set
			{
				if (mDllDir != value)
				{
					mDllDir = value;
					this.OnPropertyChanged();
				}
			}
		}


        /// <summary>
        /// 默认是选中的...
        /// </summary>
        private bool? mIsChecked = true;
        public bool? gIsChecked
        {
            get => mIsChecked;
            set
            {
                if (mIsChecked != value)
                {
                    mIsChecked = value;
                    this.OnPropertyChanging();
                }
            }
        }



    }

	public class X64Or32DllItem: PathDllItem
    {
		public X64Or32DllItem(string mName):base(mName) 
        {
            mDebug = new DebugReleaseDllItem("Debug");
            mRelease = new DebugReleaseDllItem("Release");
        }


        private DebugReleaseDllItem mDebug;
        public DebugReleaseDllItem gDebug
        {
            get => mDebug;
            set
            {
                if (mDebug != value)
                {
                    mDebug = value;
                    this.OnPropertyChanging();
                }
            }
        }


        private DebugReleaseDllItem mRelease;
		public DebugReleaseDllItem gRelease
		{
			get => mRelease;
			set
			{
				if (mRelease != value)
				{
					mRelease = value;
					this.OnPropertyChanged();
				}
			}
		}


        private bool? mIsChecked = true;
        public bool? gIsChecked
        {
            get => mIsChecked;
            set
            {
                if (mIsChecked != value)
                {
                    mIsChecked = value;
                    this.OnPropertyChanging();
                }
            }
        }


    }

    /*


    */
    public class sdkModelItem : PathDllItem
    {
        public sdkModelItem(string mName):base(mName)
        {
            mDll32 = new X64Or32DllItem("x32");
            mDll64 = new X64Or32DllItem("x64");
        }

        /// <summary>
        /// 库的版本
        /// </summary>
        private string? mDllVersion;
        public string? gDllVersion
        {
            get => mDllVersion;
            set
            {
                if (mDllVersion != value)
                {
                    mDllVersion = value;
                    this.OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// 是否选中.
        /// </summary>
        private bool? mIsChecked;
        public bool? gIsChecked
        {
            get => mIsChecked;
            set
            {
                if (mIsChecked != value)
                {
                    mIsChecked = value;
                    this.OnPropertyChanged();
                    GlobalSingleHelper.SendCheckStatus(this);

                }
            }
        }

        private X64Or32DllItem? mDll32;
        public X64Or32DllItem? gDll32
        {
            get => mDll32;
            set
            {
                if (mDll32 != value)
                {
                    mDll32 = value;
                    this.OnPropertyChanged();
                }
            }
        }




        private X64Or32DllItem? mDll64;
        public X64Or32DllItem? gDll64
        {
            get => mDll64;
            set
            {
                if (mDll64 != value)
                {
                    mDll64 = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private string? mDescript;
        public string? gDescript
        {
            get => mDescript;
            set
            {
                if (mDescript != value)
                {
                    mDescript = value;
                    this.OnPropertyChanged();
                }
            }
        }
    }

    public class CompelierItem: PathDllItem
    {
        public CompelierItem(string mName):base(mName) 
        {
            gSdkItemList = new ObservableCollection<sdkModelItem>();
        }  
        public ObservableCollection<sdkModelItem> gSdkItemList { get; set; }
    }


}
