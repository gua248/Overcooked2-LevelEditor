using System.Collections.Generic;
using UnityEngine;

public abstract class HitBuilder<T>
{
	private Dictionary<int, string> customDimensions = new Dictionary<int, string>();

	private Dictionary<int, string> customMetrics = new Dictionary<int, string>();

	private string campaignName = string.Empty;

	private string campaignSource = string.Empty;

	private string campaignMedium = string.Empty;

	private string campaignKeyword = string.Empty;

	private string campaignContent = string.Empty;

	private string campaignID = string.Empty;

	private string gclid = string.Empty;

	private string dclid = string.Empty;

	public abstract T GetThis();

	public abstract T Validate();

	public T SetCustomDimension(int dimensionNumber, string value)
	{
		customDimensions.Add(dimensionNumber, value);
		return GetThis();
	}

	public Dictionary<int, string> GetCustomDimensions()
	{
		return customDimensions;
	}

	public T SetCustomMetric(int metricNumber, string value)
	{
		customMetrics.Add(metricNumber, value);
		return GetThis();
	}

	public Dictionary<int, string> GetCustomMetrics()
	{
		return customMetrics;
	}

	public string GetCampaignName()
	{
		return campaignName;
	}

	public T SetCampaignName(string campaignName)
	{
		if (campaignName != null)
		{
			this.campaignName = campaignName;
		}
		return GetThis();
	}

	public string GetCampaignSource()
	{
		return campaignSource;
	}

	public T SetCampaignSource(string campaignSource)
	{
		if (campaignSource != null)
		{
			this.campaignSource = campaignSource;
		}
		else
		{
			Debug.Log("Campaign source cannot be null or empty");
		}
		return GetThis();
	}

	public string GetCampaignMedium()
	{
		return campaignMedium;
	}

	public T SetCampaignMedium(string campaignMedium)
	{
		if (campaignMedium != null)
		{
			this.campaignMedium = campaignMedium;
		}
		return GetThis();
	}

	public string GetCampaignKeyword()
	{
		return campaignKeyword;
	}

	public T SetCampaignKeyword(string campaignKeyword)
	{
		if (campaignKeyword != null)
		{
			this.campaignKeyword = campaignKeyword;
		}
		return GetThis();
	}

	public string GetCampaignContent()
	{
		return campaignContent;
	}

	public T SetCampaignContent(string campaignContent)
	{
		if (campaignContent != null)
		{
			this.campaignContent = campaignContent;
		}
		return GetThis();
	}

	public string GetCampaignID()
	{
		return campaignID;
	}

	public T SetCampaignID(string campaignID)
	{
		if (campaignID != null)
		{
			this.campaignID = campaignID;
		}
		return GetThis();
	}

	public string GetGclid()
	{
		return gclid;
	}

	public T SetGclid(string gclid)
	{
		if (gclid != null)
		{
			this.gclid = gclid;
		}
		return GetThis();
	}

	public string GetDclid()
	{
		return dclid;
	}

	public T SetDclid(string dclid)
	{
		if (dclid != null)
		{
			this.dclid = dclid;
		}
		return GetThis();
	}
}
