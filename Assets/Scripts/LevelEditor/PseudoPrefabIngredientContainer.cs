using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelEditorStub;


namespace LevelEditor
{
	public class PseudoPrefabIngredientContainer : PseudoPrefab
	{
        public override void Setup()
        {
            PseudoPrefabIngredientContainerStub ingredientContainerStub = (PseudoPrefabIngredientContainerStub)stub;
            IngredientContainer ingredientContainer = childGameObject.GetComponent<IngredientContainer>();
            ingredientContainer.m_capacity = ingredientContainerStub.capacity;
        }
    }
}