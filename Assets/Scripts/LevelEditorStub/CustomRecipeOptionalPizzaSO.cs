using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/CustomRecipeOptionalPizzaSO")]
    public class CustomRecipeOptionalPizzaSO : CustomRecipeSO
    {
        [Header("Optional Pizza")]
        [SerializeField] public PseudoPrefabSO doughSO;
        [SerializeField] public int ingredientContainerCapacity;
        [SerializeField] public bool cooked;
        [SerializeField] public PseudoPrefabSO[] rawPizzaIngredientPrefabSOs;
        [SerializeField] public PseudoPrefabSO[] cookedPizzaIngredientPrefabSOs;
        [SerializeField] public GameObject[] rawPizzaIngredientPrefabs;
        [SerializeField] public GameObject[] cookedPizzaIngredientPrefabs;
    }
}