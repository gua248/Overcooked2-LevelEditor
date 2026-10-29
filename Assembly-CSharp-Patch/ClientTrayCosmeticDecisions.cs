using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

public class ClientTrayCosmeticDecisions : ClientSynchroniserBase
{
	protected TrayCosmeticDecisions m_trayCosmeticDecisions;

	private GameObject[] m_containers;

	private IClientOrderDefinition m_iOrderDefinition;

	private Tray m_tray;

	private ClientTray m_clientTray;

	private ClientIngredientContainer m_ingredientContainer;

	public override void StartSynchronising(Component synchronisedObject)
	{
		m_trayCosmeticDecisions = (TrayCosmeticDecisions)synchronisedObject;
		m_iOrderDefinition = base.gameObject.RequireInterface<IClientOrderDefinition>();
		m_iOrderDefinition.RegisterOrderCompositionChangedCallback(OnOrderCompositionChanged);
		m_tray = base.gameObject.RequireComponent<Tray>();
		m_clientTray = base.gameObject.RequireComponent<ClientTray>();
		m_containers = new GameObject[m_trayCosmeticDecisions.m_attachPoints.Length];
		m_ingredientContainer = base.gameObject.RequireComponent<ClientIngredientContainer>();
		OnOrderCompositionChanged(m_iOrderDefinition.GetOrderComposition());
	}

	protected void OnOrderCompositionChanged(AssembledDefinitionNode _orderComposition)
	{
		ClientPlate.IngredientContainerAdapter ingredientContainerAdapter = new ClientPlate.IngredientContainerAdapter(m_ingredientContainer);
		for (int i = 0; i < m_trayCosmeticDecisions.m_attachPoints.Length; i++)
		{
			if (m_containers[i] != null)
			{
				Object.Destroy(m_containers[i]);
				m_containers[i] = null;
			}
			IIngredientContents ingredientContents = m_clientTray.GetIngredientContents(i, false);
			if (ingredientContents == null)
			{
				continue;
			}
			AssembledDefinitionNode orderComposition = m_tray.GetOrderComposition(ingredientContents.GetContents());
			if (!IsEmpty(orderComposition))
			{
				GameObject orderPlatingPrefab = GameUtils.GetOrderPlatingPrefab(orderComposition, m_trayCosmeticDecisions.m_platingStep);
				m_containers[i] = Object.Instantiate((!(orderPlatingPrefab != null)) ? m_trayCosmeticDecisions.m_noMatchingRecipePrefab : orderPlatingPrefab);
				// patch
				m_containers[i].SetActive(true);
				// patch
				m_containers[i].transform.SetParent(m_trayCosmeticDecisions.m_attachPoints[i]);
				m_containers[i].transform.localPosition = Vector3.zero;
				m_containers[i].transform.localRotation = Quaternion.identity;
				RendererSceneInfo rendererSceneInfo = m_containers[i].RequestComponent<RendererSceneInfo>();
				if (rendererSceneInfo == null)
				{
					rendererSceneInfo = m_containers[i].AddComponent<RendererSceneInfo>();
					rendererSceneInfo.m_rendererClass = RendererSceneSettings.RendererClass.MealCosmetic;
				}
				IAssignOrderDefinition assignOrderDefinition = m_containers[i].RequestInterface<IAssignOrderDefinition>();
				if (assignOrderDefinition != null)
				{
					assignOrderDefinition.SetOrderComposition(orderComposition);
				}
			}
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
}
