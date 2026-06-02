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
            recipeList.m_recipes = config.recipes
                .Take(takeNum)
                .Select(x => RecipeHelper.GetRecipe(x))
                .ToArray();
            configTemplate.m_rounds[0].m_recipes = recipeList;

            if (config.recipes.Any(x => x is CustomRecipeSO) ||
                config.optionalRecipeMatchListItems != null && config.optionalRecipeMatchListItems.Length > 0)
            {
                RecipeMatchList theRecipeMatchList = configTemplate.m_recipeMatchingList;
                RecipeMatchList newRecipeMatchList = ScriptableObject.CreateInstance<RecipeMatchList>();
                newRecipeMatchList.name = "RecipeMatchList_" + config.name;
                newRecipeMatchList.m_includeLists = new RecipeMatchList[] { theRecipeMatchList };
                newRecipeMatchList.m_cookingSteps = new CookingStepData[0];

                List<OrderDefinitionNode> newRecipes = new List<OrderDefinitionNode>();
                if (config.optionalRecipeMatchListItems != null && config.optionalRecipeMatchListItems.Length > 0)
                {
                    newRecipes = config.optionalRecipeMatchListItems
                        .Select(x => RecipeHelper.GetOptionalRecipeNode(x))
                        .ToList();
                }
                if (config.recipes.Any(x => x is CustomRecipeSO))
                {
                    for (int i = 0; i < recipeList.m_recipes.Length; i++)
                    {
                        if (!(config.recipes[i] is CustomRecipeSO)) continue;
                        CustomRecipeSO customRecipeSO = (CustomRecipeSO)config.recipes[i];
                        newRecipes.Add(recipeList.m_recipes[i].m_order);
                        if (config.optionalRecipeMatchListItems == null || customRecipeSO.modelSO == null) continue;
                        for (int j = 0; j < config.optionalRecipeMatchListItems.Length; j++)
                        {
                            if (customRecipeSO.modelSO == config.optionalRecipeMatchListItems[j].modelSO)
                            {
                                recipeList.m_recipes[i].m_order.m_platingPrefab = newRecipes[j].m_platingPrefab;
                                break;
                            }
                        }
                    }
                }
                newRecipeMatchList.m_recipes = newRecipes.ToArray();

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
