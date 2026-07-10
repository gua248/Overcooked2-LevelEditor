using LevelEditor;
using LevelEditorStub;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class BatchCreatePseudoPrefab
{
    [MenuItem("Assets/Batch Create PseudoPrefab", false, 200)]
    public static void CreateSOs()
    {
        string outAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!AssetDatabase.IsValidFolder(outAssetPath))
        {
            EditorUtility.DisplayDialog("错误", "请先选中一个文件夹！", "确定");
            return;
        }
        string soPath = Path.Combine(outAssetPath, "pseudo_prefab_so").Replace("\\", "/");
        string prefabPath = Path.Combine(outAssetPath, "prefabs").Replace("\\", "/");

        if (!Directory.Exists(soPath))
        {
            Directory.CreateDirectory(soPath);
        }
        if (!Directory.Exists(prefabPath))
        {
            Directory.CreateDirectory(prefabPath);
        }

        // 1. 直接弹出系统文件夹选择面板
        // 参数说明：标题、默认打开路径（这里设为工程目录）、文件类型（留空表示选择文件夹）
        string selectedPath = EditorUtility.OpenFolderPanel("选择原资源文件夹", Application.dataPath, "");

        // 如果用户点击了“取消”或者关闭了面板，则直接退出
        if (string.IsNullOrEmpty(selectedPath))
        {
            return;
        }

        // 3. 遍历用户选中路径下的所有文件
        string[] files = Directory.GetFiles(selectedPath, "*.prefab", SearchOption.TopDirectoryOnly);

        int count = 0;
        EditorUtility.DisplayProgressBar("正在处理", "资源 0/" + files.Length.ToString(), 0);
        for (int i = 0; i < files.Length; i++)
        {
            EditorUtility.DisplayProgressBar(
                "正在处理", 
                "资源 " + (count+1).ToString() + "/" + files.Length.ToString(), 
                (float)count / files.Length);

            string file = files[i];
            string standardizedPath = file.Replace('\\', '/');
            int index = standardizedPath.IndexOf("Assets/", System.StringComparison.OrdinalIgnoreCase);

            string assetPath = index != -1 ? standardizedPath.Substring(index) : standardizedPath;
            string nameWithoutExt = Path.GetFileNameWithoutExtension(file);

            // 4. 创建 ScriptableObject 实例
            PseudoPrefabSO soInstance = ScriptableObject.CreateInstance<PseudoPrefabSO>();
            soInstance.assetPath = assetPath;
            soInstance.prefabName = nameWithoutExt;

            // 5. 拼接并在工程内创建该 .asset 资源
            string soSavePath = Path.Combine(soPath, nameWithoutExt + ".asset").Replace("\\", "/");
            AssetDatabase.CreateAsset(soInstance, soSavePath);

            // 5. 在内存中创建一个【临时】游戏物体（不显示在场景中，避免弄乱 Hierarchy）
            GameObject prefab = new GameObject(nameWithoutExt);
            PseudoPrefabStub stub = prefab.AddComponent<PseudoPrefabStub>();
            stub.pseudoPrefabSO = soInstance;
            prefab.AddComponent<PseudoPrefab>();

            string prefabSavePath = Path.Combine(prefabPath, nameWithoutExt + ".prefab").Replace("\\", "/");

            // 9. 核心：将这个配置好的游戏物体直接存为硬盘上的 Prefab
            PrefabUtility.CreatePrefab(prefabSavePath, prefab);
            Object.DestroyImmediate(prefab);

            count++;
        }

        // 6. 刷新编辑器
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 7. 成功提示
        EditorUtility.DisplayDialog("成功", "已成功处理选中的文件夹！\n共生成 " + count.ToString() + " 个 PseudoPrefabSO", "确定");
    }
}