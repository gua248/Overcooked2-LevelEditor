using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerTray : ServerPlate
{
	public class TrayIngredientContainerAdapter : IIngredientContents
	{
		private List<AssembledDefinitionNode> m_contents = new List<AssembledDefinitionNode>();

		private ServerIngredientContainer m_actualContainer;

		private bool m_addToReal;

		private Tray m_tray;

		public TrayIngredientContainerAdapter(TrayIngredientContainerAdapter _adapter, bool _addToReal)
		{
			m_actualContainer = _adapter.m_actualContainer;
			m_addToReal = _addToReal;
			m_tray = _adapter.m_tray;
			m_contents = new List<AssembledDefinitionNode>(_adapter.m_contents.Count);
			for (int i = 0; i < _adapter.m_contents.Count; i++)
			{
				m_contents.Add(_adapter.m_contents[i].Simpilfy());
			}
		}

		public TrayIngredientContainerAdapter(ServerIngredientContainer _container, IIngredientContents _containerToFilter, OrderToPrefabLookup _lookup, bool _addToReal, Tray _tray)
		{
			m_actualContainer = _container;
			m_addToReal = _addToReal;
			m_contents = new List<AssembledDefinitionNode>(_containerToFilter.GetContentsCount());
			m_tray = _tray;
			List<OrderContentRestriction> contentRestrictions = _lookup.GetContentRestrictions();
			for (int i = 0; i < contentRestrictions.Count; i++)
			{
				for (int num = _containerToFilter.GetContentsCount() - 1; num >= 0; num--)
				{
					AssembledDefinitionNode contentsElement = _containerToFilter.GetContentsElement(num);
					AssembledDefinitionNode assembledDefinitionNode = contentsElement.Simpilfy();
					if (AssembledDefinitionNode.MatchingAlreadySimple(assembledDefinitionNode, contentRestrictions[i].m_content.Simpilfy()))
					{
						m_contents.Add(assembledDefinitionNode);
						// patch
						AssembledDefinitionNode contents = m_tray.GetOrderComposition(GetContents());
						if (!contentRestrictions.Any(
							x => AssembledDefinitionNode.Matching(contents, x.m_content)))
						//if (GameUtils.GetOrderPlatingPrefab(m_tray.GetOrderComposition(GetContents()), m_tray.m_platingStep) == null)
						// patch
						{
							m_contents.Remove(assembledDefinitionNode);
						}
						else
						{
							_containerToFilter.RemoveIngredient(num);
						}
						break;
					}
				}
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
			CompositeAssembledNode compositeAssembledNode = _orderData as CompositeAssembledNode;
			if (compositeAssembledNode != null && compositeAssembledNode.m_permittedEntries.Count > 0)
			{
				for (int i = 0; i < compositeAssembledNode.m_composition.Length; i++)
				{
					if (m_addToReal)
					{
						m_actualContainer.AddIngredient(compositeAssembledNode.m_composition[i]);
					}
					m_contents.Add(compositeAssembledNode.m_composition[i]);
				}
			}
			else
			{
				if (m_addToReal)
				{
					m_actualContainer.AddIngredient(_orderData);
				}
				m_contents.Add(_orderData);
			}
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
			if (m_addToReal)
			{
				for (int i = 0; i < m_contents.Count; i++)
				{
					for (int j = 0; j < m_actualContainer.GetContentsCount(); j++)
					{
						if (AssembledDefinitionNode.MatchingAlreadySimple(m_contents[i], m_actualContainer.GetContentsElement(j)))
						{
							m_actualContainer.RemoveIngredient(j);
							break;
						}
					}
				}
			}
			m_contents.Clear();
		}

		public bool HasContents()
		{
			return m_contents.Count != 0;
		}
	}

	private Tray m_tray;

	private int m_currentSlot = -1;

	private TrayIngredientContainerAdapter[] m_slots;

	private AssembledDefinitionNode[] m_previousContents;

	private List<int> m_availableSlots = new List<int>();

	public override void StartSynchronising(Component synchronisedObject)
	{
		base.StartSynchronising(synchronisedObject);
		m_tray = (Tray)synchronisedObject;
		m_slots = new TrayIngredientContainerAdapter[m_tray.m_slots.Length];
		CalculateSlots(m_ingredientContainer.GetContents());
	}

	protected override bool CanPlaceOnPlate(GameObject _gameObject, PlacementContext _context)
	{
		IIngredientContents ingredientContents = GetIngredientContents(_gameObject, _context, false);
		if (ingredientContents == null)
		{
			return false;
		}
		return !IsReserved() && m_tray.CanPlaceOnPlate(_gameObject, ingredientContents);
	}

	public IIngredientContents GetIngredientContents(GameObject _gameObject, PlacementContext _context, bool modifyReal)
	{
		IOrderDefinition orderDefinition = _gameObject.RequestInterface<IOrderDefinition>();
		if (orderDefinition == null)
		{
			return null;
		}
		CalculateSlots(m_ingredientContainer.GetContents());
		AssembledDefinitionNode orderComposition = orderDefinition.GetOrderComposition();
		m_tray.GetIngredientContents(orderComposition, _context, m_availableSlots);
		for (int i = 0; i < m_availableSlots.Count; i++)
		{
			TrayIngredientContainerAdapter trayIngredientContainerAdapter = m_slots[m_availableSlots[i]];
			AssembledDefinitionNode[] array = new AssembledDefinitionNode[trayIngredientContainerAdapter.GetContentsCount() + 1];
			for (int j = 0; j < trayIngredientContainerAdapter.GetContentsCount(); j++)
			{
				array[j] = trayIngredientContainerAdapter.GetContentsElement(j);
			}
			array[array.Length - 1] = orderComposition;
			if (GameUtils.GetOrderPlatingPrefab(m_tray.GetOrderComposition(array), m_tray.m_platingStep) != null)
			{
				return new TrayIngredientContainerAdapter(trayIngredientContainerAdapter, modifyReal);
			}
		}
		return null;
	}

	public override bool CanTransferToContainer(IIngredientContents _container)
	{
		return base.CanTransferToContainer(_container);
	}

	private void CalculateSlots(AssembledDefinitionNode[] _contents)
	{
		if (m_tray.AreContentsDifferent(m_previousContents, _contents))
		{
			IngredientContainerAdapter containerToFilter = new IngredientContainerAdapter(m_ingredientContainer);
			for (int i = 0; i < m_slots.Length; i++)
			{
				m_slots[i] = new TrayIngredientContainerAdapter(m_ingredientContainer, containerToFilter, m_tray.m_slots[i], false, m_tray);
			}
		}
		m_previousContents = _contents;
	}
}
