using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using LevelEditorStub;


namespace LevelEditor
{
    public static class LevelConfigSetup {

        public static CampaignLevelConfig SetupConfig(PseudoPrefabSO configTemplateSO, LevelInfoSO config, int playerCount)
        {
            CampaignLevelConfig configTemplate = PseudoPrefabManager.LoadAsset<CampaignLevelConfig>(configTemplateSO);
            configTemplate = ScriptableObject.Instantiate(configTemplate);

            LevelConfigSetupPerPlayerCountSO configPerPlayerCount = GetConfigPerPlayerCount(config, playerCount);
            configTemplate.name = string.Format("{0}_{1}p", config.name, playerCount.ToString());
            configTemplate.m_orderLifetime = configPerPlayerCount.orderLifeTime;
            configTemplate.m_timeBetweenOrders = configPerPlayerCount.timeBetweenOrders;
            configTemplate.m_plateReturnTime = configPerPlayerCount.plateReturnTime;
            configTemplate.m_survivalConfig.m_timeMultiplier = configPerPlayerCount.survivalTimeMultiplier;
            configTemplate.m_objectives = new LevelObjectiveBase[0];
            configTemplate.m_disableDynamicParenting = config.disableDynamicParenting;
            configTemplate.m_rounds[0].m_roundTimer = configPerPlayerCount.roundTime;

            RecipeList recipeList = configTemplate.m_rounds[0].m_recipes;
            recipeList = ScriptableObject.Instantiate(recipeList);
            recipeList.name = config.name;
            int takeNum = config.debugRecipeCount == 0 ? config.recipes.Length : config.debugRecipeCount;
            ScriptableObject[] recipes = config.recipes.Take(takeNum).ToArray();
            recipeList.m_recipes = recipes.Select(x => RecipeHelper.GetRecipe(x)).ToArray();
            configTemplate.m_rounds[0].m_recipes = recipeList;

            if (true)
            //if (recipes.Any(x => x is CustomRecipeSO) ||
            //    !config.allIngredients.IsEmpty() ||
            //    !config.optionalRecipeMatchListItems.IsEmpty() ||
            //    config.excludeStoryRecipeMatchList ||
            //    !config.includeRecipeMatchLists.IsEmpty() ||
            //    !config.allCookingSteps.IsEmpty())
            {
                RecipeMatchList theRecipeMatchList = configTemplate.m_recipeMatchingList;
                RecipeMatchList newRecipeMatchList = ScriptableObject.CreateInstance<RecipeMatchList>();
                newRecipeMatchList.name = "RecipeMatchList_" + config.name;
                List<RecipeMatchList> m_includeLists = new List<RecipeMatchList>();
                if (!config.excludeStoryRecipeMatchList)
                    m_includeLists.Add(theRecipeMatchList);
                if (!config.includeRecipeMatchLists.IsEmpty())
                    m_includeLists.AddRange(config.includeRecipeMatchLists.Select(x => PseudoPrefabManager.LoadAsset<RecipeMatchList>(x)));
                newRecipeMatchList.m_includeLists = m_includeLists.ToArray();
                newRecipeMatchList.m_cookingSteps = new CookingStepData[0];

                if (!config.allCookingSteps.IsEmpty())
                {
                    newRecipeMatchList.m_cookingSteps = config.allCookingSteps.Select(x => RecipeHelper.GetCookingStepData(x)).ToArray();
                }

                List<OrderDefinitionNode> newRecipeMatchListItems = new List<OrderDefinitionNode>();
                List<OrderDefinitionNode> optionalRecipeMatchListItems = new List<OrderDefinitionNode>();

                if (!config.allIngredients.IsEmpty())
                {
                    newRecipeMatchListItems.AddRange(config.allIngredients.Select(x => RecipeHelper.GetIngredientOrItemOrderNode(x)));
                }

                if (!config.optionalRecipeMatchListItems.IsEmpty())
                {
                    foreach (var optionalRecipeMatchListItem in config.optionalRecipeMatchListItems)
                    {
                        OrderDefinitionNode orderDefinitionNode = RecipeHelper.GetOrderDefinitionNodeCustomRecipeOptional(optionalRecipeMatchListItem);
                        optionalRecipeMatchListItems.Add(orderDefinitionNode);
                        if (!recipes.Any(x => x == optionalRecipeMatchListItem))
                            newRecipeMatchListItems.Add(orderDefinitionNode);
                    }
                }

                newRecipeMatchListItems.AddRange(recipeList.m_recipes.Select(x => x.m_order));

                // for optional burgers/pizzas/...
                // the preparation container prefab is built in GetOrderDefinitionNodeCustomRecipeOptional()
                // assign the prefab back to recipeList.m_recipes[i].m_order.m_platingPrefab
                if (!config.optionalRecipeMatchListItems.IsEmpty())
                {
                    for (int i = 0; i < recipes.Length; i++)
                    {
                        if (!(recipes[i] is CustomRecipeSO)) continue;
                        CustomRecipeSO customRecipeSO = (CustomRecipeSO)recipes[i];
                        for (int j = 0; j < config.optionalRecipeMatchListItems.Length; j++)
                        {
                            CustomRecipeSO optional = config.optionalRecipeMatchListItems[j] as CustomRecipeSO;
                            if (optional == null) continue;
                            if (optional.model != null && customRecipeSO.model != null && optional.model == customRecipeSO.model ||
                                optional.modelSO != null && customRecipeSO.modelSO != null && optional.modelSO == customRecipeSO.modelSO)
                            {
                                recipeList.m_recipes[i].m_order.m_platingPrefab = optionalRecipeMatchListItems[j].m_platingPrefab;
                                break;
                            }
                        }
                    }
                }
                newRecipeMatchList.m_recipes = newRecipeMatchListItems.ToArray();

                configTemplate.m_recipeMatchingList = newRecipeMatchList;
            }

            return configTemplate;
        }

