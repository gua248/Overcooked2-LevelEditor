using LevelEditor;
using LevelEditorStub;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class CreateAssetBundles
{
    [MenuItem("Tools/Build AssetBundles", false, 100)]
    static void BuildAllAssetBundles()
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        if (!TargetSceneSaveValidator.CheckPrepareForBuilding(activeScene))
            return;

        string assetBundleDirectory = "Assets/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        // component A (for AStub) on prefabs will throw warnings of missing script at runtime
        // thus remove A before building and restore it afterward
        List<GameObject> prefabs = new List<GameObject>();
        try
        {
            prefabs = FindPrefabsToClean();
            CleanPrefabs(prefabs);

            BuildPipeline.BuildAssetBundles(
                assetBundleDirectory,
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows);
        }
        finally
        {
            RestorePrefabs(prefabs);
        }
    }

    private static List<GameObject> FindPrefabsToClean()
    {
        var prefabs = new List<GameObject>();
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                continue;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null &&
                prefab.GetComponent<SetupCustomIngredient>() != null)
            {
                prefabs.Add(prefab);
            }
        }
        return prefabs;
    }

    private static void CleanPrefabs(List<GameObject> prefabs)
    {
        foreach (GameObject prefab in prefabs)
        {
            var component = prefab.GetComponent<SetupCustomIngredient>();
            UnityEngine.Object.DestroyImmediate(component, true);
            EditorUtility.SetDirty(prefab);
        }
        AssetDatabase.SaveAssets();
    }

    private static void RestorePrefabs(List<GameObject> prefabs)
    {
        foreach (GameObject prefab in prefabs)
        {
            if (prefab.GetComponent<SetupCustomIngredient>() != null)
                continue;
            var component = prefab.AddComponent<SetupCustomIngredient>();

            var stub = prefab.GetComponent<SetupCustomIngredientStub>();
            var components = prefab.GetComponents<Component>();
            int componentIndex = Array.IndexOf(components, component);
            int stubIndex = Array.IndexOf(components, stub);
            int moveCount = componentIndex - stubIndex - 1;

            for (int i = 0; i < moveCount; i++)
            {
                if (!ComponentUtility.MoveComponentUp(component))
                    break;
            }
            EditorUtility.SetDirty(prefab);
        }
        AssetDatabase.SaveAssets();
    }




    [MenuItem("Tools/Build AssetBundles (ForceRebuild)", false, 101)]
    static void BuildAllAssetBundlesForceRebuild()
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        if (!TargetSceneSaveValidator.CheckPrepareForBuilding(activeScene))
            return;

        string assetBundleDirectory = "Assets/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                        BuildAssetBundleOptions.ForceRebuildAssetBundle,
                                        BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/Reload Pseudo Assets", false, 10)]
    static void ReloadPseudoAssets()
    {
        PseudoPrefabManager.Instance.DeInit();
        PseudoPrefabManager.Instance.Init();
    }

    [MenuItem("Tools/Toggle Prepare For Building", false, 11)]
    static void TogglePrepareForBuilding()
    {
        PseudoPrefabManager.Instance.prepareForBuilding = !PseudoPrefabManager.Instance.prepareForBuilding;
        if (PseudoPrefabManager.Instance.prepareForBuilding)
        {
            PseudoPrefabManager.Instance.DeInit();
        }
        else
        {
            PseudoPrefabManager.Instance.Init();
        }
    }
}