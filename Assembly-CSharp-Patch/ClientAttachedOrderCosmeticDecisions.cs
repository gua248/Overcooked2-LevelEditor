using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

public class ClientAttachedOrderCosmeticDecisions : ClientSynchroniserBase
{
	private AttachedOrderCosmeticDecisions m_attachedOrderCosmeticDecisions;

	private GameObject m_container;

	private IClientOrderDefinition m_iOrderDefinition;

	public override void StartSynchronising(Component synchronisedObject)
	{
		m_attachedOrderCosmeticDecisions = (AttachedOrderCosmeticDecisions)synchronisedObject;
		m_iOrderDefinition = base.gameObject.RequireInterface<IClientOrderDefinition>();
		m_iOrderDefinition.RegisterOrderCompositionChangedCallback(OnOrderCompositionChanged);
		OnOrderCompositionChanged(m_iOrderDefinition.GetOrderComposition());
	}

	private void OnOrderCompositionChanged(AssembledDefinitionNode _orderComposition)
	{
		if (m_container != null)
		{
			Object.Destroy(m_container);
			m_container = null;
		}
		if (!IsEmpty(_orderComposition))
		{
			GameObject orderPlatingPrefab = GameUtils.GetOrderPlatingPrefab(_orderComposition, m_attachedOrderCosmeticDecisions.m_platingStep);
			m_container = Object.Instantiate((!(orderPlatingPrefab != null)) ? m_attachedOrderCosmeticDecisions.m_noMatchingRecipePrefab : orderPlatingPrefab);
			// patch
			m_container.SetActive(true);
			// patch
			m_container.transform.SetParent(m_attachedOrderCosmeticDecisions.m_attachPoint);
			m_container.transform.localPosition = Vector3.zero;
			m_container.transform.localRotation = Quaternion.identity;
			RendererSceneInfo rendererSceneInfo = m_container.RequestComponent<RendererSceneInfo>();
			if (rendererSceneInfo == null)
			{
				rendererSceneInfo = m_container.AddComponent<RendererSceneInfo>();
				rendererSceneInfo.m_rendererClass = RendererSceneSettings.RendererClass.MealCosmetic;
			}
			IAssignOrderDefinition assignOrderDefinition = m_container.RequestInterface<IAssignOrderDefinition>();
			if (assignOrderDefinition != null)
			{
				assignOrderDefinition.SetOrderComposition(_orderComposition);
			}
			OnContentsCreated(m_container);
		}
	}

	private bool IsEmpty(AssembledDefinitionNode _orderComposition)
	{
		CompositeAssembledNode compositeAssembledNode = _orderComposition as CompositeAssembledNode;
		if (compositeAssembledNode != null)
		{
			return compositeAssembledNode.m_composition.Length == 0;
		}
		return false;
	}

	protected virtual void OnContentsCreated(GameObject _contents)
	{
	}
}
