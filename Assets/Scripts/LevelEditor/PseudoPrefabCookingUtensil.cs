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

                GameObject m_prefab = ((OrderToPrefabLookup.ContentPrefabLookup[])oldLookup.GetType()
                    .GetField("m_lookupArray", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(oldLookup))[0].m_prefab;
                OrderToPrefabLookup.ContentPrefabLookup[] allowedIngredients = ingredientContainerStub.allowedIngredientSOs.Select(x =>
                {
                    GameObject ingredient = PseudoPrefabManager.LoadAsset<GameObject>(x);
                    while (ingredient.GetComponent<WorkableItem>() != null)
                        ingredient = ingredient.GetComponent<WorkableItem>().m_nextPrefab;
                    IngredientPropertiesComponent ingredientPropertiesComponent = ingredient.GetComponent<IngredientPropertiesComponent>();
                    return (IngredientOrderNode)ingredientPropertiesComponent.GetType()
                        .GetField("m_ingredientOrderNode", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                        .GetValue(ingredientPropertiesComponent);
                }).Select(x => new OrderToPrefabLookup.ContentPrefabLookup()
                {
                    m_content = x,
                    m_prefab = m_prefab,
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