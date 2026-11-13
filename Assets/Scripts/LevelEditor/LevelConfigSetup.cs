using GameModes;
using GameModes.Horde;
using LevelEditorStub;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LevelEditor
{
    public static class LevelConfigSetup {

        public static LevelConfigBase SetupConfig(PseudoPrefabSO configTemplateSO, LevelInfoSO config, int playerCount)
        {
            LevelConfigBase configTemplate = PseudoPrefabManager.LoadAsset<LevelConfigBase>(configTemplateSO);
            configTemplate = ScriptableObject.Instantiate(configTemplate);
            LevelConfigSetupPerPlayerCountSO configPerPlayerCount = GetConfigPerPlayerCount(config, playerCount);
            LevelConfigBase levelConfig;
            CampaignLevelConfig campaignLevelConfig = null;
            DynamicCampaignLevelConfig dynamicCampaignLevelConfig = null;
            ScriptedCampaignLevelConfig scriptedCampaignLevelConfig = null;
            HordeLevelConfig hordeLevelConfig = null;
            if (config.levelType == LevelInfoSO.LevelType.Campaign)
            {
                if (configPerPlayerCount.scriptedRecipes.IsEmpty())
                {
                    campaignLevelConfig = ScriptableObject.CreateInstance<CampaignLevelConfig>();
                    levelConfig = campaignLevelConfig;
                }
                else
                {
                    scriptedCampaignLevelConfig = ScriptableObject.CreateInstance<ScriptedCampaignLevelConfig>();
                    levelConfig = scriptedCampaignLevelConfig;
                }
            }
            else if (config.levelType == LevelInfoSO.LevelType.Dynamic)
            {
                dynamicCampaignLevelConfig = ScriptableObject.CreateInstance<DynamicCampaignLevelConfig>();
                levelConfig = dynamicCampaignLevelConfig;
            }
            else if (config.levelType == LevelInfoSO.LevelType.Horde)
            {
                hordeLevelConfig = ScriptableObject.CreateInstance<HordeLevelConfig>();
                levelConfig = hordeLevelConfig;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            levelConfig.name = string.Format("{0}_{1}p", config.name, playerCount.ToString());

            levelConfig.m_hazardInfo = configTemplate.m_hazardInfo;
            levelConfig.m_disableDynamicParenting = config.disableDynamicParenting;
            levelConfig.m_gridSelection = true;
            levelConfig.m_showIconPrompts = false;
            levelConfig.m_enableRespawnBounds = config.enableRespawnBounds;
            levelConfig.m_objectives = new LevelObjectiveBase[0];

            if (levelConfig is KitchenLevelConfigBase)
            {
                KitchenLevelConfigBase kitchenLevelConfigBaseTemplate = configTemplate as KitchenLevelConfigBase;
                KitchenLevelConfigBase kitchenLevelConfigBase = levelConfig as KitchenLevelConfigBase;
                kitchenLevelConfigBase.m_orderLifetime = configPerPlayerCount.orderLifeTime;
                kitchenLevelConfigBase.m_timeBetweenOrders = configPerPlayerCount.timeBetweenOrders;
                kitchenLevelConfigBase.m_plateReturnTime = configPerPlayerCount.plateReturnTime;
                kitchenLevelConfigBase.m_campaignConfig = kitchenLevelConfigBaseTemplate.m_campaignConfig;
                kitchenLevelConfigBase.m_practiceConfig = kitchenLevelConfigBaseTemplate.m_practiceConfig;
                kitchenLevelConfigBase.m_survivalConfig = kitchenLevelConfigBaseTemplate.m_survivalConfig;
                kitchenLevelConfigBase.m_survivalConfig.m_timeMultiplier = configPerPlayerCount.survivalTimeMultiplier;
            }
            if (hordeLevelConfig != null)
            {
                HordeLevelConfig hordeLevelConfigTemplate = configTemplate as HordeLevelConfig;
                hordeLevelConfig.m_plateReturnTime = configPerPlayerCount.plateReturnTime;
                hordeLevelConfig.m_targetHealth = config.targetHealth;
                hordeLevelConfig.m_targetRepairSpeed = config.targetRepairSpeed;
                hordeLevelConfig.m_targetRepairThreshold = config.targetRepairThreshold;
                hordeLevelConfig.m_targetRepairCostMax = configPerPlayerCount.hordeLevelConfig.targetRepairCostMax;
                hordeLevelConfig.m_health = config.kitchenHealth;
                hordeLevelConfig.m_flowroutineData = hordeLevelConfigTemplate.m_flowroutineData;
            }

            RecipeList recipeList = ScriptableObject.CreateInstance<RecipeList>();
            recipeList.name = levelConfig.name;
            recipeList.m_recipes = new RecipeList.Entry[0];
            List<ScriptableObject> allRecipes = new List<ScriptableObject>();
            List<RecipeList.Entry> allEntries = new List<RecipeList.Entry>();
            if (config.levelType == LevelInfoSO.LevelType.Campaign)
            {
                int takeNum = config.debugRecipeCount == 0 ? config.recipes.Length : config.debugRecipeCount;
                var recipes = config.recipes.Take(takeNum);
                recipeList.m_recipes = recipes.Select(x => RecipeHelper.GetRecipe(x)).ToArray();
                allRecipes.AddRange(recipes);
                allEntries.AddRange(recipeList.m_recipes);
            }

            if (campaignLevelConfig != null)
            {
                RoundData roundData = new RoundData();
                roundData.m_roundTimer = configPerPlayerCount.roundTime;
                roundData.m_recipes = recipeList;
                campaignLevelConfig.m_rounds = new RoundData[] { roundData };
            }
            if (scriptedCampaignLevelConfig != null)
            {
                ScriptedRoundData scriptedRoundData = new ScriptedRoundData();
                scriptedRoundData.m_roundTimer = configPerPlayerCount.roundTime;
                scriptedRoundData.m_recipes = recipeList;
                scriptedRoundData.m_manualOrder = configPerPlayerCount.scriptedRecipes.Select(x => RecipeHelper.GetRecipe(x)).ToArray();
                allRecipes.AddRange(configPerPlayerCount.scriptedRecipes);
                allEntries.AddRange(scriptedRoundData.m_manualOrder);
                scriptedCampaignLevelConfig.m_rounds = new ScriptedRoundData[] { scriptedRoundData };
            }
            if (dynamicCampaignLevelConfig != null)
            {
                DynamicRoundData dynamicRoundData = new DynamicRoundData();
                dynamicRoundData.m_roundTimer = configPerPlayerCount.roundTime;
                dynamicRoundData.m_recipes = recipeList;
                dynamicRoundData.Phases = new DynamicRoundData.Phase[config.recipeListSOs.Length];
                for (int i = 0; i < config.recipeListSOs.Length; i++)
                {
                    RecipeList phaseRecipeList = ScriptableObject.CreateInstance<RecipeList>();
                    phaseRecipeList.name = levelConfig.name + "_phase_" + i.ToString();
                    phaseRecipeList.m_recipes = config.recipeListSOs[i].recipes.Select(x => RecipeHelper.GetRecipe(x)).ToArray();
                    allRecipes.AddRange(config.recipeListSOs[i].recipes);
                    allEntries.AddRange(phaseRecipeList.m_recipes);
                    DynamicRoundData.Phase phase = new DynamicRoundData.Phase();
                    phase.Duration = i < config.roundTimes.Length ? config.roundTimes[i] : 0;
                    phase.Recipes = phaseRecipeList;
                    dynamicRoundData.Phases[i] = phase;
                }
                dynamicCampaignLevelConfig.m_data = dynamicRoundData;
            }
            if (hordeLevelConfig != null)
            {
                List<HordeWaveData> m_waves = new List<HordeWaveData>();
                foreach (var waveDataSO in configPerPlayerCount.hordeLevelConfig.hordeWaveDataSOs)
                {
                    HordeWaveData wave = new HordeWaveData();
                    RecipeList waveRecipeList = ScriptableObject.CreateInstance<RecipeList>();
                    waveRecipeList.name = waveDataSO.name;
                    waveRecipeList.m_recipes = waveDataSO.recipeListSO.recipes.Select(x => RecipeHelper.GetRecipe(x)).ToArray();
                    wave.m_recipes = waveRecipeList;
                    allRecipes.AddRange(waveDataSO.recipeListSO.recipes);
                    allEntries.AddRange(waveRecipeList.m_recipes);
                    wave.m_intervalSeconds = waveDataSO.intervalSeconds;
                    wave.m_spawns = new List<HordeSpawnData>();
                    for (int j = 0; j < waveDataSO.enemies.Length; j++)
                    {
                        HordeSpawnData spawnData = new HordeSpawnData();
                        spawnData.m_spawnTimeSeconds = j < waveDataSO.spawnTimes.Length ? waveDataSO.spawnTimes[j] : 0;
                        spawnData.m_prefab = PseudoPrefabManager.LoadAsset(waveDataSO.enemies[j]);
                        wave.m_spawns.Add(spawnData);
                    }
                    m_waves.Add(wave);
                }
                object obj = new HordeWavesData();
                typeof(HordeWavesData)
                    .GetField("m_waves", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(obj, m_waves);
                HordeWavesData waves = (HordeWavesData)obj;
                hordeLevelConfig.m_waves = waves;
                hordeLevelConfig.m_recipeMoney = BuildRecipeMoneyData(hordeLevelConfig);
            }

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
                    OrderDefinitionNode orderDefinitionNode = RecipeHelper.GetOrderDefinitionNode(optionalRecipeMatchListItem);
                    optionalRecipeMatchListItems.Add(orderDefinitionNode);
                    newRecipeMatchListItems.Add(orderDefinitionNode);
                }
            }

            List<OrderDefinitionNode> allEntriesDeduplicated = new List<OrderDefinitionNode>();
            foreach (var entry in allEntries)
                if (!allEntriesDeduplicated.Any(x => x.Equals(entry.m_order)))
                    allEntriesDeduplicated.Add(entry.m_order);
            newRecipeMatchListItems.AddRange(allEntriesDeduplicated);

            // for optional burgers/pizzas/...
            // the preparation container prefab is built in GetOrderDefinitionNodeCustomRecipeOptional()
            // assign the prefab back to recipeList.m_recipes[i].m_order.m_platingPrefab
            if (!config.optionalRecipeMatchListItems.IsEmpty())
            {
                for (int i = 0; i < allRecipes.Count; i++)
                {
                    if (!(allRecipes[i] is CustomRecipeSO)) continue;
                    CustomRecipeSO customRecipeSO = (CustomRecipeSO)allRecipes[i];
                    for (int j = 0; j < config.optionalRecipeMatchListItems.Length; j++)
                    {
                        if (!(config.optionalRecipeMatchListItems[j] is ICustomRecipeOptional)) continue;
                        CustomRecipeSO optional = config.optionalRecipeMatchListItems[j] as CustomRecipeSO;
                        if (optional == null) continue;
                        if (optional.model != null && customRecipeSO.model != null && optional.model == customRecipeSO.model ||
                            optional.modelSO != null && customRecipeSO.modelSO != null && optional.modelSO == customRecipeSO.modelSO)
                        {
                            allEntries[i].m_order.m_platingPrefab = optionalRecipeMatchListItems[j].m_platingPrefab;
                            break;
                        }
                    }
                }
            }

            newRecipeMatchList.m_recipes = newRecipeMatchListItems.ToArray();
            levelConfig.m_recipeMatchingList = newRecipeMatchList;
            return levelConfig;
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

        private static RecipeMoneyData BuildRecipeMoneyData(HordeLevelConfig hordeLevelConfig)
        {
            HordeWavesData waves = hordeLevelConfig.m_waves;
            RecipeMoneyData recipeMoneyData = ScriptableObject.CreateInstance<RecipeMoneyData>();
            recipeMoneyData.name = hordeLevelConfig.name;
            List<RecipeList.Entry> recipes = new List<RecipeList.Entry>();
            var m_waves = (List<HordeWaveData>)typeof(HordeWavesData)
                .GetField("m_waves", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(waves);
            foreach (var wave in m_waves)
                foreach (var entry in wave.m_recipes.m_recipes)
                    if (!recipes.Any(x => x.m_order.Equals(entry.m_order)))
                        recipes.Add(entry);

            typeof(RecipeMoneyData)
                .GetField("m_keys", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(recipeMoneyData, recipes.Select(x => x.m_order).ToList());
            typeof(RecipeMoneyData)
                .GetField("m_values", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(recipeMoneyData, recipes.Select(x => x.m_scoreForMeal).ToList());
            return recipeMoneyData;
        }
    }
}
