using UnityEditor;
using UnityEngine;


public class AssetBundleTools
{
    [MenuItem("Assets/Clean Folder Bundle", false, 102)]
    static void CleanFolderBundle()
    {
        // 1. 获取选中的文件夹路径
        string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            EditorUtility.DisplayDialog("错误", "请先选中一个文件夹！", "确定");
            return;
        }

        // 2. 清空内部所有文件的标记（递归）
        string[] allAssetPaths = AssetDatabase.FindAssets("", new[] { folderPath });
        foreach (string guid in allAssetPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // 排除文件夹本身，只处理内容
            if (path != folderPath)
            {
                if (path.EndsWith(".unity"))
                {
                    Debug.Log("跳过场景文件: " + path);
                    continue;
                }
                AssetImporter importer = AssetImporter.GetAtPath(path);
                if (importer != null && !string.IsNullOrEmpty(importer.assetBundleName))
                {
                    importer.assetBundleName = ""; // 清空标记
                }
            }
        }

        // 3. 清理无效的包名并刷新
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("完成", "文件夹内部标记已清空！", "好的");
    }
}