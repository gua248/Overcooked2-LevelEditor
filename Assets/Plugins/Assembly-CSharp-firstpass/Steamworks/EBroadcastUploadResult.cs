namespace Steamworks
{
	public enum EBroadcastUploadResult
	{
		k_EBroadcastUploadResultNone = 0,
		k_EBroadcastUploadResultOK = 1,
		k_EBroadcastUploadResultInitFailed = 2,
		k_EBroadcastUploadResultFrameFailed = 3,
		k_EBroadcastUploadResultTimeout = 4,
		k_EBroadcastUploadResultBandwidthExceeded = 5,
		k_EBroadcastUploadResultLowFPS = 6,
		k_EBroadcastUploadResultMissingKeyFrames = 7,
		k_EBroadcastUploadResultNoConnection = 8,
		k_EBroadcastUploadResultRelayFailed = 9,
		k_EBroadcastUploadResultSettingsChanged = 10,
		k_EBroadcastUploadResultMissingAudio = 11,
		k_EBroadcastUploadResultTooFarBehind = 12,
		k_EBroadcastUploadResultTranscodeBehind = 13
	}
}
