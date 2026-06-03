using System;
using System.Collections;
using UnityEngine;

public class DialogueAnimationState : CoroutineStateBehaviour
{
	[SerializeField]
	private string m_completedTrigger;

	[SerializeField]
	private DialogueFlowroutine m_dialogueFlowroutine = new DialogueFlowroutine();

	[SerializeField]
	private Vector2 m_anchor = 0.5f * Vector2.one;

	[SerializeField]
	private Vector2 m_pivot = 0.5f * Vector2.one;

	[SerializeField]
	private bool m_hasButtonlessAlternative;

	public int m_completedTriggerHash;

	private GameObject m_dialogueObject;

	protected virtual void Awake()
	{
		m_completedTriggerHash = Animator.StringToHash(m_completedTrigger);
	}

	private void OnValidate()
	{
		m_dialogueFlowroutine.OnValidate();
	}

	private void OnAnimatorEnable()
	{
		m_dialogueFlowroutine.OnEnable();
	}

	private void OnAnimatorDisable()
	{
		m_dialogueFlowroutine.OnDisable();
	}

	protected override void OnEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
	{
		AnimatorCommunications animatorCommunications = _animator.gameObject.RequireComponent<AnimatorCommunications>();
		animatorCommunications.AnimatorEnabledCallback = (CallbackVoid)Delegate.Combine(animatorCommunications.AnimatorEnabledCallback, new CallbackVoid(OnAnimatorEnable));
		animatorCommunications.AnimatorDisabledCallback = (CallbackVoid)Delegate.Combine(animatorCommunications.AnimatorDisabledCallback, new CallbackVoid(OnAnimatorDisable));
	}

	private static bool HasSplitPads()
	{
		GameInputConfig inputConfig = PlayerInputLookup.GetInputConfig();
		GameInputConfig.ConfigEntry[] playerConfigs = inputConfig.m_playerConfigs;
		for (int i = 0; i < playerConfigs.Length; i++)
		{
			if (playerConfigs[i].Side != PadSide.Both)
			{
				return true;
			}
		}
		return false;
	}

	private static bool HasDifferentControllers()
	{
		PlayerManager playerManager = GameUtils.RequestManager<PlayerManager>();
		GamepadUser.ControlTypeEnum? controlTypeEnum = null;
		for (int i = 0; i < 4; i++)
		{
			GamepadUser user = playerManager.GetUser((EngagementSlot)i);
			if (user != null)
			{
				if (!controlTypeEnum.HasValue)
				{
					controlTypeEnum = user.ControlType;
				}
				if (controlTypeEnum.Value != user.ControlType)
				{
					return true;
				}
			}
		}
		return false;
	}

	protected override IEnumerator Run(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
	{
		if (m_hasButtonlessAlternative && (HasDifferentControllers() || HasSplitPads()))
		{
			for (int i = 0; i < m_dialogueFlowroutine.DialogueScript.Length; i++)
			{
				string[] dialogueScript = m_dialogueFlowroutine.DialogueScript;
				if (!dialogueScript[i].Contains(".Buttonless"))
				{
					(dialogueScript)[i] = dialogueScript[i] + ".Buttonless";
				}
			}
		}
		m_dialogueFlowroutine.Setup(m_anchor, m_pivot);
		IEnumerator enumerator = m_dialogueFlowroutine.Run();
		while (enumerator.MoveNext())
		{
			yield return null;
		}
		_animator.SetTrigger(m_completedTriggerHash);
	}

	protected override void OnExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
	{
		m_dialogueFlowroutine.Shutdown();
		AnimatorCommunications animatorCommunications = _animator.gameObject.RequireComponent<AnimatorCommunications>();
		animatorCommunications.AnimatorEnabledCallback = (CallbackVoid)Delegate.Remove(animatorCommunications.AnimatorEnabledCallback, new CallbackVoid(OnAnimatorEnable));
		animatorCommunications.AnimatorDisabledCallback = (CallbackVoid)Delegate.Remove(animatorCommunications.AnimatorDisabledCallback, new CallbackVoid(OnAnimatorDisable));
	}
}
