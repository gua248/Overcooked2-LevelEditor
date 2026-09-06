using System;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

public class ServerPreparationContainer : ServerSynchroniserBase, IHandlePlacement, IOrderDefinition, IContainerTransferBehaviour, IPlaceUnder, IHandleOrderModification, IBaseHandlePlacement
{
	private PreparationContainer m_preparationContainer;

	private ServerIngredientContainer m_itemContainer;

	protected OrderCompositionChangedCallback m_orderCompositionChangedCallbacks = delegate
	{
	};

	public override void StartSynchronising(Component synchronisedObject)
	{
		m_preparationContainer = (PreparationContainer)synchronisedObject;
		m_itemContainer = base.gameObject.GetComponent<ServerIngredientContainer>();
		m_itemContainer.RegisterContentsChangedCallback(delegate
		{
			m_orderCompositionChangedCallbacks(GetOrderComposition());
		});
		ServerThrowableItem serverThrowableItem = base.gameObject.RequestComponent<ServerThrowableItem>();
		if (serverThrowableItem != null)
		{
			serverThrowableItem.RegisterCanThrowCallback(AllowThrowing);
		}
	}

	public virtual bool CanTransferToContainer(IIngredientContents _container)
	{
		// patch
		if (_container as MonoBehaviour != null)
		{
			GameObject container = (_container as MonoBehaviour).gameObject;
			if (container.RequestComponent<ServerMixableContainer>() != null ||
				container.RequestComponent<ServerCookableContainer>() != null)
			{
				if (m_itemContainer.HasContents()) 
					return false;
                AssembledDefinitionNode assembledDefinitionNode = new IngredientAssembledNode(m_preparationContainer.m_ingredientOrderNode);
                return AssembledNodeTransfer.CanCombineWithContents(assembledDefinitionNode, _container);
            }
		}
        // patch
		if (_container.HasContents())
		{
			for (int i = 0; i < _container.GetContentsCount(); i++)
			{
				AssembledDefinitionNode contentsElement = _container.GetContentsElement(i);
				if (!CanAddIngredient(contentsElement))
				{
					return false;
				}
			}
		}
		return true;
	}

	public virtual void TransferToContainer(ICarrierPlacement _carrier, IIngredientContents _container, bool _dontRemove)
	{
        // patch
        if (_container as MonoBehaviour != null)
        {
            GameObject container = (_container as MonoBehaviour).gameObject;
            if (container.RequestComponent<ServerMixableContainer>() != null ||
                container.RequestComponent<ServerCookableContainer>() != null)
            {
				if (m_itemContainer.HasContents())
					return;
                AssembledDefinitionNode assembledDefinitionNode = new IngredientAssembledNode(m_preparationContainer.m_ingredientOrderNode);
                if (_dontRemove)
                {
                    assembledDefinitionNode = assembledDefinitionNode.Simpilfy();
                }
                AssembledNodeTransfer.CombineWithContents(assembledDefinitionNode, _container, _dontRemove);
                if (!_dontRemove)
                {
                    _carrier.DestroyCarriedItem();
                }
				return;
            }
        }
        // patch

		if (_dontRemove)
		{
			TestAddToOtherContainer(_container);
			return;
		}
		GameObject gameObject = _carrier.InspectCarriedItem();
		if (gameObject == base.gameObject)
		{
			CookedCompositeAssembledNode cookedCompositeAssembledNode = GetOrderComposition() as CookedCompositeAssembledNode;
			if (cookedCompositeAssembledNode != null)
			{
				_carrier.TakeItem();
				AddToOtherContainer(_container);
			}
			else
			{
				AddToOtherContainer(_container);
				_carrier.DestroyCarriedItem();
			}
			return;
		}
		ServerHandlePickupReferral serverHandlePickupReferral = base.gameObject.RequestComponent<ServerHandlePickupReferral>();
		if (!(serverHandlePickupReferral != null))
		{
			return;
		}
		IHandlePickup handlePickupReferree = serverHandlePickupReferral.GetHandlePickupReferree();
		if (handlePickupReferree == null || !(handlePickupReferree as ServerAttachStation != null))
		{
			return;
		}
		ServerAttachStation serverAttachStation = handlePickupReferree as ServerAttachStation;
		if (serverAttachStation.HasItem())
		{
			GameObject gameObject2 = serverAttachStation.TakeItem();
			AddToOtherContainer(_container);
			CookedCompositeAssembledNode cookedCompositeAssembledNode2 = GetOrderComposition() as CookedCompositeAssembledNode;
			if (cookedCompositeAssembledNode2 == null)
			{
				NetworkUtils.DestroyObject(gameObject2);
			}
		}
	}

