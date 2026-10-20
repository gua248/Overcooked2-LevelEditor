using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OC2DIYLevel.Modules.IngredientSplitterMod
{
    public class IngredientSplitter : MonoBehaviour
    {
        [SerializeField] public int capacity = 8;
        [SerializeField] public int splitNum = 2;
        [SerializeField] public CustomRecipeSO inputRecipe;
        [SerializeField] public GameObject outputPrefab;
        [SerializeField] public Transform[] ingredientModels;
        [SerializeField] public LevelInfoSO.GameOneShotAudioTag splitAudioTag = LevelInfoSO.GameOneShotAudioTag.PutDown;
    }
}