        private static LevelConfigSetupPerPlayerCountSO GetConfigPerPlayerCount(LevelInfoSO config, int playerCount)
        {
            return new LevelConfigSetupPerPlayerCountSO[]
            {
                config.config_1p, config.config_2p, config.config_3p, config.config_4p
            }[playerCount - 1];
        }

        public static SceneDirectoryData SetupSceneDirectoryData(PseudoPrefabSO configTemplateSO, LevelInfoSO config)
        {
            return SetupSceneDirectoryData(configTemplateSO, new LevelInfoSO[] { config });
        }

        public static SceneDirectoryData SetupSceneDirectoryData(PseudoPrefabSO configTemplateSO, LevelSetInfoSO levelSetInfo)
        {
            return SetupSceneDirectoryData(configTemplateSO, levelSetInfo.levelInfos);
        }

        private static SceneDirectoryData SetupSceneDirectoryData(PseudoPrefabSO configTemplateSO, LevelInfoSO[] levelInfos)
        {
            SceneDirectoryData diyLevelSceneDirectoryData = ScriptableObject.CreateInstance<SceneDirectoryData>();
            diyLevelSceneDirectoryData.name = "DIYLevelSceneDirectory";
            List<SceneDirectoryData.SceneDirectoryEntry> entries = new List<SceneDirectoryData.SceneDirectoryEntry>();
            foreach (LevelInfoSO levelInfo in levelInfos)
            {
                SceneDirectoryData.SceneDirectoryEntry entry = new SceneDirectoryData.SceneDirectoryEntry();
                entry.Label = string.Format("\"{0}\"", Localization.GetLanguage() == SupportedLanguages.Chinese ? levelInfo.levelNameZH : levelInfo.levelName);
                entry.LoadScreenOverride = levelInfo.screenshot;
                entry.AvailableInLobby = false;
                entry.SceneVarients = new SceneDirectoryData.PerPlayerCountDirectoryEntry[4];
                for (int i = 0; i < 4; i++)
                {
                    var sceneVarients = new SceneDirectoryData.PerPlayerCountDirectoryEntry();
                    sceneVarients.PlayerCount = i + 1;
                    sceneVarients.LevelConfig = SetupConfig(configTemplateSO, levelInfo, i + 1);
                    sceneVarients.SceneName = levelInfo.sceneName;
                    //sceneVarients.SceneName = string.Format("DIYLevel/{0}/{1}", levelSetInfo.levelSetName, levelInfo.sceneName);
                    sceneVarients.Screenshot = levelInfo.screenshot;
                    LevelConfigSetupPerPlayerCountSO configSetupPerPlayerCount = GetConfigPerPlayerCount(levelInfo, i + 1);
                    sceneVarients.GetType()
                        .GetField("m_PCStarBoundaries", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                        .SetValue(sceneVarients, new SceneDirectoryData.StarBoundaries
                        {
                            m_OneStarScore = configSetupPerPlayerCount.m_OneStarScore,
                            m_TwoStarScore = configSetupPerPlayerCount.m_TwoStarScore,
                            m_ThreeStarScore = configSetupPerPlayerCount.m_ThreeStarScore,
                            m_FourStarScore = configSetupPerPlayerCount.m_FourStarScore,
                        });
                    entry.SceneVarients[i] = sceneVarients;
                }
                entries.Add(entry);
            }
            diyLevelSceneDirectoryData.Scenes = entries.ToArray();
            return diyLevelSceneDirectoryData;
        }
    }
}
