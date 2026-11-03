using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public abstract class CustomRecipeOptionalImpl
    {
        protected CustomRecipeSO recipeSO;

        public virtual void Init(ICustomRecipeOptional recipeSO)
        {
            this.recipeSO = recipeSO as CustomRecipeSO;
        }

        public abstract OrderDefinitionNode GetOrderDefinitionNode();

        public abstract GameObject GetIngredientPrefab(GameObject originalPrefab);
    }
}