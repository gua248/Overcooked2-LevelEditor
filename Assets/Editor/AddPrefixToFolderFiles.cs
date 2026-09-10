using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 在 Project 窗口中右键文件夹，为当前层所有文件批量添加自定义前缀。
/// </summary>
public static class AddPrefixToFolderFiles
{
    private const string MenuPath = "Assets/Add Prefix To Folder Files";

    [MenuItem(MenuPath, false, 201)]
    private static void AddPrefix()
    {
        if (GetSelectedFolders().Count == 0) return;

        PrefixInputWindow.ShowWindow(prefix =>
        {
            if (string.IsNullOrEmpty(prefix)) return;
            ApplyPrefix(prefix);
        });
    }

    // 只有选中文件夹时菜单才可用
    [MenuItem(MenuPath, true)]
    private static bool ValidateAddPrefix()
    {
        return GetSelectedFolders().Count > 0;
    }

    private static void ApplyPrefix(string prefix)
    {
        List<string> folders = GetSelectedFolders();
        int renamed = 0;

        try
        {
            for (int i = 0; i < folders.Count; i++)
            {
                EditorUtility.DisplayProgressBar("添加前缀", folders[i], (float)i / folders.Count);
                renamed += ProcessFolder(folders[i], prefix);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.Refresh();
        Debug.Log("[AddPrefix] 完成，共重命名 " + renamed.ToString() + " 个文件，前缀：" + prefix);
    }

    /// <summary>处理一个文件夹当前层的文件，不递归子文件夹</summary>
    private static int ProcessFolder(string folderPath, string prefix)
    {
        int renamed = 0;
        string[] files = Directory.GetFiles(folderPath);

        foreach (string file in files)
        {
            string assetPath = file.Replace('\\', '/');

            // .meta 由 Unity 自动跟随重命名，跳过
            if (assetPath.EndsWith(".meta")) continue;

            string fileName = Path.GetFileName(assetPath);

            // 跳过隐藏文件（.DS_Store 等）
            if (fileName.StartsWith(".")) continue;

            // 已有相同前缀则跳过，避免重复叠加
            if (fileName.StartsWith(prefix)) continue;

            string error = AssetDatabase.RenameAsset(assetPath, prefix + fileName);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("[AddPrefix] 重命名失败：" + assetPath + "\n" + error.ToString());
            }
            else
            {
                renamed++;
            }
        }

        return renamed;
    }

    /// <summary>获取当前选中的文件夹路径</summary>
    private static List<string> GetSelectedFolders()
    {
        var result = new List<string>();

        foreach (UnityEngine.Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && AssetDatabase.IsValidFolder(path))
            {
                result.Add(path);
            }
        }

        return result;
    }
}

/// <summary>用于输入前缀的小窗口</summary>
public class PrefixInputWindow : EditorWindow
{
    private string prefix = "";
    private Action<string> onConfirm;

    public static void ShowWindow(Action<string> onConfirm)
    {
        var window = CreateInstance<PrefixInputWindow>();
        window.titleContent = new GUIContent("输入前缀");
        window.onConfirm = onConfirm;
        window.minSize = new Vector2(300, 100);
        window.maxSize = new Vector2(300, 100);
        window.ShowUtility();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("请输入要添加的前缀：");
        prefix = EditorGUILayout.TextField(prefix);

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("确定"))
        {
            if (onConfirm != null)
                onConfirm.Invoke(prefix);
            Close();
        }
        if (GUILayout.Button("取消"))
        {
            Close();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnDestroy()
    {
        onConfirm = null;
    }
}