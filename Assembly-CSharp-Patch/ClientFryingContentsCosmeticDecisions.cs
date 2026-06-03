using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

public class ClientFryingContentsCosmeticDecisions : ClientSynchroniserBase, IClientCookingNotifed
{
	private FryingContentsCosmeticDecisions m_fryingContentsCosmeticDecisions;

	private Renderer m_renderer;

	protected Renderer AccessRenderer
	{
		get
		{
			return m_renderer;
		}
	}

	public override void StartSynchronising(Component synchronisedObject)
	{
		m_fryingContentsCosmeticDecisions = (FryingContentsCosmeticDecisions)synchronisedObject;
		// patch
		m_renderer = base.gameObject.RequestComponentRecursive<Renderer>();
		// patch
	}

	public virtual void OnCookingStarted()
	{
	}

	public virtual void OnCookingFinished()
	{
	}

	public virtual void OnCookingPropChanged(float newProp)
	{
		if (m_renderer != null)
		{
			float value = MathUtils.ClampedRemap(newProp, m_fryingContentsCosmeticDecisions.m_lowerClampProp, 1f, 0f, 1f);
			m_renderer.material.SetFloat("Prop", value);
		}
	}
}
