using UnityEngine;

//[RequireComponent(typeof(IOrderDefinition))]
public class IngredientContentGUI : MonoBehaviour
{
	[SerializeField]
	public IngredientContentsUIContainer m_ingredientContentUIPrefab;

	[SerializeField]
	public bool m_displayEmptyElements;

	[SerializeField]
	public Vector3 m_Offset = Vector3.zero;
}
