using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void SteamAPI_PostAPIResultInProcess_t(SteamAPICall_t callHandle, IntPtr pUnknown, uint unCallbackSize, int iCallbackNum);
}
