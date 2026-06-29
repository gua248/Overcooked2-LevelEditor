using LevelEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TargetSceneSaveValidator : UnityEditor.AssetModificationProcessor
{
    public static bool CheckPrepareForBuilding(Scene activeScene)
    {
        PseudoPrefabManager pseudoPrefabManager = activeScene.GetRootGameObjects()
            .Select(root => root.GetComponent<PseudoPrefabManager>())
            .FirstOrDefault(comp => comp != null);

        if (pseudoPrefabManager != null && !pseudoPrefabManager.prepareForBuilding)
        {
            // 弹出提示
            EditorUtility.DisplayDialog(
                "错误",
                "保存或构建场景前先点击 Tools - Toggle Prepare For Building 清除临时物体！",
                "确定"
            );

            return false;
        }
        return true;
    }

    private static string[] OnWillSaveAssets(string[] paths)
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();

        if (paths.Contains(activeScene.path))
        {
            bool prepareForBuilding = CheckPrepareForBuilding(activeScene);
            if (!prepareForBuilding)
            {
                return paths.Where(path => path != activeScene.path).ToArray();
            }
        }

        // 如果不是活动场景、没找到脚本、或者布尔值为 true，一律放行
        return paths;
    }
}