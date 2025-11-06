using System;

namespace Steamworks
{
	[Flags]
	public enum EAppType
	{
		k_EAppType_Invalid = 0,
		k_EAppType_Game = 1,
		k_EAppType_Application = 2,
		k_EAppType_Tool = 4,
		k_EAppType_Demo = 8,
		k_EAppType_Media_DEPRECATED = 0x10,
		k_EAppType_DLC = 0x20,
		k_EAppType_Guide = 0x40,
		k_EAppType_Driver = 0x80,
		k_EAppType_Config = 0x100,
		k_EAppType_Hardware = 0x200,
		k_EAppType_Franchise = 0x400,
		k_EAppType_Video = 0x800,
		k_EAppType_Plugin = 0x1000,
		k_EAppType_Music = 0x2000,
		k_EAppType_Series = 0x4000,
		k_EAppType_Shortcut = 0x40000000,
		k_EAppType_DepotOnly = -2147483647
	}
}
