using System;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

public class ClientPreparationContainer : ClientSynchroniserBase, IClientOrderDefinition, IClientHandlePlacement, IBaseHandlePlacement
{
	private PreparationContainer m_preparationContainer;

	protected OrderCompositionChangedCallback m_orderCompositionChangedCallbacks = delegate
	{
	};

	private ClientIngredientContainer m_itemContainer;

	public override void StartSynchronising(Component synchronisedObject)
	{
		m_preparationContainer = (PreparationContainer)synchronisedObject;
		m_itemContainer = base.gameObject.GetComponent<ClientIngredientContainer>();
		m_itemContainer.RegisterContentsChangedCallback(delegate
		{
			m_orderCompositionChangedCallbacks(GetOrderComposition());
		});
		if (m_preparationContainer.m_cosmeticsPrefab != null)
		{
			Transform parent = NetworkUtils.FindVisualRoot(base.gameObject);
			GameObject gameObject = UnityEngine.Object.Instantiate(m_preparationContainer.m_cosmeticsPrefab, parent);
			if (gameObject != null)
			{
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				// patch
				gameObject.SetActive(true);
                // patch
            }
        }
		ClientThrowableItem clientThrowableItem = base.gameObject.RequestComponent<ClientThrowableItem>();
		if (clientThrowableItem != null)
		{
			clientThrowableItem.RegisterCanThrowCallback(AllowThrowing);
		}
	}

	public AssembledDefinitionNode GetOrderComposition()
	{
		return GetAsOrderComposite();
	}

	protected virtual CompositeAssembledNode GetAsOrderComposite()
	{
		CompositeAssembledNode compositeAssembledNode = new CompositeAssembledNode();
		compositeAssembledNode.m_composition = m_itemContainer.GetContents();
		Array.Resize(ref compositeAssembledNode.m_composition, compositeAssembledNode.m_composition.Length + 1);
		compositeAssembledNode.m_composition[compositeAssembledNode.m_composition.Length - 1] = new IngredientAssembledNode(m_preparationContainer.m_ingredientOrderNode);
		compositeAssembledNode.m_permittedEntries = m_preparationContainer.m_containerRestrictions.GetContentRestrictions();
		compositeAssembledNode.m_freeObject = base.gameObject;
		return compositeAssembledNode;
	}

	public void RegisterOrderCompositionChangedCallback(OrderCompositionChangedCallback _callback)
	{
		m_orderCompositionChangedCallbacks = (OrderCompositionChangedCallback)Delegate.Combine(m_orderCompositionChangedCallbacks, _callback);
	}

	public void UnregisterOrderCompositionChangedCallback(OrderCompositionChangedCallback _callback)
	{
		m_orderCompositionChangedCallbacks = (OrderCompositionChangedCallback)Delegate.Remove(m_orderCompositionChangedCallbacks, _callback);
	}

	public bool CanAddOrderContents(AssembledDefinitionNode[] _contents)
	{
		if (_contents != null)
		{
			foreach (AssembledDefinitionNode toAdd in _contents)
			{
				if (!CanAddIngredient(toAdd))
				{
					return false;
				}
			}
			return m_itemContainer.CanTakeContents(_contents);
		}
		return false;
	}

	private bool CanAddIngredient(AssembledDefinitionNode _toAdd)
	{
		CompositeAssembledNode asOrderComposite = GetAsOrderComposite();
		return asOrderComposite.CanAddOrderNode(_toAdd, true);
	}

	private AssembledDefinitionNode[] GetOrderDefinitionOfCarriedItem(GameObject _carriedItem)
	{
		IBaseCookable cookingHandler = null;
		ClientCookableContainer component = _carriedItem.GetComponent<ClientCookableContainer>();
		if (component != null)
		{
			cookingHandler = component.GetCookingHandler();
		}
		return m_preparationContainer.GetOrderDefinitionOfCarriedItem(_carriedItem, m_itemContainer, cookingHandler);
	}

	public bool CanHandlePlacement(ICarrier _carrier, Vector2 _directionXZ, PlacementContext _context)
	{
		GameObject carriedItem = _carrier.InspectCarriedItem();
		AssembledDefinitionNode[] orderDefinitionOfCarriedItem = GetOrderDefinitionOfCarriedItem(carriedItem);
		return CanAddOrderContents(orderDefinitionOfCarriedItem);
	}

	public int GetPlacementPriority()
	{
		return int.MinValue;
	}

	public bool AllowThrowing()
	{
		return m_itemContainer.GetContentsCount() == 0;
	}
}
