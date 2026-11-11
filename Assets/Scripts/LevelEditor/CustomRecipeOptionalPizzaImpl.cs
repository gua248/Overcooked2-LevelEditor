using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    public class CustomRecipeOptionalPizzaImpl : CustomRecipeOptionalImpl
    {
        protected CustomRecipeOptionalPizzaSO pizzaSO;

        public override void Init(ICustomRecipeOptional recipeSO)
        {
            base.Init(recipeSO);
            pizzaSO = this.recipeSO as CustomRecipeOptionalPizzaSO;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        public static void Register()
        {
            CustomRecipeOptionalRegister.Register<
                CustomRecipeOptionalPizzaSO, 
                CustomRecipeOptionalPizzaImpl>();
        }

        public override int GetMaxComposition()
        {
            return pizzaSO.ingredientContainerCapacity + 1;
        }

        public override OrderDefinitionNode GetOrderDefinitionNode()
        {
            CookedCompositeOrderNode node = ScriptableObject.CreateInstance<CookedCompositeOrderNode>();
            node.m_cookingStep = PseudoPrefabManager.LoadAsset<CookingStepData>(pizzaSO.cookingStepSO);
            node.m_progress = pizzaSO.cooked ? CookedCompositeOrderNode.CookingProgress.Cooked : CookedCompositeOrderNode.CookingProgress.Raw;
            node.m_composition = new OrderDefinitionNode[] { RecipeHelper.GetIngredientOrderNode(pizzaSO.doughSO) };
            node.m_optional = pizzaSO.optionalSOs.Select(x => RecipeHelper.GetOrderDefinitionNode(x)).ToArray();

            GameObject platingPrefabAsset = pizzaSO.GetModel();
            GameObject platingPrefab = RuntimePrefabManager.CloneAsInactivePrefab(platingPrefabAsset, clearOnRestart: false);
            PizzaCosmeticDecisions pizzaCosmetic = platingPrefab.GetComponent<PizzaCosmeticDecisions>();
            OrderToPrefabLookup uncookedLookup = GetOrderToPrefabLookupPizza(false, pizzaCosmetic);
            OrderToPrefabLookup cookedLookup = GetOrderToPrefabLookupPizza(true, pizzaCosmetic);
            pizzaCosmetic.GetType()
                .GetField("m_uncookedPrefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(pizzaCosmetic, uncookedLookup);
            pizzaCosmetic.GetType()
                .GetField("m_cookedPrefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(pizzaCosmetic, cookedLookup);

            node.m_platingPrefab = platingPrefab;

            node.name = pizzaSO.recipeName;
            node.m_uID = pizzaSO.uID;
            node.m_platingStep =
                pizzaSO.platingStepSO != null ?
                PseudoPrefabManager.LoadAsset<PlatingStepData>(pizzaSO.platingStepSO) : null;

            return node;
        }

        public override GameObject GetIngredientPrefab(GameObject originalPrefab)
        {
            OrderDefinitionNode node1 = RecipeHelper.GetIngredientOrderNode(pizzaSO.doughSO);
            OrderDefinitionNode node2 = RecipeHelper.GetIngredientOrderNode(originalPrefab);
            if (!node1.Equals(node2))
                return null;
            
            GameObject prefabAssetNext = originalPrefab.GetComponent<WorkableItem>().m_nextPrefab;
            GameObject doughPrefab = RuntimePrefabManager.CloneAsInactivePrefab(originalPrefab);
            GameObject doughPrefabNext = RuntimePrefabManager.CloneAsInactivePrefab(prefabAssetNext);

            doughPrefabNext.GetComponent<IngredientContainer>().m_capacity = pizzaSO.ingredientContainerCapacity;
            CookablePreparationContainer container = doughPrefabNext.GetComponent<CookablePreparationContainer>();
            PizzaCosmeticDecisions pizzaCosmeticDecisions = container.m_cosmeticsPrefab.GetComponent<PizzaCosmeticDecisions>();
            OrderToPrefabLookup uncookedLookup = GetOrderToPrefabLookupPizza(false, pizzaCosmeticDecisions);
            OrderToPrefabLookup cookedLookup = GetOrderToPrefabLookupPizza(true, pizzaCosmeticDecisions);
            container.m_containerRestrictions = uncookedLookup;
            GameObject cosmeticsPrefabAsset = container.m_cosmeticsPrefab;
            GameObject cosmeticsPrefab = RuntimePrefabManager.CloneAsInactivePrefab(cosmeticsPrefabAsset);
            PizzaCosmeticDecisions pizzaCosmetic = cosmeticsPrefab.GetComponent<PizzaCosmeticDecisions>();
            pizzaCosmetic.GetType()
                .GetField("m_uncookedPrefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(pizzaCosmetic, uncookedLookup);
            pizzaCosmetic.GetType()
                .GetField("m_cookedPrefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(pizzaCosmetic, cookedLookup);
            container.m_cosmeticsPrefab = cosmeticsPrefab;

            doughPrefab.GetComponent<WorkableItem>().m_nextPrefab = doughPrefabNext;
            return doughPrefab;
        }

        private OrderToPrefabLookup GetOrderToPrefabLookupPizza(bool cooked, PizzaCosmeticDecisions pizzaCosmeticDecisions)
        {
            var models = cooked ? pizzaSO.cookedPizzaIngredientPrefabs : pizzaSO.rawPizzaIngredientPrefabs;
            var modelSOs = cooked ? pizzaSO.cookedPizzaIngredientPrefabSOs : pizzaSO.rawPizzaIngredientPrefabSOs;
            OrderToPrefabLookup oldLookup = (OrderToPrefabLookup)pizzaCosmeticDecisions.GetType()
                .GetField(cooked ? "m_cookedPrefabLookup" : "m_uncookedPrefabLookup", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(pizzaCosmeticDecisions);
            OrderToPrefabLookup lookup = RecipeHelper.GetOrderToPrefabLookup(
                pizzaSO.recipeName,
                pizzaSO.optionalSOs,
                models, modelSOs, oldLookup);
            return lookup;
        }
    }
}