	public void TestAddToOtherContainer(IIngredientContents _container)
	{
		CompositeAssembledNode asOrderComposite = GetAsOrderComposite();
		if (_container.HasContents())
		{
			for (int i = 0; i < _container.GetContentsCount(); i++)
			{
				AssembledDefinitionNode contentsElement = _container.GetContentsElement(i);
				asOrderComposite.AddOrderNode(contentsElement, true);
			}
			_container.Empty();
		}
		_container.AddIngredient(asOrderComposite);
	}

	public void AddToOtherContainer(IIngredientContents _container)
	{
		if (_container.HasContents())
		{
			for (int i = 0; i < _container.GetContentsCount(); i++)
			{
				AssembledDefinitionNode contentsElement = _container.GetContentsElement(i);
				m_itemContainer.AddIngredient(contentsElement);
			}
			_container.Empty();
		}
		CompositeAssembledNode asOrderComposite = GetAsOrderComposite();
		_container.AddIngredient(asOrderComposite);
		asOrderComposite.m_freeObject = base.gameObject;
		ServerPhysicalAttachment component = GetComponent<ServerPhysicalAttachment>();
		if (component != null)
		{
			component.ManualDisable(true);
		}
		base.gameObject.SetActive(false);
	}

	protected virtual bool CanAddIngredient(AssembledDefinitionNode _toAdd)
	{
		CompositeAssembledNode asOrderComposite = GetAsOrderComposite();
		return asOrderComposite.CanAddOrderNode(_toAdd, true);
	}

	private AssembledDefinitionNode[] GetOrderDefinitionOfCarriedItem(GameObject _carriedItem)
	{
		IBaseCookable cookingHandler = null;
		ServerCookableContainer component = _carriedItem.GetComponent<ServerCookableContainer>();
		if (component != null)
		{
			cookingHandler = component.GetCookingHandler();
		}
		return m_preparationContainer.GetOrderDefinitionOfCarriedItem(_carriedItem, m_itemContainer, cookingHandler);
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

	public virtual void AddOrderContents(AssembledDefinitionNode[] _contents)
	{
		m_itemContainer.CopyContents(_contents);
	}

	public bool CanHandlePlacement(ICarrier _carrier, Vector2 _directionXZ, PlacementContext _context)
	{
		GameObject gameObject = _carrier.InspectCarriedItem();
		AssembledDefinitionNode[] orderDefinitionOfCarriedItem = GetOrderDefinitionOfCarriedItem(gameObject);
		ServerIngredientContainer component = gameObject.GetComponent<ServerIngredientContainer>();
		Plate component2 = gameObject.GetComponent<Plate>();
		if (!CanAddOrderContents(orderDefinitionOfCarriedItem))
		{
			Tray component3 = gameObject.GetComponent<Tray>();
			if (component3 != null)
			{
				return true;
			}
			if (!(component2 != null) || !(component != null) || !CanTransferToContainer(component))
			{
				return false;
			}
		}
		if (component2 != null && m_preparationContainer.m_ingredientOrderNode.m_platingStep != component2.m_platingStep)
		{
			return false;
		}
		return true;
	}

	public virtual void HandlePlacement(ICarrier _carrier, Vector2 _directionXZ, PlacementContext _context)
	{
		GameObject gameObject = _carrier.InspectCarriedItem();
		AssembledDefinitionNode[] orderDefinitionOfCarriedItem = GetOrderDefinitionOfCarriedItem(gameObject);
		if ((bool)gameObject.GetComponent<CookableContainer>())
		{
			ServerIngredientContainer component = gameObject.GetComponent<ServerIngredientContainer>();
			AddOrderContents(orderDefinitionOfCarriedItem);
			component.RemoveIngredient(0);
		}
		else if (gameObject.GetComponent<Plate>() != null)
		{
			ServerIngredientContainer component2 = gameObject.GetComponent<ServerIngredientContainer>();
			if (orderDefinitionOfCarriedItem.Length > 0 && gameObject.GetComponent<Tray>() == null)
			{
				AddOrderContents(orderDefinitionOfCarriedItem);
				component2.Empty();
			}
			else if (CanTransferToContainer(component2))
			{
				TransferToContainer(_carrier, component2, false);
				ServerPhysicalAttachment serverPhysicalAttachment = base.gameObject.RequestComponent<ServerPhysicalAttachment>();
				if (serverPhysicalAttachment != null)
				{
					serverPhysicalAttachment.ManualDisable(true);
				}
			}
		}
		else
		{
			AddOrderContents(orderDefinitionOfCarriedItem);
			_carrier.DestroyCarriedItem();
		}
	}

	public bool AllowThrowing()
	{
		return m_itemContainer.GetContentsCount() == 0;
	}

	public void OnFailedToPlace(GameObject _item)
	{
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

	public int GetPlacementPriority()
	{
		return int.MinValue;
	}
}
