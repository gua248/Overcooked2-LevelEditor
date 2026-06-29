using LevelEditor;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
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
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                        BuildAssetBundleOptions.None,
                                        BuildTarget.StandaloneWindows);
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