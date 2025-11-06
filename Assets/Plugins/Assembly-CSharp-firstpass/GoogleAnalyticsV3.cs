using UnityEngine;

public class GoogleAnalyticsV3 : MonoBehaviour
{
	public delegate string GetUserData();

	public enum DebugMode
	{
		ERROR = 0,
		WARNING = 1,
		INFO = 2,
		VERBOSE = 3
	}

	private string uncaughtExceptionStackTrace;

	private string uncaughtExceptionUserData;

	private bool initialized;

	public GetUserData getUserData;

	[Tooltip("The tracking code to be used for Android. Example value: UA-XXXX-Y.")]
	public string androidTrackingCode;

	[Tooltip("The tracking code to be used for iOS. Example value: UA-XXXX-Y.")]
	public string IOSTrackingCode;

	[Tooltip("The tracking code to be used for platforms other than Android and iOS. Example value: UA-XXXX-Y.")]
	public string otherTrackingCode;

	[Tooltip("The application name. This value should be modified in the Unity Player Settings.")]
	public string productName;

	[Tooltip("The application identifier. Example value: com.company.app.")]
	public string bundleIdentifier;

	[Tooltip("The application version. Example value: 1.2")]
	public string bundleVersion;

	[RangedTooltip("The dispatch period in seconds. Only required for Android and iOS.", 0f, 3600f)]
	public int dispatchPeriod = 5;

	[RangedTooltip("The sample rate to use. Only required for Android and iOS.", 0f, 100f)]
	public int sampleFrequency = 100;

	[Tooltip("The log level. Default is WARNING.")]
	public DebugMode logLevel = DebugMode.WARNING;

	[Tooltip("If checked, the IP address of the sender will be anonymized.")]
	public bool anonymizeIP;

	[Tooltip("Automatically report uncaught exceptions.")]
	public bool UncaughtExceptionReporting;

	[Tooltip("Automatically send a launch event when the game starts up.")]
	public bool sendLaunchEvent;

	[Tooltip("If checked, hits will not be dispatched. Use for testing.")]
	public bool dryRun;

	[Tooltip("The amount of time in seconds your application can stay inthe background before the session is ended. Default is 30 minutes (1800 seconds). A value of -1 will disable session management.")]
	public int sessionTimeout = 1800;

	public static GoogleAnalyticsV3 instance;

	[HideInInspector]
	public static readonly string currencySymbol = "USD";

	public static readonly string EVENT_HIT = "createEvent";

	public static readonly string APP_VIEW = "createAppView";

	public static readonly string SET = "set";

	public static readonly string SET_ALL = "setAll";

	public static readonly string SEND = "send";

	public static readonly string ITEM_HIT = "createItem";

	public static readonly string TRANSACTION_HIT = "createTransaction";

	public static readonly string SOCIAL_HIT = "createSocial";

	public static readonly string TIMING_HIT = "createTiming";

	public static readonly string EXCEPTION_HIT = "createException";

	private GoogleAnalyticsMPV3 mpTracker = new GoogleAnalyticsMPV3();

	public void Initialize()
	{
		InitializeTracker();
		if (sendLaunchEvent)
		{
			Debug.Log("GoogleAnalytics - Initialize");
			LogEvent("Google Analytics", "Auto Instrumentation", "Game Launch", 0L);
		}
		if (UncaughtExceptionReporting)
		{
			Application.RegisterLogCallback(HandleException);
			if (belowThreshold(logLevel, DebugMode.VERBOSE))
			{
				Debug.Log("Enabling uncaught exception reporting.");
			}
		}
	}

	private void Update()
	{
		if (uncaughtExceptionStackTrace != null)
		{
			Debug.LogWarning("GoogleAnalytics - Exception about to be processed.");
			LogException(uncaughtExceptionStackTrace, true, uncaughtExceptionUserData);
			uncaughtExceptionStackTrace = null;
		}
	}

	private void HandleException(string condition, string stackTrace, LogType type)
	{
		if (type == LogType.Exception)
		{
			uncaughtExceptionStackTrace = condition + "\n" + stackTrace + StackTraceUtility.ExtractStackTrace();
			uncaughtExceptionUserData = string.Empty;
			if (getUserData != null)
			{
				uncaughtExceptionUserData = getUserData();
			}
		}
	}

	private void InitializeTracker()
	{
		if (!initialized)
		{
			instance = this;
			Object.DontDestroyOnLoad(instance);
			Debug.Log("Initializing Google Analytics 0.1.");
			mpTracker.SetTrackingCode(otherTrackingCode);
			mpTracker.SetBundleIdentifier(bundleIdentifier);
			mpTracker.SetAppName(productName);
			mpTracker.SetAppVersion(bundleVersion);
			mpTracker.SetLogLevelValue(logLevel);
			mpTracker.SetAnonymizeIP(anonymizeIP);
			mpTracker.SetDryRun(dryRun);
			mpTracker.InitializeTracker();
			initialized = true;
			SetOnTracker(Fields.DEVELOPER_ID, "GbOCSs");
		}
	}

	public void SetAppLevelOptOut(bool optOut)
	{
		InitializeTracker();
		mpTracker.SetOptOut(optOut);
	}

	public void SetUserIDOverride(string userID)
	{
		SetOnTracker(Fields.USER_ID, userID);
	}

	public void ClearUserIDOverride()
	{
		InitializeTracker();
		mpTracker.ClearUserIDOverride();
	}

	public void DispatchHits()
	{
		InitializeTracker();
	}

	public void StartSession()
	{
		InitializeTracker();
		mpTracker.StartSession();
	}

	public void StopSession()
	{
		InitializeTracker();
		mpTracker.StopSession();
	}

	public void Abort()
	{
		uncaughtExceptionStackTrace = null;
		mpTracker.AbortHits();
	}

