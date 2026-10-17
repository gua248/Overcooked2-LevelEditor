using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/CustomRecipeSO", order = -1000)]
    public class CustomRecipeSO : ScriptableObject
    {
        [Header("Basic")]
        [SerializeField] public RecipeType type;
        [SerializeField] public string recipeName;
        [SerializeField] public int uID;
        [SerializeField] public int score;
        [SerializeField] public PseudoPrefabSO platingStepSO;
        [SerializeField] public PseudoPrefabSO modelSO;
        [SerializeField] public GameObject model;
        [SerializeField] public PseudoPrefabSO iconSO;
        [SerializeField] public Sprite icon;

        [Header("Composite")]
        [SerializeField] public ScriptableObject[] compositionSOs; // CustomRecipeSO or PseudoPrefabSO
        [SerializeField] public ScriptableObject[] optionalSOs; // CustomRecipeSO or PseudoPrefabSO

        [Header("Cooked")]
        [SerializeField] public PseudoPrefabSO cookingStepSO;
        [SerializeField] public PseudoPrefabSO cookingStepIconSO;
        [SerializeField] public Sprite cookingStepIcon;
        [SerializeField] public CookingProgress cookingProgress;

        [Header("Mixed")]
        [SerializeField] public PseudoPrefabSO mixingIconSO;
        [SerializeField] public Sprite mixingIcon;
        [SerializeField] public MixingProgress mixingProgress;

        public enum RecipeType
        {
            Null,
            Composite,
            Cooked,
            Mixed,
            Ingredient,
            Item,
        }

        public enum CookingProgress
        {
            Raw,
            Cooked,
            Burnt
        }

        public enum MixingProgress
        {
            Unmixed,
            Mixed,
            OverMixed
        }
    }
}