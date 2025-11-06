namespace Steamworks
{
	public enum EAuthSessionResponse
	{
		k_EAuthSessionResponseOK = 0,
		k_EAuthSessionResponseUserNotConnectedToSteam = 1,
		k_EAuthSessionResponseNoLicenseOrExpired = 2,
		k_EAuthSessionResponseVACBanned = 3,
		k_EAuthSessionResponseLoggedInElseWhere = 4,
		k_EAuthSessionResponseVACCheckTimedOut = 5,
		k_EAuthSessionResponseAuthTicketCanceled = 6,
		k_EAuthSessionResponseAuthTicketInvalidAlreadyUsed = 7,
		k_EAuthSessionResponseAuthTicketInvalid = 8,
		k_EAuthSessionResponsePublisherIssuedBan = 9
	}
}
