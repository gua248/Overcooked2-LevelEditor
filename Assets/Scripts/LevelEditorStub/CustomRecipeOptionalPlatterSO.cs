using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/CustomRecipeOptionalPlatterSO", order = -1000)]
    public class CustomRecipeOptionalPlatterSO : CustomRecipeSO, ICustomRecipeOptional
    {
        [Header("Optional Platter")]
        [SerializeField] public PseudoPrefabSO[] ingredientModelSOs;
        [SerializeField] public GameObject[] ingredientModels;
        [SerializeField] public CosmeticsType cosmeticsType;
        [SerializeField] public Vector3 offset = new Vector3(0.3f, 0.05f, 0f);

        public enum CosmeticsType
        {
            Overlap,
            Separated,
        }
    }
}
