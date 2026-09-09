using System.Collections;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

public class ClientBellowsCosmeticDecisions : ClientSynchroniserBase
{
	private int m_bellowsUsedParam = -1;

	private int m_InUseParam = -1;

	private int m_stopParam = -1;

	private BellowsCosmeticDecisions m_cosmetics;

	private Animator m_animator;

	private IClientAttachment m_attachment;

	private ClientBellowsSpray m_bellows;

	private IEnumerator m_animToggleRoutine;

	public override void StartSynchronising(Component _synchronisedObject)
	{
		base.StartSynchronising(_synchronisedObject);
		m_cosmetics = (BellowsCosmeticDecisions)_synchronisedObject;
		m_bellowsUsedParam = Animator.StringToHash(m_cosmetics.m_animParams.Used);
		m_InUseParam = Animator.StringToHash(m_cosmetics.m_animParams.InUse);
		m_stopParam = Animator.StringToHash(m_cosmetics.m_animParams.Stop);
		AddAnimator();
		m_attachment = base.gameObject.RequireInterface<IClientAttachment>();
		m_attachment.RegisterAttachChangedCallback(OnAttachmentChanged);
		m_bellows = base.gameObject.RequireComponent<ClientBellowsSpray>();
		m_bellows.RegisterOnSpray(OnSpray);
	}

	private void AddAnimator()
	{
		// patch
        m_animator = m_cosmetics.m_animTransform.gameObject.GetComponent<Animator>();
		if (m_animator == null)
            m_animator = m_cosmetics.m_animTransform.gameObject.AddComponent<Animator>();
        //m_animator = m_cosmetics.m_animTransform.gameObject.AddComponent<Animator>();
        // patch
        m_animator.runtimeAnimatorController = m_cosmetics.m_animController;
	}

	private IEnumerator ToggleAnimatorRoutine(bool _isActive)
	{
		if (_isActive)
		{
			if (m_animator != null)
			{
				m_animator.ResetTrigger(m_stopParam);
				m_animator.enabled = true;
			}
		}
		else if (m_animator != null)
		{
			m_animator.SetTrigger(m_stopParam);
			while (m_animator.GetBool(m_InUseParam))
			{
				yield return null;
			}
			yield return null;
			m_animator.enabled = false;
		}
	}

	private void OnSpray()
	{
		if (m_animator != null)
		{
			m_animator.SetTrigger(m_bellowsUsedParam);
			GameUtils.TriggerAudio(GameOneShotAudioTag.DLC_02_Bellow, base.gameObject.layer);
		}
	}

	private void OnAttachmentChanged(IParentable _parentable)
	{
		bool isActive = false;
		if (_parentable != null && _parentable is PlayerAttachmentCarrier)
		{
			isActive = true;
		}
		m_animToggleRoutine = ToggleAnimatorRoutine(isActive);
	}

	public override void UpdateSynchronising()
	{
		base.UpdateSynchronising();
		if (m_animToggleRoutine != null && !m_animToggleRoutine.MoveNext())
		{
			m_animToggleRoutine = null;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (m_attachment != null)
		{
			m_attachment.UnregisterAttachChangedCallback(OnAttachmentChanged);
		}
		if (m_bellows != null)
		{
			m_bellows.UnregisterOnSpray(OnSpray);
		}
	}
}
