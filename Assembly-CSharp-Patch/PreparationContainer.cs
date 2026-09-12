using UnityEngine;

[AddComponentMenu("Scripts/Game/Environment/FoodObjects/PreparationContainer")]
[RequireComponent(typeof(PhysicalAttachment))]
[RequireComponent(typeof(IngredientContainer))]
[RequireComponent(typeof(HandlePlacementReferral))]
public class PreparationContainer : MonoBehaviour, ISpawnableItem
{
	[SerializeField]
	public OrderToPrefabLookup m_containerRestrictions;

	[SerializeField]
	public IngredientOrderNode m_ingredientOrderNode;

	[SerializeField]
	public GameObject m_cosmeticsPrefab;

	public SubTexture2D GetSubTexture()
	{
		return m_ingredientOrderNode.m_crateLid;
	}

	public Sprite GetUIIcon()
	{
		return m_ingredientOrderNode.m_iconSprite;
	}

	public AssembledDefinitionNode[] GetOrderDefinitionOfCarriedItem(GameObject _carriedItem, IIngredientContents _itemContainer, IBaseCookable _cookingHandler)
	{
		AssembledDefinitionNode[] result = null;
		if (_carriedItem.GetComponent<CookableContainer>() != null)
		{
			ClientIngredientContainer component = _carriedItem.GetComponent<ClientIngredientContainer>();
			IContainerTransferBehaviour containerTransferBehaviour = _carriedItem.RequireInterface<IContainerTransferBehaviour>();
			if (component.HasContents() && containerTransferBehaviour.CanTransferToContainer(_itemContainer))
			{
				// patch
				//CookableContainer component2 = _carriedItem.GetComponent<CookableContainer>();
				//ClientMixableContainer component3 = _carriedItem.GetComponent<ClientMixableContainer>();
				//AssembledDefinitionNode cookableMixableContents = null;
				//bool isMixed = false;
				//if (component3 != null)
				//{
				//	cookableMixableContents = component3.GetOrderComposition();
				//	isMixed = component3.GetMixingHandler().IsMixed();
				//}
				//CookedCompositeAssembledNode cookedCompositeAssembledNode = component2.GetOrderComposition(_itemContainer, _cookingHandler, cookableMixableContents, isMixed) as CookedCompositeAssembledNode;
				//cookedCompositeAssembledNode.m_composition = new AssembledDefinitionNode[1] { component.GetContentsElement(0) };
				ClientCookableContainer component2 = _carriedItem.GetComponent<ClientCookableContainer>();
				CookedCompositeAssembledNode cookedCompositeAssembledNode = component2.GetOrderComposition() as CookedCompositeAssembledNode;
				// patch
				result = new AssembledDefinitionNode[1] { cookedCompositeAssembledNode };
			}
		}
		else if (_carriedItem.GetComponent<IngredientPropertiesComponent>() != null)
		{
			IngredientPropertiesComponent component4 = _carriedItem.GetComponent<IngredientPropertiesComponent>();
			result = new AssembledDefinitionNode[1] { component4.GetOrderComposition() };
		}
		else if (_carriedItem.GetComponent<Plate>() != null)
		{
			ClientIngredientContainer component5 = _carriedItem.GetComponent<ClientIngredientContainer>();
			result = component5.GetContents();
		}
		return result;
	}
}
