using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class RuntimePrefabManager
{
    static List<GameObject> runtimePrefabs = new List<GameObject>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "StartScreen")
        {
            ClearAllRuntimePrefabs();
        }
    }

    public static GameObject CloneAsInactivePrefab(GameObject original)
    {
        original.SetActive(false);
        GameObject prefab = GameObject.Instantiate(original, Vector3.up * 1000, Quaternion.identity);
        SetHideFlagsRecursive(prefab, HideFlags.HideAndDontSave);
        original.SetActive(true);
        runtimePrefabs.Add(prefab);
        return prefab;
    }

    public static void SetHideFlagsRecursive(GameObject gameObject, HideFlags hideFlags)
    {
        Transform[] allTransforms = gameObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in allTransforms)
        {
            t.gameObject.hideFlags = hideFlags;
        }
    }

    public static void ClearAllRuntimePrefabs()
    {
        foreach (var prefab in runtimePrefabs)
            Object.DestroyImmediate(prefab);
        runtimePrefabs.Clear();
    }
}