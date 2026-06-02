using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


public class ForceReserializeAssets
{
    [MenuItem("Assets/Force Reserialize Assets", false, 103)]
    public static void ForceSaveSelected()
    {
        // 获取所有选中的资源路径
        string[] selectedGuids = Selection.assetGUIDs;
        List<string> assetPaths = new List<string>();

        foreach (var guid in selectedGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // 如果是文件夹，则找出其下所有资源
            if (AssetDatabase.IsValidFolder(path))
            {
                // 搜索该文件夹下的所有资源 (t:Object 代表所有类型)
                string[] subAssets = AssetDatabase.FindAssets("t:Object", new[] { path });
                foreach (var subGuid in subAssets)
                {
                    assetPaths.Add(AssetDatabase.GUIDToAssetPath(subGuid));
                }
            }
            else
            {
                assetPaths.Add(path);
            }
        }

        if (assetPaths.Count > 0)
        {
            // 核心命令：只对这些选中的路径及其子物体进行重新序列化
            AssetDatabase.ForceReserializeAssets(assetPaths);
            EditorUtility.DisplayDialog("完成", "已强制重序列化 " + assetPaths.Count.ToString() + " 个选中项！", "好的");
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "请先选中资源！", "确定");
        }
    }
}
