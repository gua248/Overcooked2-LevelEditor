using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    [CreateAssetMenu(menuName = "LevelEditor/RecipeListSO", order = -1000)]
    public class RecipeListSO : ScriptableObject
    {
        [SerializeField] public ScriptableObject[] recipes;
    }
}