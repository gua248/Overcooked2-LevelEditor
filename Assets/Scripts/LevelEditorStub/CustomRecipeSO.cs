using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/CustomRecipeSO")]
    public class CustomRecipeSO : ScriptableObject
    {
        [SerializeField] public ScriptableObject[] compositionSOs; // CustomRecipeSO or PseudoPrefabSO
        [SerializeField] public PseudoPrefabSO cookingStepSO;
        [SerializeField] public PseudoPrefabSO cookingStepIconSO;
        [SerializeField] public Sprite cookingStepIcon;
        [SerializeField] public PseudoPrefabSO platingStepSO;
        [SerializeField] public PseudoPrefabSO modelSO;
        [SerializeField] public GameObject model;
        [SerializeField] public PseudoPrefabSO iconSO;
        [SerializeField] public Sprite icon;

        [SerializeField] public string recipeName;
        [SerializeField] public int uID;

        [SerializeField] public int score1;
        [SerializeField] public int score2;
    }
}