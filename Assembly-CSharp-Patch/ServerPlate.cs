using System;
using System.Collections.Generic;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

public class ServerPlate : ServerSynchroniserBase, IOrderDefinition, IContainerTransferBehaviour
{
	public class IngredientContainerAdapter : IIngredientContents
	{
		private List<AssembledDefinitionNode> m_contents = new List<AssembledDefinitionNode>();

		public IngredientContainerAdapter(ServerIngredientContainer _container)
		{
			m_contents = new List<AssembledDefinitionNode>(_container.GetContentsCount());
			for (int i = 0; i < _container.GetContentsCount(); i++)
			{
				AssembledDefinitionNode contentsElement = _container.GetContentsElement(i);
				AssembledDefinitionNode item = contentsElement.Simpilfy();
				m_contents.Add(item);
			}
		}

		public bool CanAddIngredient(AssembledDefinitionNode _orderData)
		{
			return true;
		}

		public bool CanTakeContents(AssembledDefinitionNode[] _contents)
		{
			return true;
		}

		public void AddIngredient(AssembledDefinitionNode _orderData)
		{
			m_contents.Add(_orderData);
		}

		public AssembledDefinitionNode RemoveIngredient(int i)
		{
			AssembledDefinitionNode result = m_contents[i];
			m_contents.RemoveAt(i);
			return result;
		}

		public AssembledDefinitionNode GetContentsElement(int i)
		{
			return m_contents[i];
		}

		public int GetContentsCount()
		{
			return m_contents.Count;
		}

		public AssembledDefinitionNode[] GetContents()
		{
			return m_contents.ToArray();
		}

		public void Empty()
		{
			m_contents.Clear();
		}

		public bool HasContents()
		{
			return m_contents.Count != 0;
		}
	}

	private Plate m_plate;

	private OrderCompositionChangedCallback m_orderCompositionChangedCallbacks = delegate
	{
	};

	protected ServerIngredientContainer m_ingredientContainer;

	private ServerPlacementContainer m_placementContainer;

	private UnityEngine.Object m_reservation;

	public override void StartSynchronising(Component synchronisedObject)
	{
		m_plate = (Plate)synchronisedObject;
		m_reservation = null;
		m_ingredientContainer = base.gameObject.GetComponent<ServerIngredientContainer>();
		m_ingredientContainer.RegisterContentsChangedCallback(OnContentsChanged);
		m_placementContainer = base.gameObject.GetComponent<ServerPlacementContainer>();
		m_placementContainer.RegisterAllowItemPlacement(CanPlaceOnPlate);
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		if (null != m_placementContainer)
		{
			m_placementContainer.UnregisterAllowItemPlacement(CanPlaceOnPlate);
		}
		if (null != m_ingredientContainer)
		{
			m_ingredientContainer.UnregisterContentsChangedCallback(OnContentsChanged);
		}
	}

	public virtual bool CanTransferToContainer(IIngredientContents _container)
	{
        // patch
		bool flag = false;
        if (_container as MonoBehaviour != null)
        {
            GameObject container = (_container as MonoBehaviour).gameObject;
            if (container.RequestComponent<ServerMixableContainer>() != null ||
                container.RequestComponent<ServerCookableContainer>() != null)
            {
				flag = true;
            }
        }
        // patch
		if (!IsReserved() && m_ingredientContainer.GetContentsCount() > 0 && _container.CanTakeContents(m_ingredientContainer.GetContents()))
		{
			for (int i = 0; i < m_ingredientContainer.GetContentsCount(); i++)
			{
				AssembledDefinitionNode contentsElement = m_ingredientContainer.GetContentsElement(i);
                // patch
                //if (contentsElement.m_freeObject != null)
				if (contentsElement.m_freeObject != null && !flag)
                // patch
				{
					IHandleOrderModification handleOrderModification = contentsElement.m_freeObject.RequestInterface<IHandleOrderModification>();
					if (handleOrderModification != null && !handleOrderModification.CanAddOrderContents(_container.GetContents()))
					{
						return false;
					}
				}
				if (!AssembledNodeTransfer.CanCombineWithContents(contentsElement, _container))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public void TransferToContainer(ICarrierPlacement _carrier, IIngredientContents _otherContainer, bool _dontRemove)
	{
		if (m_ingredientContainer.GetContentsCount() <= 0 || !_otherContainer.CanTakeContents(m_ingredientContainer.GetContents()))
		{
			return;
		}
		for (int i = 0; i < m_ingredientContainer.GetContentsCount(); i++)
		{
			AssembledDefinitionNode assembledDefinitionNode = m_ingredientContainer.GetContentsElement(i);
			if (_dontRemove)
			{
				assembledDefinitionNode = assembledDefinitionNode.Simpilfy();
			}
			AssembledNodeTransfer.CombineWithContents(assembledDefinitionNode, _otherContainer, _dontRemove);
		}
		if (!_dontRemove)
		{
			m_ingredientContainer.Empty();
		}
	}

	private void OnContentsChanged(AssembledDefinitionNode[] _contents)
	{
		m_orderCompositionChangedCallbacks(GetOrderComposition());
	}

	public AssembledDefinitionNode GetOrderComposition()
	{
		return GetOrderComposition(m_ingredientContainer.GetContents());
	}

	private AssembledDefinitionNode GetOrderComposition(AssembledDefinitionNode[] _contents)
	{
		return m_plate.GetOrderComposition(_contents);
	}

	public void RegisterOrderCompositionChangedCallback(OrderCompositionChangedCallback _callback)
	{
		m_orderCompositionChangedCallbacks = (OrderCompositionChangedCallback)Delegate.Combine(m_orderCompositionChangedCallbacks, _callback);
	}

	public void UnregisterOrderCompositionChangedCallback(OrderCompositionChangedCallback _callback)
	{
		m_orderCompositionChangedCallbacks = (OrderCompositionChangedCallback)Delegate.Remove(m_orderCompositionChangedCallbacks, _callback);
	}

	public PlatingStepData GetPlatingStep()
	{
		return m_plate.m_platingStep;
	}

	private void Awake()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("GameController");
	}

	protected virtual bool CanPlaceOnPlate(GameObject _gameObject, PlacementContext _context)
	{
		return !IsReserved() && m_plate.CanPlaceOnPlate(_gameObject, new IngredientContainerAdapter(m_ingredientContainer));
	}

	public void StartDeliverySequence(ServerPlateStation _returnStation)
	{
		ServerAttachStation serverAttachStation = _returnStation.gameObject.RequireComponent<ServerAttachStation>();
		GameObject gameObject = serverAttachStation.TakeItem();
		Collider[] array = base.gameObject.RequestComponentsRecursive<Collider>();
		foreach (Collider collider in array)
		{
			collider.enabled = false;
		}
		Rigidbody rigidbody = base.gameObject.GetComponent<IAttachment>().AccessRigidbody();
		rigidbody.isKinematic = true;
	}

	public void Reserve(UnityEngine.Object reserveOwner)
	{
		m_reservation = reserveOwner;
	}

	public void ReleaseReservation(UnityEngine.Object reserveOwner)
	{
		m_reservation = null;
	}

	public bool IsReserved()
	{
		return m_reservation != null;
	}
}
