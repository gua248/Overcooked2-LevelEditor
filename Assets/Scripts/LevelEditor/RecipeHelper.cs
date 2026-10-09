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
        public static RecipeList.Entry GetRecipe(ScriptableObject recipeSO)
        {
            if (recipeSO is PseudoPrefabSORecipe)
            {
                PseudoPrefabSORecipe x = (PseudoPrefabSORecipe)recipeSO;
                var entry = new RecipeList.Entry();
                entry.m_weight = 1f; // this is unused in the game
                entry.m_scoreForMeal = x.score;
                entry.m_order = PseudoPrefabManager.LoadAsset<OrderDefinitionNode>(x);
                return entry;
            }
            else if (recipeSO is CustomRecipeSO)
            {
                CustomRecipeSO customRecipeSO = (CustomRecipeSO)recipeSO;
                var entry = new RecipeList.Entry();
                entry.m_weight = 1f; // this is unused in the game
                entry.m_scoreForMeal = customRecipeSO.score;
                entry.m_order = GetOrderDefinitionNodeCustomRecipe(customRecipeSO);
                entry.m_order.m_orderGuiDescription = GetGuiDescriptionCustomRecipe(customRecipeSO, entry.m_order);
                return entry;
            }
            else return new RecipeList.Entry();
        }

        public static OrderDefinitionNode GetOrderDefinitionNode(ScriptableObject recipeSO)
        {
            if (recipeSO is PseudoPrefabSORecipe)
                return PseudoPrefabManager.LoadAsset<OrderDefinitionNode>(recipeSO as PseudoPrefabSORecipe);
            else if (recipeSO is CustomRecipeSO)
                return GetOrderDefinitionNodeCustomRecipe(recipeSO as CustomRecipeSO);
            else if (recipeSO is PseudoPrefabSO)
                return GetIngredientOrderNode(recipeSO as PseudoPrefabSO);
            else return null;
        }

        public static OrderDefinitionNode GetOrderDefinitionNodeCustomRecipe(CustomRecipeSO customRecipeSO)
        {
            FixOldCustomRecipeSO(customRecipeSO);

            OrderDefinitionNode recipe;
            switch (customRecipeSO.type)
            {
                case CustomRecipeSO.RecipeType.Composite:
                    recipe = ScriptableObject.CreateInstance<CompositeOrderNode>();
                    break;
                case CustomRecipeSO.RecipeType.Cooked:
                    recipe = ScriptableObject.CreateInstance<CookedCompositeOrderNode>();
                    break;
                case CustomRecipeSO.RecipeType.Mixed:
                    recipe = ScriptableObject.CreateInstance<MixedCompositeOrderNode>();
                    break;
                default:
                    recipe = ScriptableObject.CreateInstance<WildcardOrderNode>();
                    break;
            }

            if (customRecipeSO.type == CustomRecipeSO.RecipeType.Composite ||
                customRecipeSO.type == CustomRecipeSO.RecipeType.Cooked ||
                customRecipeSO.type == CustomRecipeSO.RecipeType.Mixed)
            {
                CompositeOrderNode compositeOrderNode = recipe as CompositeOrderNode;
                if (customRecipeSO.compositionSOs != null)
                {
                    compositeOrderNode.m_composition = customRecipeSO.compositionSOs
                        .Select(x => GetOrderDefinitionNode(x)).ToArray();
                }
                if (customRecipeSO.optionalSOs != null)
                {
                    compositeOrderNode.m_optional = customRecipeSO.optionalSOs
                        .Select(x => GetOrderDefinitionNode(x)).ToArray();
                }
            }

            if (customRecipeSO.type == CustomRecipeSO.RecipeType.Cooked)
            {
                CookedCompositeOrderNode cookedCompositeOrderNode = recipe as CookedCompositeOrderNode;
                cookedCompositeOrderNode.m_cookingStep = PseudoPrefabManager.LoadAsset<CookingStepData>(customRecipeSO.cookingStepSO);
                cookedCompositeOrderNode.m_progress = (CookedCompositeOrderNode.CookingProgress)customRecipeSO.cookingProgress;
            }

            if (customRecipeSO.type == CustomRecipeSO.RecipeType.Mixed)
            {
                MixedCompositeOrderNode mixedCompositeOrderNode = recipe as MixedCompositeOrderNode;
                mixedCompositeOrderNode.m_progress = (MixedCompositeOrderNode.MixingProgress)customRecipeSO.mixingProgress;
            }

            recipe.name = customRecipeSO.recipeName;
            recipe.m_uID = customRecipeSO.uID;
            recipe.m_platingStep = customRecipeSO.platingStepSO != null ? PseudoPrefabManager.LoadAsset<PlatingStepData>(customRecipeSO.platingStepSO) : null;
            recipe.m_platingPrefab = customRecipeSO.GetModel();
            return recipe;
        }

        private static RecipeWidgetUIController.RecipeTileData[] GetGuiDescriptionCustomRecipe(CustomRecipeSO customRecipeSO, OrderDefinitionNode recipe)
        {
            FixOldCustomRecipeSO(customRecipeSO);

            RecipeWidgetUIController.RecipeTileData gui0 = new RecipeWidgetUIController.RecipeTileData();
            gui0.m_tileDefinition = new RecipeWidgetTile.TileDefinition();
            gui0.m_tileDefinition.m_mainPictures = new List<Sprite> { customRecipeSO.GetIcon() };

            switch (customRecipeSO.type)
            {
                case CustomRecipeSO.RecipeType.Composite:
                    RecipeWidgetUIController.RecipeTileData[] children = new RecipeWidgetUIController.RecipeTileData[customRecipeSO.compositionSOs.Length];
                    CompositeOrderNode compositeOrderNode = recipe as CompositeOrderNode;
                    for (int i = 0; i < customRecipeSO.compositionSOs.Length; i++)
                    {
                        children[i] = new RecipeWidgetUIController.RecipeTileData();
                        children[i].m_tileDefinition = GetTileDefinition(customRecipeSO.compositionSOs[i], compositeOrderNode.m_composition[i]);
                    }
                    gui0.m_children = Enumerable.Range(1, children.Length).ToList();
                    return new List<RecipeWidgetUIController.RecipeTileData> { gui0 }.Concat(children).ToArray();
                case CustomRecipeSO.RecipeType.Cooked:
                case CustomRecipeSO.RecipeType.Mixed:
                    RecipeWidgetUIController.RecipeTileData gui1 = new RecipeWidgetUIController.RecipeTileData();
                    gui1.m_tileDefinition = GetTileDefinition(customRecipeSO, recipe);
                    gui0.m_children = new List<int> { 1 };
                    return new RecipeWidgetUIController.RecipeTileData[] { gui0, gui1 };
                default:
                    return new RecipeWidgetUIController.RecipeTileData[] { gui0 };
            }
        }

        private static List<Sprite> GetSpriteList(ScriptableObject recipeSO, OrderDefinitionNode recipe, out int cookingStepCount)
        {
            if (recipeSO is PseudoPrefabSORecipe)
            {
                return GetSpriteListPseudoRecipe(recipeSO as PseudoPrefabSORecipe, recipe, out cookingStepCount);
            }
            else if (recipeSO is CustomRecipeSO)
            {
                CustomRecipeSO customRecipeSO = recipeSO as CustomRecipeSO;
                List<Sprite> list = new List<Sprite>();
                if (customRecipeSO.type == CustomRecipeSO.RecipeType.Null || !(recipe is CompositeOrderNode))
                {
                    cookingStepCount = 0;
                    return list;
                }
                CompositeOrderNode compositeOrderNode = recipe as CompositeOrderNode;
                cookingStepCount = 0;
                for (int i = 0; i < customRecipeSO.compositionSOs.Length; i++)
                {
                    list.AddRange(GetSpriteList(customRecipeSO.compositionSOs[i], compositeOrderNode.m_composition[i], out cookingStepCount));
                }
                if (customRecipeSO.type == CustomRecipeSO.RecipeType.Cooked || customRecipeSO.type == CustomRecipeSO.RecipeType.Mixed)
                {
                    cookingStepCount += 1;
                    if (customRecipeSO.compositionSOs.Length > 1)
                        cookingStepCount = 1;
                }
                if (customRecipeSO.type == CustomRecipeSO.RecipeType.Cooked)
                    list.Add(customRecipeSO.GetCookingStepIcon());
                else if (customRecipeSO.type == CustomRecipeSO.RecipeType.Mixed)
                    list.Add(customRecipeSO.GetMixingIcon());
                return list;
            }
            else if (recipeSO is PseudoPrefabSO && recipe is IngredientOrderNode)
            {
                cookingStepCount = 0;
                return new List<Sprite> { (recipe as IngredientOrderNode).m_iconSprite };
            }
            else
            {
                cookingStepCount = 0;
                return new List<Sprite>();
            }
        }
        
        private static List<Sprite> GetSpriteListPseudoRecipe(PseudoPrefabSORecipe recipeSO, OrderDefinitionNode recipe, out int cookingStepCount)
        {
            List<Sprite> sprites = new List<Sprite>();
            cookingStepCount = 0;
            if (!recipe.m_orderGuiDescription.IsEmpty())
            {
                foreach (RecipeWidgetUIController.RecipeTileData tile in recipe.m_orderGuiDescription.Skip(1))
                {
                    if (tile.m_tileDefinition.m_mainPictures != null)
                        sprites.AddRange(tile.m_tileDefinition.m_mainPictures);
                    if (tile.m_tileDefinition.m_modifierPictures != null)
                        sprites.AddRange(tile.m_tileDefinition.m_modifierPictures);
                }
                if (recipe.m_orderGuiDescription.Length == 2)
                    cookingStepCount = recipe.m_orderGuiDescription[1].m_tileDefinition.m_modifierPictures.Count;
            }
            else
            {
                MixedCompositeOrderNode mixedCompositeOrderNode = recipe as MixedCompositeOrderNode;
                if (mixedCompositeOrderNode != null)
                {
                    // mixed or smoothie
                    sprites.AddRange(mixedCompositeOrderNode.m_composition.Select(x => (x as IngredientOrderNode).m_iconSprite));
                    if (recipe.m_platingStep == null && recipe.m_platingPrefab == null)
                    {
                        // mixed
                        PseudoPrefabSO pseudoPrefabSO = ScriptableObject.CreateInstance<PseudoPrefabSO>();
                        pseudoPrefabSO.bundleName = "bundle18";
                        pseudoPrefabSO.assetPath = "Assets/data/recipedata/cookingstepdata/icons/Mixer.png";
                        Sprite icon = PseudoPrefabManager.LoadSpriteSubAsset(pseudoPrefabSO);
                        sprites.Add(icon);
                        UnityEngine.Object.DestroyImmediate(pseudoPrefabSO);
                        cookingStepCount = 1;
                    }
                    else
                    {
                        // smoothie
                        PseudoPrefabSO pseudoPrefabSO = ScriptableObject.CreateInstance<PseudoPrefabSO>();
                        pseudoPrefabSO.bundleName = "bundle158";
                        pseudoPrefabSO.assetPath = "Assets/data/recipedata/cookingstepdata/icons/Blender.png";
                        Sprite icon = PseudoPrefabManager.LoadSpriteSubAsset(pseudoPrefabSO);
                        sprites.Add(icon);
                        UnityEngine.Object.DestroyImmediate(pseudoPrefabSO);
                        cookingStepCount = 1;
                    }
                }
            }
            return sprites;
        }

        private static RecipeWidgetTile.TileDefinition GetTileDefinition(ScriptableObject recipeSO, OrderDefinitionNode recipe)
        {
            RecipeWidgetTile.TileDefinition tileDefinition = new RecipeWidgetTile.TileDefinition();
            int cookingStepCount;
            List<Sprite> sprites = GetSpriteList(recipeSO, recipe, out cookingStepCount);
            tileDefinition.m_mainPictures = sprites.GetRange(0, sprites.Count - cookingStepCount);
            tileDefinition.m_modifierPictures = sprites.GetRange(sprites.Count - cookingStepCount, cookingStepCount);
            return tileDefinition;
        }

        public static OrderDefinitionNode GetOrderDefinitionNodeCustomRecipeOptional(ScriptableObject recipeSO)
        {
            if (recipeSO is PseudoPrefabSO)
                return PseudoPrefabManager.LoadAsset<OrderDefinitionNode>(recipeSO as PseudoPrefabSO);

            CustomRecipeSO customRecipeSO = recipeSO as CustomRecipeSO;
            if (customRecipeSO == null) return null;
            FixOldCustomRecipeSO(customRecipeSO);
            OrderDefinitionNode recipe;

            if (customRecipeSO is CustomRecipeOptionalBurgerSO)
            {
                CustomRecipeOptionalBurgerSO optionalBurgerSO = (CustomRecipeOptionalBurgerSO)customRecipeSO;
                CompositeOrderNode node = ScriptableObject.CreateInstance<CompositeOrderNode>();
                node.m_composition = new OrderDefinitionNode[0];
                node.m_optional = optionalBurgerSO.optionalSOs
                    .Where(x => x != optionalBurgerSO.bunSO)
                    .Select(x => GetOrderDefinitionNode(x))
                    .Concat(new OrderDefinitionNode[] { GetIngredientOrderNode(optionalBurgerSO.bunSO) })
                    .ToArray();

                GameObject platingPrefabAsset = customRecipeSO.GetModel();
                GameObject platingPrefab = RuntimePrefabManager.CloneAsInactivePrefab(platingPrefabAsset, clearOnRestart: false);
                BurgerBunCosmeticDecisions burgerCosmetic = platingPrefab.GetComponent<BurgerBunCosmeticDecisions>();
                BurritoCosmeticDecisions burritoCosmetic = platingPrefab.GetComponent<BurritoCosmeticDecisions>();
                HotdogCosmeticDecisions hotdogCosmetic = platingPrefab.GetComponent<HotdogCosmeticDecisions>();
                if (burgerCosmetic != null)
                {
                    OrderToPrefabLookup oldLookup = (OrderToPrefabLookup)typeof(BurgerBunCosmeticDecisions)
                        .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(burgerCosmetic);
                    OrderToPrefabLookup lookup = GetOrderToPrefabLookupBurger(optionalBurgerSO, oldLookup);
                    typeof(BurgerBunCosmeticDecisions)
                        .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                        .SetValue(burgerCosmetic, lookup);
                }
                else if (burritoCosmetic != null)
                {
                    OrderToPrefabLookup oldLookup = (OrderToPrefabLookup)typeof(OverlapModelsMealDecisions)
                        .GetField("m_prefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                        .GetValue(burritoCosmetic);
                    OrderToPrefabLookup lookup = GetOrderToPrefabLookupBurger(optionalBurgerSO, oldLookup);
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
                    OrderToPrefabLookup lookup = GetOrderToPrefabLookupBurger(optionalBurgerSO, oldLookup);
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
                    UnityEngine.Object.DestroyImmediate(hotdogCosmetic);
                }

                node.m_platingPrefab = platingPrefab;
                recipe = node;
            }
            else if (customRecipeSO is CustomRecipeOptionalPizzaSO)
            {
                CustomRecipeOptionalPizzaSO optionalPizzaSO = (CustomRecipeOptionalPizzaSO)customRecipeSO;
                CookedCompositeOrderNode node = ScriptableObject.CreateInstance<CookedCompositeOrderNode>();
                node.m_cookingStep = PseudoPrefabManager.LoadAsset<CookingStepData>(optionalPizzaSO.cookingStepSO);
                node.m_progress = optionalPizzaSO.cooked ? CookedCompositeOrderNode.CookingProgress.Cooked : CookedCompositeOrderNode.CookingProgress.Raw;
                node.m_composition = new OrderDefinitionNode[] { GetIngredientOrderNode(optionalPizzaSO.doughSO) };
                node.m_optional = optionalPizzaSO.optionalSOs.Select(x => GetOrderDefinitionNode(x)).ToArray();

                GameObject platingPrefabAsset = customRecipeSO.GetModel();
                GameObject platingPrefab = RuntimePrefabManager.CloneAsInactivePrefab(platingPrefabAsset, clearOnRestart: false);
                PizzaCosmeticDecisions pizzaCosmetic = platingPrefab.GetComponent<PizzaCosmeticDecisions>();
                OrderToPrefabLookup uncookedLookup = GetOrderToPrefabLookupPizza(optionalPizzaSO, false, pizzaCosmetic);
                OrderToPrefabLookup cookedLookup = GetOrderToPrefabLookupPizza(optionalPizzaSO, true, pizzaCosmetic);
                pizzaCosmetic.GetType()
                    .GetField("m_uncookedPrefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(pizzaCosmetic, uncookedLookup);
                pizzaCosmetic.GetType()
                    .GetField("m_cookedPrefabLookup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(pizzaCosmetic, cookedLookup);

                node.m_platingPrefab = platingPrefab;
                recipe = node;
            }
            else
            {
                recipe = GetOrderDefinitionNodeCustomRecipe(customRecipeSO);
            }

            recipe.name = customRecipeSO.recipeName;
            recipe.m_uID = customRecipeSO.uID;
            recipe.m_platingStep =
                customRecipeSO.platingStepSO != null ?
                PseudoPrefabManager.LoadAsset<PlatingStepData>(customRecipeSO.platingStepSO) : null;

            return recipe;
        }

        public static OrderToPrefabLookup GetOrderToPrefabLookupBurger(CustomRecipeOptionalBurgerSO optionalBurgerSO, OrderToPrefabLookup oldLookup)
        {
            OrderToPrefabLookup lookup = GetOrderToPrefabLookup(
                optionalBurgerSO.recipeName, 
                optionalBurgerSO.optionalSOs, 
                optionalBurgerSO.ingredientModels, optionalBurgerSO.ingredientModelSOs, oldLookup);
            return lookup;
        }

        public static OrderToPrefabLookup GetOrderToPrefabLookupPizza(CustomRecipeOptionalPizzaSO optionalPizzaSO, bool cooked, PizzaCosmeticDecisions pizzaCosmeticDecisions)
        {
            var models = cooked ? optionalPizzaSO.cookedPizzaIngredientPrefabs : optionalPizzaSO.rawPizzaIngredientPrefabs;
            var modelSOs = cooked ? optionalPizzaSO.cookedPizzaIngredientPrefabSOs : optionalPizzaSO.rawPizzaIngredientPrefabSOs;
            OrderToPrefabLookup oldLookup = (OrderToPrefabLookup)pizzaCosmeticDecisions.GetType()
                .GetField(cooked ? "m_cookedPrefabLookup" : "m_uncookedPrefabLookup", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(pizzaCosmeticDecisions);
            OrderToPrefabLookup lookup = GetOrderToPrefabLookup(
                optionalPizzaSO.recipeName, 
                optionalPizzaSO.optionalSOs, 
                models, modelSOs, oldLookup);
            return lookup;
        }

        public static OrderToPrefabLookup GetOrderToPrefabLookup(string recipeName, ScriptableObject[] optionalSOs, GameObject[] models, PseudoPrefabSO[] modelSOs, OrderToPrefabLookup oldLookup)
        {
            var indices = Enumerable.Range(0, optionalSOs.Length);
            OrderDefinitionNode[] orderDefinitionNodes = indices.Select(
                i => GetOrderDefinitionNode(optionalSOs[i])).ToArray();
            OrderToPrefabLookup.ContentPrefabLookup[] oldLookupArray = new OrderToPrefabLookup.ContentPrefabLookup[0];
            if (oldLookup != null)
            {
                oldLookupArray = (OrderToPrefabLookup.ContentPrefabLookup[])oldLookup.GetType()
                    .GetField("m_lookupArray", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(oldLookup);
            }
            GameObject[] realModels = indices.Select(i =>
            {
                if (i < models.Length && models[i] != null)
                    return models[i];
                if (i < modelSOs.Length && modelSOs[i] != null)
                    return PseudoPrefabManager.LoadAsset(modelSOs[i]);
                int index = oldLookupArray.FindIndex_Predicate(x => x.m_content.Equals(orderDefinitionNodes[i]));
                if (index >= 0)
                    return oldLookupArray[index].m_prefab;
                else
                    return null;
            }).ToArray();
            OrderToPrefabLookup lookup = GetOrderToPrefabLookup(orderDefinitionNodes, realModels);
            lookup.name = "Lookup_" + recipeName;
            return lookup;
        }

        private static OrderToPrefabLookup GetOrderToPrefabLookup(OrderDefinitionNode[] orderDefinitionNodes, GameObject[] models)
        {
            OrderToPrefabLookup lookup = ScriptableObject.CreateInstance<OrderToPrefabLookup>();
            Dictionary<OrderDefinitionNode, OrderToPrefabLookup.ContentPrefabLookup> lookupDic = new Dictionary<OrderDefinitionNode, OrderToPrefabLookup.ContentPrefabLookup>();
            for (int i = 0; i < orderDefinitionNodes.Length; i++)
            {
                OrderDefinitionNode ingredientNode = orderDefinitionNodes[i];
                OrderDefinitionNode matchKey = lookupDic.Keys.FirstOrDefault(key => key.Equals(ingredientNode));
                if (matchKey != null)
                {
                    lookupDic[matchKey].m_amountAllowed += 1;
                }
                else
                {
                    GameObject prefab = models[i];
                    lookupDic[ingredientNode] = new OrderToPrefabLookup.ContentPrefabLookup
                    {
                        m_content = ingredientNode,
                        m_prefab = prefab,
                        m_amountAllowed = 1,
                    };
                }
            }
            lookup.GetType()
                .GetField("m_lookupArray", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(lookup, lookupDic.Values.ToArray());
            return lookup;
        }

        public static IngredientOrderNode GetIngredientOrderNode(PseudoPrefabSO pseudoPrefabSO)
        {
            UnityEngine.Object prefab = PseudoPrefabManager.LoadAsset<UnityEngine.Object>(pseudoPrefabSO);
            IngredientOrderNode ingredientOrderNode = prefab as IngredientOrderNode;
            if (ingredientOrderNode != null)
                return ingredientOrderNode;
            GameObject ingredient = prefab as GameObject;
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
                if (preparationContainer != null)
                {
                    return preparationContainer.m_ingredientOrderNode;
                }
                else
                {
                    return null;
                }
            }
        }

        public static ItemOrderNode GetItemOrderNode(PseudoPrefabSO pseudoPrefabSO)
        {
            UnityEngine.Object prefab = PseudoPrefabManager.LoadAsset<UnityEngine.Object>(pseudoPrefabSO);
            ItemOrderNode itemOrderNode = prefab as ItemOrderNode;
            if (itemOrderNode != null)
                return itemOrderNode;
            GameObject item = prefab as GameObject;
            while (item.GetComponent<WorkableItem>() != null)
                item = item.GetComponent<WorkableItem>().m_nextPrefab;
            ItemPropertiesComponent itemPropertiesComponent = item.GetComponent<ItemPropertiesComponent>();
            if (itemPropertiesComponent != null)
            {
                return (ItemOrderNode)itemPropertiesComponent.GetType()
                    .GetField("m_itemDefinition", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(itemPropertiesComponent);
            }
            else
            {
                return null;
            }
        }

        public static OrderDefinitionNode GetIngredientOrItemOrderNode(PseudoPrefabSO pseudoPrefabSO)
        {
            OrderDefinitionNode ingredientOrderNode = GetIngredientOrderNode(pseudoPrefabSO);
            return ingredientOrderNode ?? GetItemOrderNode(pseudoPrefabSO);
        }

        public static CookingStepData GetCookingStepData(PseudoPrefabSO pseudoPrefabSO)
        {
            return PseudoPrefabManager.LoadAsset<CookingStepData>(pseudoPrefabSO);
        }

        public static GameObject GetModel(this CustomRecipeSO customRecipeSO)
        {
            return 
                customRecipeSO.model != null ? customRecipeSO.model :
                customRecipeSO.modelSO != null ? PseudoPrefabManager.LoadAsset(customRecipeSO.modelSO) : null;
        }

        public static Sprite GetIcon(this CustomRecipeSO customRecipeSO)
        {
            return 
                customRecipeSO.icon != null ? customRecipeSO.icon :
                customRecipeSO.iconSO != null ? PseudoPrefabManager.LoadSpriteSubAsset(customRecipeSO.iconSO) : null;
        }

        public static Sprite GetCookingStepIcon(this CustomRecipeSO customRecipeSO)
        {
            return 
                customRecipeSO.cookingStepIcon != null ? customRecipeSO.cookingStepIcon :
                customRecipeSO.cookingStepIconSO != null ? PseudoPrefabManager.LoadSpriteSubAsset(customRecipeSO.cookingStepIconSO) : null;
        }

        public static Sprite GetMixingIcon(this CustomRecipeSO customRecipeSO)
        {
            return
                customRecipeSO.mixingIcon != null ? customRecipeSO.mixingIcon :
                customRecipeSO.mixingIconSO != null ? PseudoPrefabManager.LoadSpriteSubAsset(customRecipeSO.mixingIconSO) : null;
        }

        private static void FixOldCustomRecipeSO(CustomRecipeSO customRecipeSO)
        {
            if (customRecipeSO.type == CustomRecipeSO.RecipeType.Null)
            {
                if (customRecipeSO.cookingStepSO != null)
                {
                    customRecipeSO.type = CustomRecipeSO.RecipeType.Cooked;
                    customRecipeSO.cookingProgress = CustomRecipeSO.CookingProgress.Cooked;
                }
                else
                {
                    customRecipeSO.type = CustomRecipeSO.RecipeType.Composite;
                }
                if (customRecipeSO is CustomRecipeOptionalPizzaSO)
                {
                    CustomRecipeOptionalPizzaSO pizzaSO = (CustomRecipeOptionalPizzaSO)customRecipeSO;
                    customRecipeSO.cookingProgress = pizzaSO.cooked ? CustomRecipeSO.CookingProgress.Cooked : CustomRecipeSO.CookingProgress.Raw;
                    int doughIndex = pizzaSO.compositionSOs.FindIndex_Predicate(x => x == pizzaSO.doughSO);
                    int length = pizzaSO.compositionSOs.Length;
                    if (doughIndex >= 0 &&
                        pizzaSO.rawPizzaIngredientPrefabs.Length == length &&
                        pizzaSO.rawPizzaIngredientPrefabSOs.Length == length &&
                        pizzaSO.cookedPizzaIngredientPrefabs.Length == length &&
                        pizzaSO.cookedPizzaIngredientPrefabSOs.Length == length)
                    {
                        var indices = Enumerable.Range(0, length).Where(i => i != doughIndex);
                        pizzaSO.optionalSOs = indices.Select(i => pizzaSO.compositionSOs[i]).ToArray();
                        pizzaSO.rawPizzaIngredientPrefabs = indices.Select(i => pizzaSO.rawPizzaIngredientPrefabs[i]).ToArray();
                        pizzaSO.rawPizzaIngredientPrefabSOs = indices.Select(i => pizzaSO.rawPizzaIngredientPrefabSOs[i]).ToArray();
                        pizzaSO.cookedPizzaIngredientPrefabs = indices.Select(i => pizzaSO.cookedPizzaIngredientPrefabs[i]).ToArray();
                        pizzaSO.cookedPizzaIngredientPrefabSOs = indices.Select(i => pizzaSO.cookedPizzaIngredientPrefabSOs[i]).ToArray();
                        pizzaSO.compositionSOs = new ScriptableObject[] { pizzaSO.doughSO };
                    }
                }
            }
        }

        public static bool IsEmpty(this Array array)
        {
            return array == null || array.Length == 0;
        }

        public static GameObject GetIngredientPrefabForOptional(PseudoPrefabSO itemPrefabSO)
        {
            GameObject originalPrefab = PseudoPrefabManager.LoadAsset(itemPrefabSO);

            // fix: coal can be thrown into the furnace
            if (originalPrefab.GetComponent<ItemPropertiesComponent>() != null && 
                originalPrefab.GetComponent<ItemHeatTransferBehaviour>() == null)
            {
                GameObject coal = RuntimePrefabManager.CloneAsInactivePrefab(originalPrefab);
                coal.AddComponent<ItemHeatTransferBehaviour>();
                return coal;
            }

            var optionalRecipeMatchListItems = PseudoPrefabManager.Instance.stub.levelInfo.optionalRecipeMatchListItems;
            if (optionalRecipeMatchListItems == null) return originalPrefab;
            foreach (var optionalRecipe in optionalRecipeMatchListItems)
            {
                if (optionalRecipe is CustomRecipeOptionalBurgerSO)
                {
                    var optionalBurger = (CustomRecipeOptionalBurgerSO)optionalRecipe;
                    if (optionalBurger.bunSO != itemPrefabSO) continue;

                    GameObject originalUnchoppedPrefab = null;
                    if (originalPrefab.GetComponent<WorkableItem>() != null)
                    {
                        originalUnchoppedPrefab = originalPrefab;
                        originalPrefab = originalPrefab.GetComponent<WorkableItem>().m_nextPrefab;
                    }
                    GameObject bunPrefab = RuntimePrefabManager.CloneAsInactivePrefab(originalPrefab);
                    bunPrefab.GetComponent<IngredientContainer>().m_capacity = optionalBurger.ingredientContainerCapacity;
                    PreparationContainer container = bunPrefab.GetComponent<PreparationContainer>();
                    OrderToPrefabLookup oldLookup = container.m_containerRestrictions;
                    OrderToPrefabLookup lookup = GetOrderToPrefabLookupBurger(optionalBurger, oldLookup);
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
                        UnityEngine.Object.DestroyImmediate(hotdogCosmetic);
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
                else if (optionalRecipe is CustomRecipeOptionalPizzaSO)
                {
                    var optionalPizza = (CustomRecipeOptionalPizzaSO)optionalRecipe;
                    if (optionalPizza.doughSO != itemPrefabSO) continue;
                    GameObject prefabAssetNext = originalPrefab.GetComponent<WorkableItem>().m_nextPrefab;
                    GameObject doughPrefab = RuntimePrefabManager.CloneAsInactivePrefab(originalPrefab);
                    GameObject doughPrefabNext = RuntimePrefabManager.CloneAsInactivePrefab(prefabAssetNext);

                    doughPrefabNext.GetComponent<IngredientContainer>().m_capacity = optionalPizza.ingredientContainerCapacity;
                    CookablePreparationContainer container = doughPrefabNext.GetComponent<CookablePreparationContainer>();
                    PizzaCosmeticDecisions pizzaCosmeticDecisions = container.m_cosmeticsPrefab.GetComponent<PizzaCosmeticDecisions>();
                    OrderToPrefabLookup uncookedLookup = GetOrderToPrefabLookupPizza(optionalPizza, false, pizzaCosmeticDecisions);
                    OrderToPrefabLookup cookedLookup = GetOrderToPrefabLookupPizza(optionalPizza, true, pizzaCosmeticDecisions);
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
            }
            return originalPrefab;
        }
    }
}
