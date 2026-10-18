using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "LevelEditor/IngredientOrderNode", order = -1000)]
public class IngredientOrderNode : OrderDefinitionNode
{
	public Sprite m_iconSprite;

	public SubTexture2D m_crateLid;

	public Color m_colour = Color.white;

	public override AssembledDefinitionNode Convert()
	{
		return new IngredientAssembledNode(this);
	}
}
