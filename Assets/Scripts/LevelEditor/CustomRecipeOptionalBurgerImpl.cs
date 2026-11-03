using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    public class CustomRecipeOptionalBurgerImpl : CustomRecipeOptionalImpl
    {
        protected CustomRecipeOptionalBurgerSO burgerSO;

        public override void Init(ICustomRecipeOptional recipeSO)
        {
            base.Init(recipeSO);
            burgerSO = this.recipeSO as CustomRecipeOptionalBurgerSO;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        public static void Register()
        {
            CustomRecipeOptionalRegister.Register<
                CustomRecipeOptionalBurgerSO, 
                CustomRecipeOptionalBurgerImpl>();
        }

        public override OrderDefinitionNode GetOrderDefinitionNode()
        {
            CompositeOrderNode node = ScriptableObject.CreateInstance<CompositeOrderNode>();
            node.m_composition = new OrderDefinitionNode[0];
            node.m_optional = burgerSO.optionalSOs
                .Where(x => x != burgerSO.bunSO)
                .Select(x => RecipeHelper.GetOrderDefinitionNode(x))
                .Concat(new OrderDefinitionNode[] { RecipeHelper.GetIngredientOrderNode(burgerSO.bunSO) })
                .ToArray();

            GameObject platingPrefabAsset = burgerSO.GetModel();
            GameObject platingPrefab = RuntimePrefabManager.CloneAsInactivePrefab(platingPrefabAsset, clearOnRestart: false);
            BurgerBunCosmeticDecisions burgerCosmetic = platingPrefab.GetComponent<BurgerBunCosmeticDecisions>();
            BurritoCosmeticDecisions burritoCosmetic = platingPrefab.GetComponent<BurritoCosmeticDecisions>();
            HotdogCosmeticDecisions hotdogCosmetic = platingPrefab.GetComponent<HotdogCosmeticDecisions>();
            if (burgerCosmetic != null)
            {
                OrderToPrefabLookup oldLookup = (OrderToPrefabLookup)typeof(BurgerBunCosmeticDecisions)
                    .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(burgerCosmetic);
                OrderToPrefabLookup lookup = GetOrderToPrefabLookupBurger(oldLookup);
                typeof(BurgerBunCosmeticDecisions)
                    .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(burgerCosmetic, lookup);
            }
            else if (burritoCosmetic != null)
            {
                OrderToPrefabLookup oldLookup = (OrderToPrefabLookup)typeof(OverlapModelsMealDecisions)
                    .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(burritoCosmetic);
                OrderToPrefabLookup lookup = GetOrderToPrefabLookupBurger(oldLookup);
                typeof(OverlapModelsMealDecisions)
                    .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(burritoCosmetic, lookup);
            }
            else if (hotdogCosmetic != null)
            {
                BurritoCosmeticDecisions newCosmetic = platingPrefab.AddComponent<BurritoCosmeticDecisions>();
                OrderToPrefabLookup oldLookup = (OrderToPrefabLookup)typeof(HotdogCosmeticDecisions)
                    .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(hotdogCosmetic);
                OrderToPrefabLookup lookup = GetOrderToPrefabLookupBurger(oldLookup);
                typeof(OverlapModelsMealDecisions)
                    .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(newCosmetic, lookup);
                OrderDefinitionNode m_emptyHotdogDefinition = (OrderDefinitionNode)typeof(HotdogCosmeticDecisions)
                    .GetField("m_emptyHotdogDefinition", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(hotdogCosmetic);
                typeof(BurritoCosmeticDecisions)
                    .GetField("m_tortillaOrderDefinition", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(newCosmetic, m_emptyHotdogDefinition);
                GameObject m_emptyBun = (GameObject)typeof(HotdogCosmeticDecisions)
                    .GetField("m_emptyBun", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(hotdogCosmetic);
                typeof(BurritoCosmeticDecisions)
                    .GetField("m_fullTortilla", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(newCosmetic, m_emptyBun);
                typeof(BurritoCosmeticDecisions)
                    .GetField("m_emptyTortilla", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(newCosmetic, m_emptyBun);
                Object.DestroyImmediate(hotdogCosmetic);
            }

            node.m_platingPrefab = platingPrefab;

            node.name = burgerSO.recipeName;
            node.m_uID = burgerSO.uID;
            node.m_platingStep =
                burgerSO.platingStepSO != null ?
                PseudoPrefabManager.LoadAsset<PlatingStepData>(burgerSO.platingStepSO) : null;

            return node;
        }

        public override GameObject GetIngredientPrefab(GameObject originalPrefab)
        {
            OrderDefinitionNode node1 = RecipeHelper.GetIngredientOrderNode(burgerSO.bunSO);
            OrderDefinitionNode node2 = RecipeHelper.GetIngredientOrderNode(originalPrefab);
            if (!node1.Equals(node2))
                return null;

            GameObject originalUnchoppedPrefab = null;
            if (originalPrefab.GetComponent<WorkableItem>() != null)
            {
                originalUnchoppedPrefab = originalPrefab;
                originalPrefab = originalPrefab.GetComponent<WorkableItem>().m_nextPrefab;
            }
            GameObject bunPrefab = RuntimePrefabManager.CloneAsInactivePrefab(originalPrefab);
            bunPrefab.GetComponent<IngredientContainer>().m_capacity = burgerSO.ingredientContainerCapacity;
            PreparationContainer container = bunPrefab.GetComponent<PreparationContainer>();
            OrderToPrefabLookup oldLookup = container.m_containerRestrictions;
            OrderToPrefabLookup lookup = GetOrderToPrefabLookupBurger(oldLookup);
            container.m_containerRestrictions = lookup;
            GameObject cosmeticsPrefabAsset = container.m_cosmeticsPrefab;
            GameObject cosmeticsPrefab = RuntimePrefabManager.CloneAsInactivePrefab(cosmeticsPrefabAsset);
            BurgerBunCosmeticDecisions burgerBunCosmetic = cosmeticsPrefab.GetComponent<BurgerBunCosmeticDecisions>();
            BurritoCosmeticDecisions burritoCosmetic = cosmeticsPrefab.GetComponent<BurritoCosmeticDecisions>();
            HotdogCosmeticDecisions hotdogCosmetic = cosmeticsPrefab.GetComponent<HotdogCosmeticDecisions>();
            if (burgerBunCosmetic != null)
            {
                typeof(BurgerBunCosmeticDecisions)
                    .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(burgerBunCosmetic, lookup);
            }
            else if (burritoCosmetic != null)
            {
                typeof(OverlapModelsMealDecisions)
                    .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(burritoCosmetic, lookup);
            }
            else if (hotdogCosmetic != null)
            {
                BurritoCosmeticDecisions newCosmetic = cosmeticsPrefab.AddComponent<BurritoCosmeticDecisions>();
                typeof(OverlapModelsMealDecisions)
                    .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(newCosmetic, lookup);
                OrderDefinitionNode m_emptyHotdogDefinition = (OrderDefinitionNode)typeof(HotdogCosmeticDecisions)
                    .GetField("m_emptyHotdogDefinition", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(hotdogCosmetic);
                typeof(BurritoCosmeticDecisions)
                    .GetField("m_tortillaOrderDefinition", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(newCosmetic, m_emptyHotdogDefinition);
                GameObject m_emptyBun = (GameObject)typeof(HotdogCosmeticDecisions)
                    .GetField("m_emptyBun", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(hotdogCosmetic);
                typeof(BurritoCosmeticDecisions)
                    .GetField("m_fullTortilla", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(newCosmetic, m_emptyBun);
                typeof(BurritoCosmeticDecisions)
                    .GetField("m_emptyTortilla", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(newCosmetic, m_emptyBun);
                Object.DestroyImmediate(hotdogCosmetic);
            }
            container.m_cosmeticsPrefab = cosmeticsPrefab;
            if (originalUnchoppedPrefab == null)
            {
                return bunPrefab;
            }
            else
            {
                GameObject unchoppedPrefab = RuntimePrefabManager.CloneAsInactivePrefab(originalUnchoppedPrefab);
                unchoppedPrefab.GetComponent<WorkableItem>().m_nextPrefab = bunPrefab;
                return unchoppedPrefab;
            }
        }

        private OrderToPrefabLookup GetOrderToPrefabLookupBurger(OrderToPrefabLookup oldLookup)
        {
            OrderToPrefabLookup lookup = RecipeHelper.GetOrderToPrefabLookup(
                burgerSO.recipeName,
                burgerSO.optionalSOs,
                burgerSO.ingredientModels, burgerSO.ingredientModelSOs, oldLookup);
            return lookup;
        }
    }
}