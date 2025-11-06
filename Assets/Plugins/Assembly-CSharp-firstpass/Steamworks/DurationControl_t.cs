using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	[CallbackIdentity(167)]
	public struct DurationControl_t
	{
		public const int k_iCallback = 167;

		public EResult m_eResult;

		public AppId_t m_appid;

		public bool m_bApplicable;

		public int m_csecsLast5h;

		public EDurationControlProgress m_progress;

		public EDurationControlNotification m_notification;
	}
}
