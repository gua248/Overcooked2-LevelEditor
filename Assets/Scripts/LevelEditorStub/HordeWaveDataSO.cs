using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/HordeWaveDataSO", order = -1000)]
    public class HordeWaveDataSO : ScriptableObject
    {
        [SerializeField] public RecipeListSO recipeListSO;
        [SerializeField] public int intervalSeconds;
        [SerializeField] public PseudoPrefabSO[] enemies;
        [SerializeField] public int[] spawnTimes;
    }
}