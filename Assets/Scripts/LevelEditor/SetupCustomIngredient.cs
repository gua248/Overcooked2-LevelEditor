using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class SetupCustomIngredient : SetupCustomPrefab
    {
        public override void Setup()
        {
            base.Setup();
            SetupCustomIngredientStub ingredientStub = (SetupCustomIngredientStub)stub;
            GameObject ingredientsContentsUI = ingredientStub.ingredientContentUIPrefabSO == null ? null : PseudoPrefabManager.LoadAsset(ingredientStub.ingredientContentUIPrefabSO);
            GameObject throwParticle = ingredientStub.throwParticleSO == null ? null : PseudoPrefabManager.LoadAsset(ingredientStub.throwParticleSO);
            GameObject progressUIPrefab = ingredientStub.progressUIPrefabSO == null ? null : PseudoPrefabManager.LoadAsset(ingredientStub.progressUIPrefabSO);

            IngredientContentGUI ingredientContentGUI = GetComponent<IngredientContentGUI>();
            ThrowableItem throwableItem = GetComponent<ThrowableItem>();
            WorkableItem workableItem = GetComponent<WorkableItem>();

            if (ingredientContentGUI != null)
                ingredientContentGUI.m_ingredientContentUIPrefab = ingredientsContentsUI.GetComponent<IngredientContentsUIContainer>();
            if (throwableItem != null)
                throwableItem.m_throwParticle = throwParticle;
            if (workableItem != null)
                workableItem.m_progressUIPrefab = progressUIPrefab.GetComponent<ProgressUIController>();
        }

        public override void Clear()
        {
            base.Clear();
            IngredientContentGUI ingredientContentGUI = GetComponent<IngredientContentGUI>();
            ThrowableItem throwableItem = GetComponent<ThrowableItem>();
            WorkableItem workableItem = GetComponent<WorkableItem>();
            if (ingredientContentGUI != null)
                ingredientContentGUI.m_ingredientContentUIPrefab = null;
            if (throwableItem != null)
                throwableItem.m_throwParticle = null;
            if (workableItem != null)
                workableItem.m_progressUIPrefab = null;
        }
    }
}