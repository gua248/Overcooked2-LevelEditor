using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/CustomRecipeOptionalBurgerSO", order = -1000)]
    public class CustomRecipeOptionalBurgerSO : CustomRecipeSO, ICustomRecipeOptional
    {
        [Header("Optional Burger")]
        [SerializeField] public PseudoPrefabSO bunSO;
        [SerializeField] public int ingredientContainerCapacity;
        [SerializeField] public PseudoPrefabSO[] ingredientModelSOs;
        [SerializeField] public GameObject[] ingredientModels;
    }
}
