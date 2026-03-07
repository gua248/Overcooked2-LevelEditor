using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace LevelEditor
{
	public class PseudoPrefabCookingUtensil : PseudoPrefab
	{
        public override void Setup()
        {
            PseudoPrefabCookingUtensilStub ingredientContainerStub = (PseudoPrefabCookingUtensilStub)stub;
            IngredientContainer ingredientContainer = childGameObject.GetComponent<IngredientContainer>();
            ingredientContainer.m_capacity = ingredientContainerStub.capacity;
            if (ingredientContainerStub.allowedIngredientSOs != null &&
                ingredientContainerStub.allowedIngredientSOs.Length > 0)
            {
                CookableContainer cookableContainer = childGameObject.GetComponent<CookableContainer>();
                OrderToPrefabLookup oldLookup = cookableContainer.m_approvedContentsList;
                OrderToPrefabLookup newLookup = ScriptableObject.Instantiate(oldLookup);

                var oldLookupArray = (OrderToPrefabLookup.ContentPrefabLookup[])oldLookup.GetType()
                    .GetField("m_lookupArray", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(oldLookup);
                GameObject m_prefab_default = oldLookupArray[0].m_prefab;
                OrderToPrefabLookup.ContentPrefabLookup[] allowedIngredients = ingredientContainerStub.allowedIngredientSOs.SelectMany(x =>
                {
                    GameObject ingredient = PseudoPrefabManager.LoadAsset<GameObject>(x);
                    while (ingredient.GetComponent<WorkableItem>() != null)
                        ingredient = ingredient.GetComponent<WorkableItem>().m_nextPrefab;
                    IngredientPropertiesComponent ingredientPropertiesComponent = ingredient.GetComponent<IngredientPropertiesComponent>();
                    IngredientOrderNode ingredientOrderNode = (IngredientOrderNode)ingredientPropertiesComponent.GetType()
                        .GetField("m_ingredientOrderNode", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                        .GetValue(ingredientPropertiesComponent);
                    var allLookup = oldLookupArray.Where(y =>
                    {
                        if (y.m_content == ingredientOrderNode) return true;
                        if (y.m_content is CookedCompositeOrderNode)
                        {
                            CookedCompositeOrderNode cookedCompositeOrderNode = (CookedCompositeOrderNode)y.m_content;
                            return cookedCompositeOrderNode.m_composition.Length == 1 && cookedCompositeOrderNode.m_composition[0] == ingredientOrderNode;
                        }
                        return false;
                    });
                    if (allLookup.Any())
                    {
                        return allLookup;
                    }
                    else
                    {
                        var lookup = new OrderToPrefabLookup.ContentPrefabLookup()
                        {
                            m_content = ingredientOrderNode,
                            m_prefab = m_prefab_default,
                        };
                        return new OrderToPrefabLookup.ContentPrefabLookup[] { lookup };
                    }
                }).ToArray();
                newLookup.GetType()
                    .GetField("m_lookupArray", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(newLookup, allowedIngredients);
                cookableContainer.m_approvedContentsList = newLookup;
            }

            var contentsCosmeticDecisions = childGameObject.RequestComponentRecursive<ContentsCosmeticDecisions>();
            if (contentsCosmeticDecisions != null)
            {
                contentsCosmeticDecisions.m_contentsYPositionWhenEmpty = -0.2f;
            }
        }
    }
}