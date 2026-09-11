using LevelEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class RecipeWidgetUIController : UIControllerAndContainer
{
	[Serializable]
	public class RecipeTileData
	{
		public RecipeWidgetTile.TileDefinition m_tileDefinition;

		public List<int> m_children = new List<int>();
	}

	public struct Size
	{
		public int GridTiles;

		public float PixelOffset;

		public Size(int _grid, float _pixels)
		{
			GridTiles = _grid;
			PixelOffset = _pixels;
		}
	}

	public struct Position
	{
		public Size X;

		public Size Y;

		public Position(Size _x, Size _y)
		{
			X = _x;
			Y = _y;
		}
	}

	[SerializeField]
	private RecipeTileData[] m_recipeTree = new RecipeTileData[0];

	[SerializeField]
	private RecipeWidgetTile.DisplayConfiguration m_displayConfig = new RecipeWidgetTile.DisplayConfiguration();

	[SerializeField]
	private TopRecipeWidgetTile.TopDisplayConfiguration m_topDisplayConfig = new TopRecipeWidgetTile.TopDisplayConfiguration();

	[SerializeField]
	[HideInInspector]
	private List<RecipeWidgetTile> m_recipeTiles = new List<RecipeWidgetTile>();

	[SerializeField]
	private RuntimeAnimatorController m_widgetAnimatorController;

	[SerializeField]
	[AssignResource("UI_Move", Editorbility.Editable)]
	private Material m_UIMoveMaterial;

	private Animator m_widgetAnimator;

	private WidgetAnimation m_animation;

	private int m_tableNumber;

	private TopRecipeWidgetTile m_topWidgetTile;

	private static readonly int m_iWarning = Animator.StringToHash("Warning");

	public void PlayAnimation(WidgetAnimation _animation)
	{
		m_animation = _animation;
		_animation.Init(m_widgetAnimator);
	}

	public bool IsPlayingAnimation()
	{
		return m_animation != null;
	}

	public int GetTableNumber()
	{
		return m_tableNumber;
	}

	public void SetTimePropRemaining(float _propLeft)
	{
		if (m_topWidgetTile != null)
		{
			m_topWidgetTile.SetProgress(_propLeft);
		}
		if (m_widgetAnimator != null)
		{
			m_widgetAnimator.SetBool(m_iWarning, _propLeft < 0.2f);
		}
	}

	public float GetTimePropRemaining()
	{
		if (m_topWidgetTile != null)
		{
			return m_topWidgetTile.GetProgress();
		}
		return 0f;
	}

	public void SetIsStuck()
	{
	}

	public Rect GetBounds()
	{
		if (m_recipeTiles.Count > 0)
		{
			RecipeWidgetTile recipeWidgetTile = m_recipeTiles[m_recipeTiles.Count - 1];
			if (recipeWidgetTile != null)
			{
				RectTransform rectTransform = recipeWidgetTile.gameObject.RequestComponent<RectTransform>();
				if (rectTransform != null)
				{
					return rectTransform.rect;
				}
			}
		}
		return default(Rect);
	}

	protected void OnPauseMenuVisibilityChange(BaseMenuBehaviour _menu)
	{
		Image[] array = base.gameObject.RequestComponentsRecursive<Image>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = !_menu.isActiveAndEnabled;
		}
	}

	public void SetupFromOrderDefinition(OrderDefinitionNode _data, int _tableNumber)
	{
		m_recipeTree = _data.m_orderGuiDescription;
		m_tableNumber = _tableNumber;
		StartCoroutine(RefreshSubObjectsAtEndOfFrame());
		if (T17InGameFlow.Instance != null)
		{
			T17InGameFlow.Instance.RegisterOnPauseMenuVisibilityChanged(OnPauseMenuVisibilityChange);
		}
	}

	private IEnumerator RefreshSubObjectsAtEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		RefreshSubElements();
	}

	protected override void OnCreateSubObjects(GameObject _container)
	{
		m_widgetAnimator = _container.AddComponent<Animator>();
		m_widgetAnimator.runtimeAnimatorController = m_widgetAnimatorController;
		m_recipeTiles.Clear();
		Size?[] array = BuildTileSizes();
		Position?[] array2 = BuildTilePositions(array);
		RecipeTileData recipeTileData = FindHead(m_recipeTree);
		for (int i = 0; i < m_recipeTree.Length; i++)
		{
			RecipeTileData recipeTileData2 = m_recipeTree[i];
			GameObject gameObject = GameObjectUtils.CreateOnParent<RectTransform>(_container, "Tile " + i);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			Size value = array[i].Value;
			Position value2 = array2[i].Value;
			Size sizey = CalculateSizeY(i);
			component.anchorMin = new Vector2((float)value2.X.GridTiles * 0.5f, (0f - (float)(value2.Y.GridTiles + sizey.GridTiles)) * 0.5f);
			component.anchorMax = new Vector2((float)(value2.X.GridTiles + value.GridTiles) * 0.5f, (0f - (float)value2.Y.GridTiles) * 0.5f);
			component.pivot = new Vector2(0f, 1f);
			component.offsetMin = new Vector2(value2.X.PixelOffset, 0f);
			component.offsetMax = new Vector2(value2.X.PixelOffset + value.PixelOffset, sizey.PixelOffset);
			component.localScale = Vector3.one;
			recipeTileData2.m_tileDefinition.m_largeIcon = recipeTileData2.m_children.Count > 0;
			RecipeWidgetTile recipeWidgetTile;
			if (recipeTileData2 == recipeTileData)
			{
				recipeWidgetTile = gameObject.AddComponent<TopRecipeWidgetTile>();
				recipeWidgetTile.SetupTile(recipeTileData2.m_tileDefinition, m_topDisplayConfig, value, sizey);
				m_topWidgetTile = recipeWidgetTile as TopRecipeWidgetTile;
				RectTransform rectTransform = _container.transform as RectTransform;
				rectTransform.pivot = VectorUtils.Select(0.5f * (component.anchorMin + component.anchorMax), component.anchorMax, true, false);
			}
			else
			{
				recipeWidgetTile = gameObject.AddComponent<RecipeWidgetTile>();
				recipeWidgetTile.SetupTile(recipeTileData2.m_tileDefinition, m_displayConfig, value, sizey);
			}
			m_recipeTiles.Add(recipeWidgetTile);
		}
		m_recipeTiles.Sort(delegate(RecipeWidgetTile _t1, RecipeWidgetTile _t2)
		{
			RectTransform rectTransform2 = _t1.transform as RectTransform;
			RectTransform rectTransform3 = _t2.transform as RectTransform;
			if (rectTransform3.anchorMin.y < rectTransform2.anchorMin.y)
			{
				return 1;
			}
			return (rectTransform3.anchorMin.y != rectTransform2.anchorMin.y) ? (-1) : 0;
		});
		for (int num = 0; num < m_recipeTiles.Count; num++)
		{
			m_recipeTiles[num].transform.SetSiblingIndex(num);
		}
		if (Application.isPlaying)
		{
			for (int num2 = 0; num2 < m_recipeTiles.Count; num2++)
			{
				m_recipeTiles[num2].RefreshSubElements();
			}
		}
		UI_Move uI_Move = _container.AddComponent<UI_Move>();
		uI_Move.m_UIMaterial = m_UIMoveMaterial;
		uI_Move.UpdateGraphics();
        // patch
        m_recipeTiles.Sort(delegate (RecipeWidgetTile _t1, RecipeWidgetTile _t2)
		{
			RectTransform rectTransform2 = _t1.transform as RectTransform;
			RectTransform rectTransform3 = _t2.transform as RectTransform;
			if (rectTransform3.anchorMin.y < rectTransform2.anchorMin.y)
			{
				return 1;
			}
			if (rectTransform3.anchorMin.y > rectTransform2.anchorMin.y)
			{
				return -1;
			}
			return string.Compare(rectTransform2.name, rectTransform3.name, StringComparison.Ordinal);
		});
		for (int num = 0; num < m_recipeTiles.Count; num++)
		{
			m_recipeTiles[num].transform.SetSiblingIndex(num);
		}
		RecipeWidgetTileFollow follow = gameObject.AddComponent<RecipeWidgetTileFollow>();
		follow.SetTiles(m_recipeTiles);
		follow.SetAnimator(m_widgetAnimator);
		// patch
    }

	public void Update()
	{
		if (m_animation == null)
		{
			return;
		}
		Color color = Color.white;
		m_animation.Advance(TimeManager.GetDeltaTime(base.gameObject));
		if (!m_animation.IsFinished())
		{
			color = m_animation.GetColourModifier();
		}
		else
		{
			m_animation = null;
		}
		Image[] array = base.gameObject.RequestComponentsRecursive<Image>();
		foreach (Image image in array)
		{
			Color color2 = Color.white;
			if (image.sprite == m_topDisplayConfig.m_background)
			{
				color2 = m_topDisplayConfig.m_tint;
			}
			else if (image.sprite == m_displayConfig.m_background)
			{
				color2 = m_displayConfig.m_tint;
			}
			else if (image.sprite == m_topDisplayConfig.lowTipSprite || image.sprite == m_topDisplayConfig.highTipSprite)
			{
				color2 = m_topDisplayConfig.notchColor;
			}
			image.color = color2 * color;
		}
	}

	private Size?[] BuildTileSizes()
	{
		Size?[] array = new Size?[m_recipeTree.Length];
		for (int i = 0; i < m_recipeTree.Length; i++)
		{
			CalculateSize(i, array);
		}
		return array;
	}

	private Position?[] BuildTilePositions(Size?[] _sizes)
	{
		Position?[] array = new Position?[m_recipeTree.Length];
		for (int i = 0; i < m_recipeTree.Length; i++)
		{
			CalculatePosition(i, array, _sizes);
		}
		return array;
	}

	private Size CalculateSize(int _tileIdx, Size?[] _cachedSizes)
	{
		RecipeTileData recipeTileData = m_recipeTree[_tileIdx];
		if (_cachedSizes[_tileIdx].HasValue)
		{
			return _cachedSizes[_tileIdx].Value;
		}
		Size size;
		if (recipeTileData.m_children.Count > 0)
		{
			size = new Size(0, 2f * m_displayConfig.m_levelInset + m_displayConfig.m_tileSpacing * (float)(recipeTileData.m_children.Count - 1));
			for (int i = 0; i < recipeTileData.m_children.Count; i++)
			{
				Size size2 = CalculateSize(recipeTileData.m_children[i], _cachedSizes);
				size.GridTiles += size2.GridTiles;
				size.PixelOffset += size2.PixelOffset;
			}
			size.GridTiles = Mathf.Max(size.GridTiles, 2);
		}
		else
		{
			int num = recipeTileData.m_tileDefinition.m_mainPictures.Count;
			if (recipeTileData.m_tileDefinition.m_modifierPictures != null)
			{
				num = Mathf.Max(num, recipeTileData.m_tileDefinition.m_modifierPictures.Count);
			}
			size = new Size(num, 2f * m_displayConfig.m_levelInset);
		}
		_cachedSizes[_tileIdx] = size;
		return size;
	}

	private Position CalculatePosition(int _tileIdx, Position?[] _cachedPositions, Size?[] _sizes)
	{
		RecipeTileData recipeTileData = m_recipeTree[_tileIdx];
		if (_cachedPositions[_tileIdx].HasValue)
		{
			return _cachedPositions[_tileIdx].Value;
		}
		int parentIndex = GetParentIndex(_tileIdx);
		Position position;
		if (parentIndex != -1)
		{
			position = CalculatePosition(parentIndex, _cachedPositions, _sizes);
			position.X.PixelOffset += m_displayConfig.m_levelInset;
			position.Y.GridTiles = position.Y.GridTiles + CalculateSizeY(parentIndex).GridTiles;
			RecipeTileData recipeTileData2 = m_recipeTree[parentIndex];
			for (int i = 0; i < recipeTileData2.m_children.Count; i++)
			{
				int num = recipeTileData2.m_children[i];
				if (num == _tileIdx)
				{
					break;
				}
				position.X.GridTiles += _sizes[num].Value.GridTiles;
				position.X.PixelOffset += _sizes[num].Value.PixelOffset + m_displayConfig.m_tileSpacing;
			}
		}
		else
		{
			position = new Position(new Size(0, 0f), new Size(-2, 0f));
		}
		_cachedPositions[_tileIdx] = position;
		return position;
	}

	private RecipeTileData FindHead(RecipeTileData[] _tileData)
	{
		bool[] array = new bool[_tileData.Length];
		for (int i = 0; i < _tileData.Length; i++)
		{
			for (int j = 0; j < _tileData[i].m_children.Count; j++)
			{
				array[_tileData[i].m_children[j]] = true;
			}
		}
		for (int k = 0; k < array.Length; k++)
		{
			if (!array[k])
			{
				return _tileData[k];
			}
		}
		return null;
	}

	private Size CalculateSizeY(int _tileIdx)
	{
		RecipeTileData recipeTileData = m_recipeTree[_tileIdx];
		bool flag = recipeTileData.m_children.Count > 0;
		bool flag2 = recipeTileData.m_tileDefinition.m_modifierPictures != null && recipeTileData.m_tileDefinition.m_modifierPictures.Count > 0;
		return new Size((!flag && !flag2) ? 1 : 2, m_displayConfig.m_yExtension);
	}

	private int GetParentIndex(int _childIdx)
	{
		for (int i = 0; i < m_recipeTree.Length; i++)
		{
			RecipeTileData recipeTileData = m_recipeTree[i];
			for (int j = 0; j < recipeTileData.m_children.Count; j++)
			{
				if (recipeTileData.m_children[j] == _childIdx)
				{
					return i;
				}
			}
		}
		return -1;
	}

	protected override void OnRefreshSubObjectProperties(GameObject _container)
	{
	}

	protected virtual void OnDestroy()
	{
		if (T17InGameFlow.Instance != null)
		{
			T17InGameFlow.Instance.UnRegisterOnPauseMenuVisibilityChanged(OnPauseMenuVisibilityChange);
		}
	}
}
