namespace Steamworks
{
	public static class SteamUnifiedMessages
	{
		public static ClientUnifiedMessageHandle SendMethod(string pchServiceMethod, byte[] pRequestBuffer, uint unRequestBufferSize, ulong unContext)
		{
			InteropHelp.TestIfAvailableClient();
			using (InteropHelp.UTF8StringHandle pchServiceMethod2 = new InteropHelp.UTF8StringHandle(pchServiceMethod))
			{
				return (ClientUnifiedMessageHandle)NativeMethods.ISteamUnifiedMessages_SendMethod(pchServiceMethod2, pRequestBuffer, unRequestBufferSize, unContext);
			}
		}

		public static bool GetMethodResponseInfo(ClientUnifiedMessageHandle hHandle, out uint punResponseSize, out EResult peResult)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUnifiedMessages_GetMethodResponseInfo(hHandle, out punResponseSize, out peResult);
		}

		public static bool GetMethodResponseData(ClientUnifiedMessageHandle hHandle, byte[] pResponseBuffer, uint unResponseBufferSize, bool bAutoRelease)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUnifiedMessages_GetMethodResponseData(hHandle, pResponseBuffer, unResponseBufferSize, bAutoRelease);
		}

		public static bool ReleaseMethod(ClientUnifiedMessageHandle hHandle)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUnifiedMessages_ReleaseMethod(hHandle);
		}

		public static bool SendNotification(string pchServiceNotification, byte[] pNotificationBuffer, uint unNotificationBufferSize)
		{
			InteropHelp.TestIfAvailableClient();
			using (InteropHelp.UTF8StringHandle pchServiceNotification2 = new InteropHelp.UTF8StringHandle(pchServiceNotification))
			{
				return NativeMethods.ISteamUnifiedMessages_SendNotification(pchServiceNotification2, pNotificationBuffer, unNotificationBufferSize);
			}
		}
	}
}
