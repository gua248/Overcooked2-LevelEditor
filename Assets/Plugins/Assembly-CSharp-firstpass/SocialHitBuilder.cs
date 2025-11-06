using UnityEngine;

public class SocialHitBuilder : HitBuilder<SocialHitBuilder>
{
	private string socialNetwork = string.Empty;

	private string socialAction = string.Empty;

	private string socialTarget = string.Empty;

	public string GetSocialNetwork()
	{
		return socialNetwork;
	}

	public SocialHitBuilder SetSocialNetwork(string socialNetwork)
	{
		if (socialNetwork != null)
		{
			this.socialNetwork = socialNetwork;
		}
		return this;
	}

	public string GetSocialAction()
	{
		return socialAction;
	}

	public SocialHitBuilder SetSocialAction(string socialAction)
	{
		if (socialAction != null)
		{
			this.socialAction = socialAction;
		}
		return this;
	}

	public string GetSocialTarget()
	{
		return socialTarget;
	}

	public SocialHitBuilder SetSocialTarget(string socialTarget)
	{
		if (socialTarget != null)
		{
			this.socialTarget = socialTarget;
		}
		return this;
	}

	public override SocialHitBuilder GetThis()
	{
		return this;
	}

	public override SocialHitBuilder Validate()
	{
		if (string.IsNullOrEmpty(socialNetwork))
		{
			Debug.LogError("No social network provided - Social hit cannot be sent");
			return null;
		}
		if (string.IsNullOrEmpty(socialAction))
		{
			Debug.LogError("No social action provided - Social hit cannot be sent");
			return null;
		}
		if (string.IsNullOrEmpty(socialTarget))
		{
			Debug.LogError("No social target provided - Social hit cannot be sent");
			return null;
		}
		return this;
	}
}
