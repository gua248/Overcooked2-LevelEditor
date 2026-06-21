using LevelEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TargetSceneSaveValidator : UnityEditor.AssetModificationProcessor
{
    private static string[] OnWillSaveAssets(string[] paths)
    {
        // 1. 获取当前正在编辑的活动场景
        Scene activeScene = EditorSceneManager.GetActiveScene();

        // 3. 如果当前活动场景正在保存，深入检查场景内的脚本组件
        if (paths.Contains(activeScene.path))
        {
            // 尝试在当前场景中寻找目标组件 MySceneConfig
            PseudoPrefabManager pseudoPrefabManager = activeScene.GetRootGameObjects()
                .Select(root => root.GetComponent<PseudoPrefabManager>())
                .FirstOrDefault(comp => comp != null);

            // 如果找到了该脚本，且其身上的布尔值为 false，则触发拦截
            if (pseudoPrefabManager != null && !pseudoPrefabManager.prepareForBuilding)
            {
                // 弹出提示
                EditorUtility.DisplayDialog(
                    "错误",
                    "保存场景前先点击 Tools - Toggle Prepare For Building 清除临时物体！",
                    "确定"
                );

                // 过滤掉当前场景，允许其他资源正常保存
                return paths.Where(path => path != activeScene.path).ToArray();
            }
        }

        // 如果不是活动场景、没找到脚本、或者布尔值为 true，一律放行
        return paths;
    }
}