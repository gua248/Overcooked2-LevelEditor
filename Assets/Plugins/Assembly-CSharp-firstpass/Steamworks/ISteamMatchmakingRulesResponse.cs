using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public class ISteamMatchmakingRulesResponse
	{
		public delegate void RulesResponded(string pchRule, string pchValue);

		public delegate void RulesFailedToRespond();

		public delegate void RulesRefreshComplete();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void InternalRulesResponded(IntPtr pchRule, IntPtr pchValue);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void InternalRulesFailedToRespond();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void InternalRulesRefreshComplete();

		[StructLayout(LayoutKind.Sequential)]
		private class VTable
		{
			[NonSerialized]
			[MarshalAs(UnmanagedType.FunctionPtr)]
			public InternalRulesResponded m_VTRulesResponded;

			[NonSerialized]
			[MarshalAs(UnmanagedType.FunctionPtr)]
			public InternalRulesFailedToRespond m_VTRulesFailedToRespond;

			[NonSerialized]
			[MarshalAs(UnmanagedType.FunctionPtr)]
			public InternalRulesRefreshComplete m_VTRulesRefreshComplete;
		}

		private VTable m_VTable;

		private IntPtr m_pVTable;

		private GCHandle m_pGCHandle;

		private RulesResponded m_RulesResponded;

		private RulesFailedToRespond m_RulesFailedToRespond;

		private RulesRefreshComplete m_RulesRefreshComplete;

		public ISteamMatchmakingRulesResponse(RulesResponded onRulesResponded, RulesFailedToRespond onRulesFailedToRespond, RulesRefreshComplete onRulesRefreshComplete)
		{
			if (onRulesResponded == null || onRulesFailedToRespond == null || onRulesRefreshComplete == null)
			{
				throw new ArgumentNullException();
			}
			m_RulesResponded = onRulesResponded;
			m_RulesFailedToRespond = onRulesFailedToRespond;
			m_RulesRefreshComplete = onRulesRefreshComplete;
			m_VTable = new VTable
			{
				m_VTRulesResponded = InternalOnRulesResponded,
				m_VTRulesFailedToRespond = InternalOnRulesFailedToRespond,
				m_VTRulesRefreshComplete = InternalOnRulesRefreshComplete
			};
			m_pVTable = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VTable)));
			Marshal.StructureToPtr(m_VTable, m_pVTable, false);
			m_pGCHandle = GCHandle.Alloc(m_pVTable, GCHandleType.Pinned);
		}

		~ISteamMatchmakingRulesResponse()
		{
			if (m_pVTable != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(m_pVTable);
			}
			if (m_pGCHandle.IsAllocated)
			{
				m_pGCHandle.Free();
			}
		}

		private void InternalOnRulesResponded(IntPtr pchRule, IntPtr pchValue)
		{
			m_RulesResponded(InteropHelp.PtrToStringUTF8(pchRule), InteropHelp.PtrToStringUTF8(pchValue));
		}

		private void InternalOnRulesFailedToRespond()
		{
			m_RulesFailedToRespond();
		}

		private void InternalOnRulesRefreshComplete()
		{
			m_RulesRefreshComplete();
		}

		public static explicit operator IntPtr(ISteamMatchmakingRulesResponse that)
		{
			return that.m_pGCHandle.AddrOfPinnedObject();
		}
	}
}
