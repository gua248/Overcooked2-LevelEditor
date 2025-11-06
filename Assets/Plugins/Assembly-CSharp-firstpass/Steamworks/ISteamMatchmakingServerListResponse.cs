using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public class ISteamMatchmakingServerListResponse
	{
		public delegate void ServerResponded(HServerListRequest hRequest, int iServer);

		public delegate void ServerFailedToRespond(HServerListRequest hRequest, int iServer);

		public delegate void RefreshComplete(HServerListRequest hRequest, EMatchMakingServerResponse response);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void InternalServerResponded(HServerListRequest hRequest, int iServer);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void InternalServerFailedToRespond(HServerListRequest hRequest, int iServer);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void InternalRefreshComplete(HServerListRequest hRequest, EMatchMakingServerResponse response);

		[StructLayout(LayoutKind.Sequential)]
		private class VTable
		{
			[NonSerialized]
			[MarshalAs(UnmanagedType.FunctionPtr)]
			public InternalServerResponded m_VTServerResponded;

			[NonSerialized]
			[MarshalAs(UnmanagedType.FunctionPtr)]
			public InternalServerFailedToRespond m_VTServerFailedToRespond;

			[NonSerialized]
			[MarshalAs(UnmanagedType.FunctionPtr)]
			public InternalRefreshComplete m_VTRefreshComplete;
		}

		private VTable m_VTable;

		private IntPtr m_pVTable;

		private GCHandle m_pGCHandle;

		private ServerResponded m_ServerResponded;

		private ServerFailedToRespond m_ServerFailedToRespond;

		private RefreshComplete m_RefreshComplete;

		public ISteamMatchmakingServerListResponse(ServerResponded onServerResponded, ServerFailedToRespond onServerFailedToRespond, RefreshComplete onRefreshComplete)
		{
			if (onServerResponded == null || onServerFailedToRespond == null || onRefreshComplete == null)
			{
				throw new ArgumentNullException();
			}
			m_ServerResponded = onServerResponded;
			m_ServerFailedToRespond = onServerFailedToRespond;
			m_RefreshComplete = onRefreshComplete;
			m_VTable = new VTable
			{
				m_VTServerResponded = InternalOnServerResponded,
				m_VTServerFailedToRespond = InternalOnServerFailedToRespond,
				m_VTRefreshComplete = InternalOnRefreshComplete
			};
			m_pVTable = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VTable)));
			Marshal.StructureToPtr(m_VTable, m_pVTable, false);
			m_pGCHandle = GCHandle.Alloc(m_pVTable, GCHandleType.Pinned);
		}

		~ISteamMatchmakingServerListResponse()
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

		private void InternalOnServerResponded(HServerListRequest hRequest, int iServer)
		{
			m_ServerResponded(hRequest, iServer);
		}

		private void InternalOnServerFailedToRespond(HServerListRequest hRequest, int iServer)
		{
			m_ServerFailedToRespond(hRequest, iServer);
		}

		private void InternalOnRefreshComplete(HServerListRequest hRequest, EMatchMakingServerResponse response)
		{
			m_RefreshComplete(hRequest, response);
		}

		public static explicit operator IntPtr(ISteamMatchmakingServerListResponse that)
		{
			return that.m_pGCHandle.AddrOfPinnedObject();
		}
	}
}
