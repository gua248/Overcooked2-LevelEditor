using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    public class CustomRecipeOptionalPlatterImpl : CustomRecipeOptionalImpl
    {
        protected CustomRecipeOptionalPlatterSO platterSO;

        public override void Init(ICustomRecipeOptional recipeSO)
        {
            base.Init(recipeSO);
            platterSO = this.recipeSO as CustomRecipeOptionalPlatterSO;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        public static void Register()
        {
            CustomRecipeOptionalRegister.Register<
                CustomRecipeOptionalPlatterSO,
                CustomRecipeOptionalPlatterImpl>();
        }

        public override OrderDefinitionNode GetOrderDefinitionNode()
        {
            CompositeOrderNode node = ScriptableObject.CreateInstance<CompositeOrderNode>();
            node.m_composition = new OrderDefinitionNode[0];
            node.m_optional = platterSO.optionalSOs
                .Select(x => RecipeHelper.GetOrderDefinitionNode(x))
                .ToArray();

            GameObject platingPrefabAsset = platterSO.GetModel();
            GameObject platingPrefab = RuntimePrefabManager.CloneAsInactivePrefab(platingPrefabAsset, clearOnRestart: false);
            PlatterCosmeticDecisionsStub platterCosmeticDecisionsStub = platingPrefab.GetComponent<PlatterCosmeticDecisionsStub>();
            if (platterCosmeticDecisionsStub != null)
            {
                PlatterCosmeticDecisions platterCosmeticDecisions = platingPrefab.AddComponent<PlatterCosmeticDecisions>();
                platterCosmeticDecisions.Init(platterSO);
                OrderToPrefabLookup lookup = RecipeHelper.GetOrderToPrefabLookup(
                    platterSO.recipeName,
                    platterSO.optionalSOs,
                    platterSO.ingredientModels, platterSO.ingredientModelSOs, null);
                typeof(OverlapModelsMealDecisions)
                    .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(platterCosmeticDecisions, lookup);
            }

            node.m_platingPrefab = platingPrefab;

            node.name = platterSO.recipeName;
            node.m_uID = platterSO.uID;
            node.m_platingStep =
                platterSO.platingStepSO != null ?
                PseudoPrefabManager.LoadAsset<PlatingStepData>(platterSO.platingStepSO) : null;

            return node;
        }
    }
}