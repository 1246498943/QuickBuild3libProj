using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using Newtonsoft.Json;

namespace XPloteQuickBuidProj
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
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

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
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

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class DebugReleaseDllItem: PathDllItem
    {
		public DebugReleaseDllItem(string mName):base(mName)
		{
			mIncludeDir = new dllItem("include");
			mLibDir = new dllItem("lib");
			mDllDir = new dllItem("dll");
        }


        public string LibString()
        {
            return $"{gItemName}\\{mLibDir.gItemName}";
        }
        public string DllString()
        {
            return $"{gItemName}\\{mDllDir.gItemName}";
        }
        public string IncludeString()
        {
            return $"{gItemName}\\{mIncludeDir.gItemName}";
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
                    this.OnPropertyChanged();
                    GlobalSingleHelper.SendCheckStatus(mIsChecked);
                }
            }
        }



    }

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class X64Or32DllItem: PathDllItem
    {
		public X64Or32DllItem(string mName):base(mName) 
        {
            mDebug = new DebugReleaseDllItem("Debug");
            mRelease = new DebugReleaseDllItem("Release");
        }

        public string Platform_Debug_Lib_String()
        {
            return $"{gItemName}\\{mDebug.LibString()}";
        }
        public string Platform_Debug_Dll_String()
        {
            return $"{gItemName}\\{mDebug.DllString()}";
        }
        public string Platform_Debug_Include_String()
        {
            return $"{gItemName}\\{mDebug.IncludeString()}";
        }

        public string Platform_Release_Lib_String()
        {
            return $"{gItemName}\\{mRelease.LibString()}";
        }
        public string Platform_Release_Dll_String()
        {
            return $"{gItemName}\\{mRelease.DllString()}";
        }
        public string Platform_Release_Include_String()
        {
            return $"{gItemName}\\{mRelease.IncludeString()}";
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
                    this.OnPropertyChanged();
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
                    this.OnPropertyChanged();
                    GlobalSingleHelper.SendCheckStatus(mIsChecked);
                }
            }
        }


    }

    /*


    */
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class sdkModelItem : PathDllItem
    {


        public sdkModelItem(string mName):base(mName)
        {
            mDll32 = new X64Or32DllItem("x32");
            mDll32.gIsChecked = false;
            mDll64 = new X64Or32DllItem("x64");
        }

        public string DllName_64_Debug_Lib_String()
        {
            return $"{gItemName}\\{mDll64.Platform_Debug_Lib_String()}";
        }
        public string DllName_64_Debug_Dll_String()
        {
            return $"{gItemName}\\{mDll64.Platform_Debug_Dll_String()}";
        }
        public string DllName_64_Debug_Include_String()
        {
            return $"{gItemName}\\{mDll64.Platform_Debug_Include_String()}";
        }

        public string DllName_64_Release_Lib_String()
        {
            return $"{gItemName}\\{mDll64.Platform_Release_Lib_String()}";
        }
        public string DllName_64_Release_Dll_String()
        {
            return $"{gItemName}\\{mDll64.Platform_Release_Dll_String()}";
        }
        public string DllName_64_Release_Include_String()
        {
            return $"{gItemName}\\{mDll64.Platform_Release_Include_String()}";
        }

        public string DllName_32_Debug_Lib_String()
        {
            return $"{gItemName}\\{mDll32.Platform_Debug_Lib_String()}";
        }
        public string DllName_32_Debug_Dll_String()
        {
            return $"{gItemName}\\{mDll32.Platform_Debug_Dll_String()}";
        }
        public string DllName_32_Debug_Include_String()
        {
            return $"{gItemName}\\{mDll32.Platform_Debug_Include_String()}";
        }

        public string DllName_32_Release_Lib_String()
        {
            return $"{gItemName}\\{mDll32.Platform_Release_Lib_String()}";
        }
        public string DllName_32_Release_Dll_String()
        {
            return $"{gItemName}\\{mDll32.Platform_Release_Dll_String()}";
        }
        public string DllName_32_Release_Include_String()
        {
            return $"{gItemName}\\{mDll32.Platform_Release_Include_String()}";
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
        private bool? mIsChecked = false;
        public bool? gIsChecked
        {
            get => mIsChecked;
            set
            {
                if (mIsChecked != value)
                {
                    mIsChecked = value;
                    this.OnPropertyChanged();
                    GlobalSingleHelper.SendCheckStatus(mIsChecked);

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

    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class CompelierItem: PathDllItem
    {
        public CompelierItem(string mName):base(mName) 
        {
            gSdkItemList = new ObservableCollection<sdkModelItem>();
        }
        public sdkModelItem gCurSdkModelItem { get; set; }
        public ObservableCollection<sdkModelItem> gSdkItemList { get; set; }


    }



}
