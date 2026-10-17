using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class SetupCustomUtensil : SetupCustomPrefab
    {
        public override void Setup()
        {
            base.Setup();
            SetupCustomUtensilStub utensilStub = (SetupCustomUtensilStub)stub;
            GameObject ingredientsContentsUI = utensilStub.ingredientContentUIPrefabSO == null ? null : PseudoPrefabManager.LoadAsset(utensilStub.ingredientContentUIPrefabSO);
            GameObject sludge = utensilStub.sludgeSO == null ? null : PseudoPrefabManager.LoadAsset(utensilStub.sludgeSO);
            GameObject spawnEffect = utensilStub.spawnEffectSO == null ? null : PseudoPrefabManager.LoadAsset(utensilStub.spawnEffectSO);
            GameObject progressUIPrefab = utensilStub.progressUIPrefabSO == null ? null : PseudoPrefabManager.LoadAsset(utensilStub.progressUIPrefabSO);

            IngredientContentGUI ingredientContentGUI = GetComponent<IngredientContentGUI>();
            AttachedOrderCosmeticDecisions attachedOrderCosmeticDecisions = GetComponent<AttachedOrderCosmeticDecisions>();
            UtensilRespawnBehaviour utensilRespawnBehaviour = GetComponent<UtensilRespawnBehaviour>();
            Washable washable = GetComponent<Washable>();

            if (ingredientContentGUI != null)
                ingredientContentGUI.m_ingredientContentUIPrefab = ingredientsContentsUI.GetComponent<IngredientContentsUIContainer>();
            if (attachedOrderCosmeticDecisions != null)
                attachedOrderCosmeticDecisions.m_noMatchingRecipePrefab = sludge;
            if (utensilRespawnBehaviour != null)
                utensilRespawnBehaviour.m_spawnEffect = spawnEffect;
            if (washable != null)
                washable.m_progressUIPrefab = progressUIPrefab.GetComponent<ProgressUIController>();
        }

        public override void LateSetup()
        {
            base.LateSetup();
        }

        public override void Clear()
        {
            base.Clear();
            IngredientContentGUI ingredientContentGUI = GetComponent<IngredientContentGUI>();
            AttachedOrderCosmeticDecisions attachedOrderCosmeticDecisions = GetComponent<AttachedOrderCosmeticDecisions>();
            UtensilRespawnBehaviour utensilRespawnBehaviour = GetComponent<UtensilRespawnBehaviour>();
            Washable washable = GetComponent<Washable>();

            if (ingredientContentGUI != null)
                ingredientContentGUI.m_ingredientContentUIPrefab = null;
            if (attachedOrderCosmeticDecisions != null)
                attachedOrderCosmeticDecisions.m_noMatchingRecipePrefab = null;
            if (utensilRespawnBehaviour != null)
                utensilRespawnBehaviour.m_spawnEffect = null;
            if (washable != null)
                washable.m_progressUIPrefab = null;
        }
    }
}