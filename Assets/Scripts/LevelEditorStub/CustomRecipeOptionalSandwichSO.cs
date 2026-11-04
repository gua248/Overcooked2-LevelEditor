using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/CustomRecipeOptionalSandwichSO", order = -1000)]
    public class CustomRecipeOptionalSandwichSO : CustomRecipeSO, ICustomRecipeOptional
    {
        [Header("Optional Sandwich")]
        [SerializeField] public PseudoPrefabSO toastSO;
        [SerializeField] public int ingredientContainerCapacity;
        [SerializeField] public PseudoPrefabSO[] ingredientModelSOs;
        [SerializeField] public GameObject[] ingredientModels;

        [SerializeField] public ScriptableObject[] sauceSOs;
        [SerializeField] public GameObject[] sauceInPlateModels;
        [SerializeField] public GameObject[] sauceOnToastModels;
        [SerializeField] public GameObject wrongSauceInPlateModel;
        [SerializeField] public GameObject wrongSauceOnToastModel;
    }
}
