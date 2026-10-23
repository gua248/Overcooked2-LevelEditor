using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    public class TutorialRecipeGUIStub : Stub
    {
        [SerializeField] public int distanceFromLeft = 0;
        [SerializeField] public int distanceBetween = 5;
        [SerializeField] public PseudoPrefabSO recipeUIPrefabSO;
        [SerializeField] public CustomRecipeSO[] recipes;
    }
}