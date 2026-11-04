using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    public class CustomRecipeOptionalSandwichImpl : CustomRecipeOptionalImpl
    {
        protected CustomRecipeOptionalSandwichSO sandwichSO;

        public override void Init(ICustomRecipeOptional recipeSO)
        {
            base.Init(recipeSO);
            sandwichSO = this.recipeSO as CustomRecipeOptionalSandwichSO;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        public static void Register()
        {
            CustomRecipeOptionalRegister.Register<
                CustomRecipeOptionalSandwichSO,
                CustomRecipeOptionalSandwichImpl>();
        }

        public override OrderDefinitionNode GetOrderDefinitionNode()
        {
            CompositeOrderNode node = ScriptableObject.CreateInstance<CompositeOrderNode>();
            node.m_composition = new OrderDefinitionNode[0];
            var optionals = sandwichSO.optionalSOs
                .Where(x => x != sandwichSO.toastSO)
                .Select(x => RecipeHelper.GetOrderDefinitionNode(x));
            var sauces = sandwichSO.sauceSOs
                .Select(x => RecipeHelper.GetOrderDefinitionNode(x));
            OrderDefinitionNode toast = RecipeHelper.GetIngredientOrderNode(sandwichSO.toastSO);
            node.m_optional = optionals.Concat(sauces).Concat(new[] { toast }).ToArray();

            GameObject platingPrefabAsset = sandwichSO.GetModel();
            GameObject platingPrefab = RuntimePrefabManager.CloneAsInactivePrefab(platingPrefabAsset, clearOnRestart: false);
            ToastCosmeticDecisionsStub toastCosmeticDecisionsStub = platingPrefab.GetComponent<ToastCosmeticDecisionsStub>();
            if (toastCosmeticDecisionsStub != null)
            {
                ToastCosmeticDecisions toastCosmeticDecisions = platingPrefab.AddComponent<ToastCosmeticDecisions>();
                toastCosmeticDecisions.m_prefabLookup = RecipeHelper.GetOrderToPrefabLookup(
                    sandwichSO.recipeName,
                    sandwichSO.optionalSOs,
                    sandwichSO.ingredientModels, sandwichSO.ingredientModelSOs, null);
                toastCosmeticDecisions.m_sauceInPlatePrefabLookup = RecipeHelper.GetOrderToPrefabLookup(
                    sandwichSO.recipeName + "_SauceInPlate",
                    sandwichSO.sauceSOs,
                    sandwichSO.sauceInPlateModels, null, null);
                toastCosmeticDecisions.m_sauceOnToastPrefabLookup = RecipeHelper.GetOrderToPrefabLookup(
                    sandwichSO.recipeName + "_SauceOnToast",
                    sandwichSO.sauceSOs,
                    sandwichSO.sauceOnToastModels, null, null);
                toastCosmeticDecisions.m_wrongSauceInPlatePrefab = sandwichSO.wrongSauceInPlateModel;
                toastCosmeticDecisions.m_wrongSauceOnToastPrefab = sandwichSO.wrongSauceOnToastModel;
                toastCosmeticDecisions.m_bapOrderDefinition = toast;
            }

            node.m_platingPrefab = platingPrefab;

            node.name = sandwichSO.recipeName;
            node.m_uID = sandwichSO.uID;
            node.m_platingStep =
                sandwichSO.platingStepSO != null ?
                PseudoPrefabManager.LoadAsset<PlatingStepData>(sandwichSO.platingStepSO) : null;

            return node;
        }

        public override GameObject GetIngredientPrefab(GameObject originalPrefab)
        {
            OrderDefinitionNode node1 = RecipeHelper.GetIngredientOrderNode(sandwichSO.toastSO);
            OrderDefinitionNode node2 = RecipeHelper.GetIngredientOrderNode(originalPrefab);
            if (!node1.Equals(node2))
                return null;

            GameObject originalUnchoppedPrefab = null;
            if (originalPrefab.GetComponent<WorkableItem>() != null)
            {
                originalUnchoppedPrefab = originalPrefab;
                originalPrefab = originalPrefab.GetComponent<WorkableItem>().m_nextPrefab;
            }
            GameObject toastPrefab = RuntimePrefabManager.CloneAsInactivePrefab(originalPrefab);
            toastPrefab.GetComponent<IngredientContainer>().m_capacity = sandwichSO.ingredientContainerCapacity;
            PreparationContainer container = toastPrefab.GetComponent<PreparationContainer>();
            container.m_containerRestrictions = RecipeHelper.GetOrderToPrefabLookup(
                sandwichSO.recipeName,
                sandwichSO.optionalSOs.Concat(sandwichSO.sauceSOs).ToArray(),
                null, null, null);

            GameObject cosmeticsPrefabAsset = container.m_cosmeticsPrefab;
            GameObject cosmeticsPrefab = RuntimePrefabManager.CloneAsInactivePrefab(cosmeticsPrefabAsset);
            ToastCosmeticDecisionsStub toastCosmeticDecisionsStub = cosmeticsPrefab.GetComponent<ToastCosmeticDecisionsStub>();
            if (toastCosmeticDecisionsStub != null)
            {
                ToastCosmeticDecisions toastCosmeticDecisions = cosmeticsPrefab.AddComponent<ToastCosmeticDecisions>();
                toastCosmeticDecisions.m_prefabLookup = RecipeHelper.GetOrderToPrefabLookup(
                    sandwichSO.recipeName,
                    sandwichSO.optionalSOs,
                    sandwichSO.ingredientModels, sandwichSO.ingredientModelSOs, null);
                toastCosmeticDecisions.m_sauceInPlatePrefabLookup = RecipeHelper.GetOrderToPrefabLookup(
                    sandwichSO.recipeName + "_SauceInPlate",
                    sandwichSO.sauceSOs,
                    sandwichSO.sauceInPlateModels, null, null);
                toastCosmeticDecisions.m_sauceOnToastPrefabLookup = RecipeHelper.GetOrderToPrefabLookup(
                    sandwichSO.recipeName + "_SauceOnToast",
                    sandwichSO.sauceSOs,
                    sandwichSO.sauceOnToastModels, null, null);
                toastCosmeticDecisions.m_wrongSauceInPlatePrefab = sandwichSO.wrongSauceInPlateModel;
                toastCosmeticDecisions.m_wrongSauceOnToastPrefab = sandwichSO.wrongSauceOnToastModel;
                toastCosmeticDecisions.m_bapOrderDefinition = node1;
            }
            container.m_cosmeticsPrefab = cosmeticsPrefab;
            if (originalUnchoppedPrefab == null)
            {
                return toastPrefab;
            }
            else
            {
                GameObject unchoppedPrefab = RuntimePrefabManager.CloneAsInactivePrefab(originalUnchoppedPrefab);
                unchoppedPrefab.GetComponent<WorkableItem>().m_nextPrefab = toastPrefab;
                return unchoppedPrefab;
            }
        }
    }
}