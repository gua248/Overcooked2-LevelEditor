using LevelEditorStub;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace LevelEditor
{
    public static class RecipeHelper
    {
        public static RecipeList.Entry GetRecipe(ScriptableObject recipeSO, bool useScore2)
        {
            if (recipeSO is PseudoPrefabSORecipe)
            {
                PseudoPrefabSORecipe x = (PseudoPrefabSORecipe)recipeSO;
                var entry = new RecipeList.Entry();
                entry.m_weight = 1f; // this is unused in the game
                entry.m_scoreForMeal = useScore2 ? x.score2 : x.score1;
                entry.m_order = PseudoPrefabManager.LoadAsset<OrderDefinitionNode>(x);
                return entry;
            }
            else if (recipeSO is CustomRecipeSO)
            {
                CustomRecipeSO customRecipeSO = (CustomRecipeSO)recipeSO;
                var entry = new RecipeList.Entry();
                entry.m_weight = 1f; // this is unused in the game
                entry.m_scoreForMeal = useScore2 ? customRecipeSO.score2 : customRecipeSO.score1;
                entry.m_order = GetRecipeNode(customRecipeSO);
                return entry;
            }
            else return new RecipeList.Entry();
        }

        public static OrderDefinitionNode GetRecipeNode(CustomRecipeSO customRecipeSO)
        {
            OrderDefinitionNode recipe;
            if (customRecipeSO.cookingStepSO != null)
            {
                CookedCompositeOrderNode node = ScriptableObject.CreateInstance<CookedCompositeOrderNode>();
                node.m_cookingStep = PseudoPrefabManager.LoadAsset<CookingStepData>(customRecipeSO.cookingStepSO);
                IngredientOrderNode[] ingredientOrderNodes = customRecipeSO.compositionSOs.Select(x => GetIngredientOrderNode(x as PseudoPrefabSO)).ToArray();
                node.m_composition = ingredientOrderNodes;
                node.m_progress = CookedCompositeOrderNode.CookingProgress.Cooked;

                RecipeWidgetUIController.RecipeTileData gui1 = new RecipeWidgetUIController.RecipeTileData();
                gui1.m_tileDefinition = new RecipeWidgetTile.TileDefinition();
                gui1.m_tileDefinition.m_mainPictures = new List<Sprite> {
                    customRecipeSO.icon != null ? customRecipeSO.icon :
                    customRecipeSO.iconSO != null ? PseudoPrefabManager.LoadSpriteAsset(customRecipeSO.iconSO) : null };
                gui1.m_children = new List<int> { 1 };
                RecipeWidgetUIController.RecipeTileData gui2 = new RecipeWidgetUIController.RecipeTileData();
                gui2.m_tileDefinition = new RecipeWidgetTile.TileDefinition();
                gui2.m_tileDefinition.m_mainPictures = ingredientOrderNodes.Select(x => x.m_iconSprite).ToList();
                gui2.m_tileDefinition.m_modifierPictures = new List<Sprite> {
                    customRecipeSO.cookingStepIcon != null ? customRecipeSO.cookingStepIcon :
                    customRecipeSO.cookingStepIconSO != null ? PseudoPrefabManager.LoadSpriteAsset(customRecipeSO.cookingStepIconSO) : null };
                node.m_orderGuiDescription = new RecipeWidgetUIController.RecipeTileData[] { gui1, gui2 };
                recipe = node;
            }
            else
            {
                CompositeOrderNode node = ScriptableObject.CreateInstance<CompositeOrderNode>();
                OrderDefinitionNode[] orderNodes = customRecipeSO.compositionSOs.Select(
                    x => x is CustomRecipeSO ? 
                    GetRecipeNode(x as CustomRecipeSO) : 
                    GetIngredientOrderNode(x as PseudoPrefabSO)).ToArray();
                node.m_composition = orderNodes;

                RecipeWidgetUIController.RecipeTileData gui1 = new RecipeWidgetUIController.RecipeTileData();
                gui1.m_tileDefinition = new RecipeWidgetTile.TileDefinition();
                gui1.m_tileDefinition.m_mainPictures = new List<Sprite> {
                    customRecipeSO.icon != null ? customRecipeSO.icon :
                    customRecipeSO.iconSO != null ? PseudoPrefabManager.LoadSpriteAsset(customRecipeSO.iconSO) : null };
                gui1.m_children = Enumerable.Range(1, orderNodes.Length).ToList();

                RecipeWidgetUIController.RecipeTileData[] children = new RecipeWidgetUIController.RecipeTileData[orderNodes.Length];
                for (int i = 0; i < orderNodes.Length; i++)
                {
                    if (customRecipeSO.compositionSOs[i] is CustomRecipeSO)
                    {
                        children[i] = orderNodes[i].m_orderGuiDescription[1];
                    }
                    else if (orderNodes[i] is IngredientOrderNode)
                    {
                        RecipeWidgetUIController.RecipeTileData gui2 = new RecipeWidgetUIController.RecipeTileData();
                        gui2.m_tileDefinition = new RecipeWidgetTile.TileDefinition();
                        gui2.m_tileDefinition.m_mainPictures = new List<Sprite> { (orderNodes[i] as IngredientOrderNode).m_iconSprite };
                        children[i] = gui2;
                    }
                    else
                    {
                    }
                }
                node.m_orderGuiDescription = new List<RecipeWidgetUIController.RecipeTileData> { gui1 }.Concat(children).ToArray();
                recipe = node;
            }

            recipe.name = customRecipeSO.recipeName;
            recipe.m_uID = customRecipeSO.uID;
            recipe.m_platingPrefab =
                customRecipeSO.model != null ? customRecipeSO.model :
                customRecipeSO.modelSO != null ? PseudoPrefabManager.LoadAsset(customRecipeSO.modelSO) : null;
            recipe.m_platingStep = 
                customRecipeSO.platingStepSO != null ? 
                PseudoPrefabManager.LoadAsset<PlatingStepData>(customRecipeSO.platingStepSO) : null;

            return recipe;
        }

        private static IngredientOrderNode GetIngredientOrderNode(PseudoPrefabSO pseudoPrefabSO)
        {
            GameObject ingredient = PseudoPrefabManager.LoadAsset<GameObject>(pseudoPrefabSO);
            while (ingredient.GetComponent<WorkableItem>() != null)
                ingredient = ingredient.GetComponent<WorkableItem>().m_nextPrefab;
            IngredientPropertiesComponent ingredientPropertiesComponent = ingredient.GetComponent<IngredientPropertiesComponent>();
            if (ingredientPropertiesComponent != null)
            {
                return (IngredientOrderNode)ingredientPropertiesComponent.GetType()
                    .GetField("m_ingredientOrderNode", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(ingredientPropertiesComponent);
            }
            else
            {
                PreparationContainer preparationContainer = ingredient.GetComponent<PreparationContainer>();
                return preparationContainer.m_ingredientOrderNode;
            }
        }
    }
}
