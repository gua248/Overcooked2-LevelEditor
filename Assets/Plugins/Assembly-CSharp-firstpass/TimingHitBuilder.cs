using UnityEngine;

public class TimingHitBuilder : HitBuilder<TimingHitBuilder>
{
	private string timingCategory = string.Empty;

	private long timingInterval;

	private string timingName = string.Empty;

	private string timingLabel = string.Empty;

	public string GetTimingCategory()
	{
		return timingCategory;
	}

	public TimingHitBuilder SetTimingCategory(string timingCategory)
	{
		if (timingCategory != null)
		{
			this.timingCategory = timingCategory;
		}
		return this;
	}

	public long GetTimingInterval()
	{
		return timingInterval;
	}

	public TimingHitBuilder SetTimingInterval(long timingInterval)
	{
		this.timingInterval = timingInterval;
		return this;
	}

	public string GetTimingName()
	{
		return timingName;
	}

	public TimingHitBuilder SetTimingName(string timingName)
	{
		if (timingName != null)
		{
			this.timingName = timingName;
		}
		return this;
	}

	public string GetTimingLabel()
	{
		return timingLabel;
	}

	public TimingHitBuilder SetTimingLabel(string timingLabel)
	{
		if (timingLabel != null)
		{
			this.timingLabel = timingLabel;
		}
		return this;
	}

	public override TimingHitBuilder GetThis()
	{
		return this;
	}

	public override TimingHitBuilder Validate()
	{
		if (string.IsNullOrEmpty(timingCategory))
		{
			Debug.LogError("No timing category provided - Timing hit cannot be sent");
			return null;
		}
		if (timingInterval == 0)
		{
			Debug.Log("Interval in timing hit is 0.");
		}
		return this;
	}
}
