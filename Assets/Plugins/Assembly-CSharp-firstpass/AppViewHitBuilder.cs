using UnityEngine;

public class AppViewHitBuilder : HitBuilder<AppViewHitBuilder>
{
	private string screenName = string.Empty;

	public string GetScreenName()
	{
		return screenName;
	}

	public AppViewHitBuilder SetScreenName(string screenName)
	{
		if (screenName != null)
		{
			this.screenName = screenName;
		}
		return this;
	}

	public override AppViewHitBuilder GetThis()
	{
		return this;
	}

	public override AppViewHitBuilder Validate()
	{
		if (string.IsNullOrEmpty(screenName))
		{
			Debug.Log("No screen name provided - App View hit cannot be sent.");
			return null;
		}
		return this;
	}
}
