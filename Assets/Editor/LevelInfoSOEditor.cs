using LevelEditor;
using LevelEditorStub;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(LevelInfoSO))]
public class LevelInfoSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 获取序列化属性的迭代器
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        // 顺着脚本里的字段顺序，从上往下挨个遍历绘制
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false; // 只遍历顶层字段，不进入子属性内部

            // 跳过 Unity 默认自带的 "m_Script" 脚本路径字段
            if (iterator.name == "m_Script")
            {
                continue;
            }

            // 正常绘制当前字段（保留原本的 Header 和 Space 装饰）
            EditorGUILayout.PropertyField(iterator, true);

            if (iterator.name == "allIngredients")
            {
                // 在它下方插入按钮
                GUILayout.Space(5);
                //GUI.backgroundColor = Color.white;
                if (GUILayout.Button("Auto Fill All Ingredients", GUILayout.Height(25)))
                {
                    LevelInfoSO levelInfo = (LevelInfoSO)target;
                    Undo.RecordObject(levelInfo, "Auto Fill Ingredients");
                    AutoFillIngredients(levelInfo);
                    EditorUtility.SetDirty(levelInfo);
                }
                //GUI.backgroundColor = Color.white; // 恢复默认颜色
                GUILayout.Space(5);
            }

            if (iterator.name == "audioDirectorySOs")
            {
                // 在它下方插入按钮
                GUILayout.Space(5);
                //GUI.backgroundColor = Color.white;
                if (GUILayout.Button("Fill All AudioDirectorySOs", GUILayout.Height(25)))
                {
                    LevelInfoSO levelInfo = (LevelInfoSO)target;
                    Undo.RecordObject(levelInfo, "Fill All AudioDirectorySOs");
                    FillAllAudioDirectorySOs(levelInfo);
                    EditorUtility.SetDirty(levelInfo);
                }
                //GUI.backgroundColor = Color.white; // 恢复默认颜色
                GUILayout.Space(5);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AutoFillIngredients(LevelInfoSO levelInfo)
    {
        List<PseudoPrefabSO> allIngredients = new List<PseudoPrefabSO>();
        List<OrderDefinitionNode> allOrderDefinitionNodes = new List<OrderDefinitionNode>();

        if (!levelInfo.excludeStoryRecipeMatchList)
        {
            PseudoPrefabSO theRecipeMatchListSO = ScriptableObject.CreateInstance<PseudoPrefabSO>();
            theRecipeMatchListSO.bundleName = "bundle18";
            theRecipeMatchListSO.assetPath = "Assets/data/recipedata/TheRecipeMatchList.asset";
            RecipeMatchList recipeMatchList = PseudoPrefabManager.LoadAsset<RecipeMatchList>(theRecipeMatchListSO);
            foreach (OrderDefinitionNode orderDefinitionNode in recipeMatchList.m_recipes)
                if (orderDefinitionNode is IngredientOrderNode || orderDefinitionNode is ItemOrderNode)
                    allOrderDefinitionNodes.Add(orderDefinitionNode);
            DestroyImmediate(theRecipeMatchListSO);
        }
        if (!levelInfo.includeRecipeMatchLists.IsEmpty())
        {
            foreach (PseudoPrefabSO matchListSO in levelInfo.includeRecipeMatchLists)
            {
                RecipeMatchList recipeMatchList = PseudoPrefabManager.LoadAsset<RecipeMatchList>(matchListSO);
                foreach (OrderDefinitionNode orderDefinitionNode in recipeMatchList.m_recipes)
                    if (orderDefinitionNode is IngredientOrderNode || orderDefinitionNode is ItemOrderNode)
                        allOrderDefinitionNodes.Add(orderDefinitionNode);
            }
        }

        foreach (PseudoPrefabDispenserStub dispenserStub in FindObjectsOfType<PseudoPrefabDispenserStub>())
        {
            OrderDefinitionNode orderDefinitionNode = RecipeHelper.GetIngredientOrItemOrderNode(dispenserStub.spawnerItemPrefabSO);
            if (!allOrderDefinitionNodes.Any(x => x.Equals(orderDefinitionNode)))
            {
                allOrderDefinitionNodes.Add(orderDefinitionNode);
                allIngredients.Add(dispenserStub.spawnerItemPrefabSO);
            }
        }
        foreach (PseudoPrefabAttachingFoodSpawnerStub pseudoPrefabAttachingFoodSpawnerStub in FindObjectsOfType<PseudoPrefabAttachingFoodSpawnerStub>())
        {
            foreach (PseudoPrefabSO spawnerItemPrefabSO in pseudoPrefabAttachingFoodSpawnerStub.attachmentPrefabSOs)
            {
                OrderDefinitionNode orderDefinitionNode = RecipeHelper.GetIngredientOrItemOrderNode(spawnerItemPrefabSO);
                if (!allOrderDefinitionNodes.Any(x => x.Equals(orderDefinitionNode)))
                {
                    allOrderDefinitionNodes.Add(orderDefinitionNode);
                    allIngredients.Add(spawnerItemPrefabSO);
                }
            }
        }
        foreach (PseudoPrefabIngredientSprayStub pseudoPrefabIngredientSprayStub in FindObjectsOfType<PseudoPrefabIngredientSprayStub>())
        {
            OrderDefinitionNode orderDefinitionNode = RecipeHelper.GetIngredientOrItemOrderNode(pseudoPrefabIngredientSprayStub.orderPrefabSO);
            if (!allOrderDefinitionNodes.Any(x => x.Equals(orderDefinitionNode)))
            {
                allOrderDefinitionNodes.Add(orderDefinitionNode);
                allIngredients.Add(pseudoPrefabIngredientSprayStub.orderPrefabSO);
            }
        }
        foreach (SpecificPseudoPrefabTag specificPseudoPrefabTag in FindObjectsOfType<SpecificPseudoPrefabTag>())
        {
            switch (specificPseudoPrefabTag.prefabTag)
            {
                case "dispenser_coal_01":
                case "dlc08_condiment_dispenser":
                    {
                        PseudoPrefabSO[] itemSOs = specificPseudoPrefabTag.GetComponent<PseudoPrefabSOArray>().pseudoPrefabSOs;
                        foreach (PseudoPrefabSO itemSO in itemSOs)
                        {
                            OrderDefinitionNode orderDefinitionNode = RecipeHelper.GetIngredientOrItemOrderNode(itemSO);
                            if (!allOrderDefinitionNodes.Any(x => x.Equals(orderDefinitionNode)))
                            {
                                allOrderDefinitionNodes.Add(orderDefinitionNode);
                                allIngredients.Add(itemSO);
                            }

                        }
                        break;
                    }

                default:
                    break;
            }
        }

        levelInfo.allIngredients = allIngredients.ToArray();
    }

    private void FillAllAudioDirectorySOs(LevelInfoSO levelInfo)
    {
        string path = "Assets/common02/pseudo_prefab_so/audio/AudioDirectories";
        string[] guids = AssetDatabase.FindAssets("", new[] { path });

        List<PseudoPrefabSO> allAudioDirectorySOs = new List<PseudoPrefabSO>();
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            allAudioDirectorySOs.Add(AssetDatabase.LoadAssetAtPath<PseudoPrefabSO>(assetPath));
        }
        levelInfo.audioDirectorySOs = allAudioDirectorySOs.ToArray();
    }
}