	public void SetOnTracker(Field fieldName, object value)
	{
		InitializeTracker();
		mpTracker.SetTrackerVal(fieldName, value);
	}

	public void LogScreen(string title)
	{
		AppViewHitBuilder builder = new AppViewHitBuilder().SetScreenName(title);
		LogScreen(builder);
	}

	public void LogScreen(AppViewHitBuilder builder)
	{
		InitializeTracker();
		if (builder.Validate() != null)
		{
			if (belowThreshold(logLevel, DebugMode.VERBOSE))
			{
				Debug.Log("Logging screen.");
			}
			mpTracker.LogScreen(builder);
		}
	}

	public void LogEvent(string eventCategory, string eventAction, string eventLabel, long value)
	{
		EventHitBuilder builder = new EventHitBuilder().SetEventCategory(eventCategory).SetEventAction(eventAction).SetEventLabel(eventLabel)
			.SetEventValue(value);
		LogEvent(builder);
	}

	public void LogEvent(EventHitBuilder builder)
	{
		InitializeTracker();
		if (builder.Validate() != null)
		{
			if (belowThreshold(logLevel, DebugMode.VERBOSE))
			{
				Debug.Log("Logging event.");
			}
			mpTracker.LogEvent(builder);
		}
	}

	public void LogTransaction(string transID, string affiliation, double revenue, double tax, double shipping)
	{
		LogTransaction(transID, affiliation, revenue, tax, shipping, string.Empty);
	}

	public void LogTransaction(string transID, string affiliation, double revenue, double tax, double shipping, string currencyCode)
	{
		TransactionHitBuilder builder = new TransactionHitBuilder().SetTransactionID(transID).SetAffiliation(affiliation).SetRevenue(revenue)
			.SetTax(tax)
			.SetShipping(shipping)
			.SetCurrencyCode(currencyCode);
		LogTransaction(builder);
	}

	public void LogTransaction(TransactionHitBuilder builder)
	{
		InitializeTracker();
		if (builder.Validate() != null)
		{
			if (belowThreshold(logLevel, DebugMode.VERBOSE))
			{
				Debug.Log("Logging transaction.");
			}
			mpTracker.LogTransaction(builder);
		}
	}

	public void LogItem(string transID, string name, string sku, string category, double price, long quantity)
	{
		LogItem(transID, name, sku, category, price, quantity, null);
	}

	public void LogItem(string transID, string name, string sku, string category, double price, long quantity, string currencyCode)
	{
		ItemHitBuilder builder = new ItemHitBuilder().SetTransactionID(transID).SetName(name).SetSKU(sku)
			.SetCategory(category)
			.SetPrice(price)
			.SetQuantity(quantity)
			.SetCurrencyCode(currencyCode);
		LogItem(builder);
	}

	public void LogItem(ItemHitBuilder builder)
	{
		InitializeTracker();
		if (builder.Validate() != null)
		{
			if (belowThreshold(logLevel, DebugMode.VERBOSE))
			{
				Debug.Log("Logging item.");
			}
			mpTracker.LogItem(builder);
		}
	}

	public void LogException(string exceptionDescription, bool isFatal, string userData)
	{
		ExceptionHitBuilder builder = new ExceptionHitBuilder().SetExceptionDescription(exceptionDescription).SetFatal(isFatal);
		LogException(builder, userData);
	}

	public void LogException(ExceptionHitBuilder builder, string userData)
	{
		InitializeTracker();
		if (builder.Validate() != null)
		{
			if (belowThreshold(logLevel, DebugMode.VERBOSE))
			{
				Debug.Log("Logging exception.");
			}
			mpTracker.LogException(builder, userData);
		}
	}

	public void LogSocial(string socialNetwork, string socialAction, string socialTarget)
	{
		SocialHitBuilder builder = new SocialHitBuilder().SetSocialNetwork(socialNetwork).SetSocialAction(socialAction).SetSocialTarget(socialTarget);
		LogSocial(builder);
	}

	public void LogSocial(SocialHitBuilder builder)
	{
		InitializeTracker();
		if (builder.Validate() != null)
		{
			if (belowThreshold(logLevel, DebugMode.VERBOSE))
			{
				Debug.Log("Logging social.");
			}
			mpTracker.LogSocial(builder);
		}
	}

	public void LogTiming(string timingCategory, long timingInterval, string timingName, string timingLabel)
	{
		TimingHitBuilder builder = new TimingHitBuilder().SetTimingCategory(timingCategory).SetTimingInterval(timingInterval).SetTimingName(timingName)
			.SetTimingLabel(timingLabel);
		LogTiming(builder);
	}

	public void LogTiming(TimingHitBuilder builder)
	{
		InitializeTracker();
		if (builder.Validate() != null)
		{
			if (belowThreshold(logLevel, DebugMode.VERBOSE))
			{
				Debug.Log("Logging timing.");
			}
			mpTracker.LogTiming(builder);
		}
	}

	public void Dispose()
	{
		initialized = false;
	}

	public static bool belowThreshold(DebugMode userLogLevel, DebugMode comparelogLevel)
	{
		if (comparelogLevel == userLogLevel)
		{
			return true;
		}
		switch (userLogLevel)
		{
		case DebugMode.ERROR:
			return false;
		case DebugMode.VERBOSE:
			return true;
		case DebugMode.WARNING:
			if (comparelogLevel == DebugMode.INFO || comparelogLevel == DebugMode.VERBOSE)
			{
				return false;
			}
			break;
		}
		if (userLogLevel == DebugMode.INFO && comparelogLevel == DebugMode.VERBOSE)
		{
			return false;
		}
		return true;
	}

	public static GoogleAnalyticsV3 getInstance()
	{
		return instance;
	}
}
