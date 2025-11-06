using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class GoogleAnalyticsMPV3
{
	private string trackingCode;

	private string bundleIdentifier;

	private string appName;

	private string appVersion;

	private GoogleAnalyticsV3.DebugMode logLevel;

	private bool anonymizeIP;

	private bool dryRun;

	private bool optOut;

	private int sessionTimeout;

	private string screenRes;

	private string clientId;

	private string url;

	private float timeStarted;

	private Dictionary<Field, object> trackerValues = new Dictionary<Field, object>();

	private bool startSessionOnNextHit;

	private bool endSessionOnNextHit;

	private bool trackingCodeSet = true;

	private List<WWW> activeRequests = new List<WWW>();

	public void InitializeTracker()
	{
		if (string.IsNullOrEmpty(trackingCode))
		{
			Debug.Log("No tracking code set for 'Other' platforms - hits will not be set");
			trackingCodeSet = false;
			return;
		}
		if (GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.INFO))
		{
			Debug.Log("Platform is not Android or iOS - hits will be sent using measurement protocol.");
		}
		screenRes = Screen.width + "x" + Screen.height;
		clientId = SystemInfo.deviceUniqueIdentifier;
		string value = Application.systemLanguage.ToString();
		optOut = false;
		CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
		CultureInfo[] array = cultures;
		foreach (CultureInfo cultureInfo in array)
		{
			if (cultureInfo.EnglishName == Application.systemLanguage.ToString())
			{
				value = cultureInfo.Name;
			}
		}
		try
		{
			url = "https://www.google-analytics.com/collect?v=1" + AddRequiredMPParameter(Fields.LANGUAGE, value) + AddRequiredMPParameter(Fields.SCREEN_RESOLUTION, screenRes) + AddRequiredMPParameter(Fields.APP_NAME, appName) + AddRequiredMPParameter(Fields.TRACKING_ID, trackingCode) + AddRequiredMPParameter(Fields.APP_ID, bundleIdentifier) + AddRequiredMPParameter(Fields.CLIENT_ID, clientId) + AddRequiredMPParameter(Fields.APP_VERSION, appVersion);
			if (anonymizeIP)
			{
				url += AddOptionalMPParameter(Fields.ANONYMIZE_IP, 1);
			}
			if (GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.VERBOSE))
			{
				Debug.Log("Base URL for hits: " + url);
			}
		}
		catch (Exception)
		{
			if (GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.WARNING))
			{
				Debug.Log("Error building url.");
			}
		}
	}

	public void SetTrackerVal(Field field, object value)
	{
		trackerValues[field] = value;
	}

	private string AddTrackerVals()
	{
		if (!trackingCodeSet)
		{
			return string.Empty;
		}
		string text = string.Empty;
		foreach (KeyValuePair<Field, object> trackerValue in trackerValues)
		{
			text += AddOptionalMPParameter(trackerValue.Key, trackerValue.Value);
		}
		return text;
	}

	internal void StartSession()
	{
		startSessionOnNextHit = true;
	}

	internal void StopSession()
	{
		endSessionOnNextHit = true;
	}

	internal void AbortHits()
	{
		for (int i = 0; i < activeRequests.Count; i++)
		{
			activeRequests[i].Dispose();
		}
		activeRequests.Clear();
		GoogleAnalyticsV3.getInstance().StopAllCoroutines();
	}

	private void SendGaHitWithMeasurementProtocol(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			if (GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.WARNING))
			{
				Debug.Log("No tracking code set for 'Other' platforms - hit will not be sent.");
			}
			return;
		}
		if (dryRun || optOut)
		{
			if (GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.WARNING))
			{
				Debug.Log("Dry run or opt out enabled - hits will not be sent.");
			}
			return;
		}
		if (startSessionOnNextHit)
		{
			url += AddOptionalMPParameter(Fields.SESSION_CONTROL, "start");
			startSessionOnNextHit = false;
		}
		else if (endSessionOnNextHit)
		{
			url += AddOptionalMPParameter(Fields.SESSION_CONTROL, "end");
			endSessionOnNextHit = false;
		}
		string message = url + "&z=" + UnityEngine.Random.Range(0, 500);
		if (GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.VERBOSE))
		{
			Debug.Log(message);
		}
		GoogleAnalyticsV3.getInstance().StartCoroutine(HandleWWW(new WWW(message)));
	}

	public IEnumerator HandleWWW(WWW request)
	{
		activeRequests.Add(request);
		while (!request.isDone)
		{
			yield return request;
			if (request.responseHeaders.ContainsKey("STATUS"))
			{
				if (request.responseHeaders["STATUS"].Contains("200 OK"))
				{
					if (GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.INFO))
					{
						Debug.Log("Successfully sent Google Analytics hit.");
					}
				}
				else if (GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.WARNING))
				{
					Debug.LogWarning("Google Analytics hit request rejected with status code " + request.responseHeaders["STATUS"]);
				}
			}
			else if (GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.WARNING))
			{
				Debug.LogWarning("Google Analytics hit request failed with error " + request.error);
			}
		}
		request.Dispose();
		activeRequests.Remove(request);
	}

	private string AddRequiredMPParameter(Field parameter, object value)
	{
		if (!trackingCodeSet)
		{
			return string.Empty;
		}
		if (value == null)
		{
			if (GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.WARNING))
			{
				Debug.LogWarning(string.Concat("Value was null for required parameter ", parameter, ". Hit cannot be sent"));
			}
			throw new ArgumentNullException();
		}
		return string.Concat(parameter, "=", WWW.EscapeURL(value.ToString()));
	}

	private string AddRequiredMPParameter(Field parameter, string value)
	{
		if (!trackingCodeSet)
		{
			return string.Empty;
		}
		if (value == null)
		{
			if (GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.WARNING))
			{
				Debug.LogWarning(string.Concat("Value was null for required parameter ", parameter, ". Hit cannot be sent"));
			}
			throw new ArgumentNullException();
		}
		return string.Concat(parameter, "=", WWW.EscapeURL(value));
	}

	private string AddOptionalMPParameter(Field parameter, object value)
	{
		if (value == null || !trackingCodeSet)
		{
			return string.Empty;
		}
		return string.Concat(parameter, "=", WWW.EscapeURL(value.ToString()));
	}

	private string AddOptionalMPParameter(Field parameter, string value)
	{
		if (string.IsNullOrEmpty(value) || !trackingCodeSet)
		{
			return string.Empty;
		}
		return string.Concat(parameter, "=", WWW.EscapeURL(value));
	}

	private string AddCustomVariables<T>(HitBuilder<T> builder)
	{
		if (!trackingCodeSet)
		{
			return string.Empty;
		}
		string text = string.Empty;
		foreach (KeyValuePair<int, string> customDimension in builder.GetCustomDimensions())
		{
			if (customDimension.Value != null)
			{
				string text2 = text;
				text = text2 + Fields.CUSTOM_DIMENSION.ToString() + customDimension.Key + "=" + WWW.EscapeURL(customDimension.Value.ToString());
			}
		}
		foreach (KeyValuePair<int, string> customMetric in builder.GetCustomMetrics())
		{
			if (customMetric.Value != null)
			{
				string text2 = text;
				text = text2 + Fields.CUSTOM_METRIC.ToString() + customMetric.Key + "=" + WWW.EscapeURL(customMetric.Value.ToString());
			}
		}
		if (!string.IsNullOrEmpty(text) && GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.VERBOSE))
		{
			Debug.Log("Added custom variables to hit.");
		}
		return text;
	}

	private string AddCampaignParameters<T>(HitBuilder<T> builder)
	{
		if (!trackingCodeSet)
		{
			return string.Empty;
		}
		string empty = string.Empty;
		empty += AddOptionalMPParameter(Fields.CAMPAIGN_NAME, builder.GetCampaignName());
		empty += AddOptionalMPParameter(Fields.CAMPAIGN_SOURCE, builder.GetCampaignSource());
		empty += AddOptionalMPParameter(Fields.CAMPAIGN_MEDIUM, builder.GetCampaignMedium());
		empty += AddOptionalMPParameter(Fields.CAMPAIGN_KEYWORD, builder.GetCampaignKeyword());
		empty += AddOptionalMPParameter(Fields.CAMPAIGN_CONTENT, builder.GetCampaignContent());
		empty += AddOptionalMPParameter(Fields.CAMPAIGN_ID, builder.GetCampaignID());
		empty += AddOptionalMPParameter(Fields.GCLID, builder.GetGclid());
		empty += AddOptionalMPParameter(Fields.DCLID, builder.GetDclid());
		if (!string.IsNullOrEmpty(empty) && GoogleAnalyticsV3.belowThreshold(logLevel, GoogleAnalyticsV3.DebugMode.VERBOSE))
		{
			Debug.Log("Added campaign parameters to hit. url:" + empty);
		}
		return empty;
	}

	public void LogScreen(AppViewHitBuilder builder)
	{
		trackerValues[Fields.SCREEN_NAME] = null;
		SendGaHitWithMeasurementProtocol(url + AddRequiredMPParameter(Fields.HIT_TYPE, "appview") + AddRequiredMPParameter(Fields.SCREEN_NAME, builder.GetScreenName()) + AddCustomVariables(builder) + AddCampaignParameters(builder) + AddTrackerVals());
	}

	public void LogEvent(EventHitBuilder builder)
	{
		trackerValues[Fields.EVENT_CATEGORY] = null;
		trackerValues[Fields.EVENT_ACTION] = null;
		trackerValues[Fields.EVENT_LABEL] = null;
		trackerValues[Fields.EVENT_VALUE] = null;
		SendGaHitWithMeasurementProtocol(url + AddRequiredMPParameter(Fields.HIT_TYPE, "event") + AddOptionalMPParameter(Fields.EVENT_CATEGORY, builder.GetEventCategory()) + AddOptionalMPParameter(Fields.EVENT_ACTION, builder.GetEventAction()) + AddOptionalMPParameter(Fields.EVENT_LABEL, builder.GetEventLabel()) + AddOptionalMPParameter(Fields.EVENT_VALUE, builder.GetEventValue()) + AddCustomVariables(builder) + AddCampaignParameters(builder) + AddTrackerVals());
	}

	public void LogTransaction(TransactionHitBuilder builder)
	{
		trackerValues[Fields.TRANSACTION_ID] = null;
		trackerValues[Fields.TRANSACTION_AFFILIATION] = null;
		trackerValues[Fields.TRANSACTION_REVENUE] = null;
		trackerValues[Fields.TRANSACTION_SHIPPING] = null;
		trackerValues[Fields.TRANSACTION_TAX] = null;
		trackerValues[Fields.CURRENCY_CODE] = null;
		SendGaHitWithMeasurementProtocol(url + AddRequiredMPParameter(Fields.HIT_TYPE, "transaction") + AddRequiredMPParameter(Fields.TRANSACTION_ID, builder.GetTransactionID()) + AddOptionalMPParameter(Fields.TRANSACTION_AFFILIATION, builder.GetAffiliation()) + AddOptionalMPParameter(Fields.TRANSACTION_REVENUE, builder.GetRevenue()) + AddOptionalMPParameter(Fields.TRANSACTION_SHIPPING, builder.GetShipping()) + AddOptionalMPParameter(Fields.TRANSACTION_TAX, builder.GetTax()) + AddOptionalMPParameter(Fields.CURRENCY_CODE, builder.GetCurrencyCode()) + AddCustomVariables(builder) + AddCampaignParameters(builder) + AddTrackerVals());
	}

	public void LogItem(ItemHitBuilder builder)
	{
		trackerValues[Fields.TRANSACTION_ID] = null;
		trackerValues[Fields.ITEM_NAME] = null;
		trackerValues[Fields.ITEM_SKU] = null;
		trackerValues[Fields.ITEM_CATEGORY] = null;
		trackerValues[Fields.ITEM_PRICE] = null;
		trackerValues[Fields.ITEM_QUANTITY] = null;
		trackerValues[Fields.CURRENCY_CODE] = null;
		SendGaHitWithMeasurementProtocol(url + AddRequiredMPParameter(Fields.HIT_TYPE, "item") + AddRequiredMPParameter(Fields.TRANSACTION_ID, builder.GetTransactionID()) + AddRequiredMPParameter(Fields.ITEM_NAME, builder.GetName()) + AddOptionalMPParameter(Fields.ITEM_SKU, builder.GetSKU()) + AddOptionalMPParameter(Fields.ITEM_CATEGORY, builder.GetCategory()) + AddOptionalMPParameter(Fields.ITEM_PRICE, builder.GetPrice()) + AddOptionalMPParameter(Fields.ITEM_QUANTITY, builder.GetQuantity()) + AddOptionalMPParameter(Fields.CURRENCY_CODE, builder.GetCurrencyCode()) + AddCustomVariables(builder) + AddCampaignParameters(builder) + AddTrackerVals());
	}

	public void LogException(ExceptionHitBuilder builder, string userData)
	{
		trackerValues[Fields.EX_DESCRIPTION] = null;
		trackerValues[Fields.EX_FATAL] = null;
		SendGaHitWithMeasurementProtocol(url + AddRequiredMPParameter(Fields.HIT_TYPE, "exception") + AddOptionalMPParameter(Fields.EX_DESCRIPTION, builder.GetExceptionDescription()) + AddOptionalMPParameter(Fields.EX_FATAL, builder.IsFatal()) + AddRequiredMPParameter(Fields.SCREEN_NAME, userData) + AddTrackerVals());
	}

	public void LogSocial(SocialHitBuilder builder)
	{
		trackerValues[Fields.SOCIAL_NETWORK] = null;
		trackerValues[Fields.SOCIAL_ACTION] = null;
		trackerValues[Fields.SOCIAL_TARGET] = null;
		SendGaHitWithMeasurementProtocol(url + AddRequiredMPParameter(Fields.HIT_TYPE, "social") + AddRequiredMPParameter(Fields.SOCIAL_NETWORK, builder.GetSocialNetwork()) + AddRequiredMPParameter(Fields.SOCIAL_ACTION, builder.GetSocialAction()) + AddRequiredMPParameter(Fields.SOCIAL_TARGET, builder.GetSocialTarget()) + AddCustomVariables(builder) + AddCampaignParameters(builder) + AddTrackerVals());
	}

	public void LogTiming(TimingHitBuilder builder)
	{
		trackerValues[Fields.TIMING_CATEGORY] = null;
		trackerValues[Fields.TIMING_VALUE] = null;
		trackerValues[Fields.TIMING_LABEL] = null;
		trackerValues[Fields.TIMING_VAR] = null;
		SendGaHitWithMeasurementProtocol(url + AddRequiredMPParameter(Fields.HIT_TYPE, "timing") + AddOptionalMPParameter(Fields.TIMING_CATEGORY, builder.GetTimingCategory()) + AddOptionalMPParameter(Fields.TIMING_VALUE, builder.GetTimingInterval()) + AddOptionalMPParameter(Fields.TIMING_LABEL, builder.GetTimingLabel()) + AddOptionalMPParameter(Fields.TIMING_VAR, builder.GetTimingName()) + AddCustomVariables(builder) + AddCampaignParameters(builder) + AddTrackerVals());
	}

	public void ClearUserIDOverride()
	{
		SetTrackerVal(Fields.USER_ID, null);
	}

	public void SetTrackingCode(string trackingCode)
	{
		this.trackingCode = trackingCode;
	}

	public void SetBundleIdentifier(string bundleIdentifier)
	{
		this.bundleIdentifier = bundleIdentifier;
	}

	public void SetAppName(string appName)
	{
		this.appName = appName;
	}

	public void SetAppVersion(string appVersion)
	{
		this.appVersion = appVersion;
	}

	public void SetLogLevelValue(GoogleAnalyticsV3.DebugMode logLevel)
	{
		this.logLevel = logLevel;
	}

	public void SetAnonymizeIP(bool anonymizeIP)
	{
		this.anonymizeIP = anonymizeIP;
	}

	public void SetDryRun(bool dryRun)
	{
		this.dryRun = dryRun;
	}

	public void SetOptOut(bool optOut)
	{
		this.optOut = optOut;
	}
}
