using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreatePseudoPrefabSOForAsset
{
    [MenuItem("Assets/Create PseudoPrefabSO For Asset", false, 202)]
    private static void Create()
    {
        // 获取选中的任意 Unity 资源（可以是 Prefab, Material, TextAsset 等）
        UnityEngine.Object selectedAsset = Selection.activeObject;
        if (selectedAsset == null) return;

        string assetPath = AssetDatabase.GetAssetPath(selectedAsset);
        if (string.IsNullOrEmpty(assetPath) || AssetDatabase.IsValidFolder(assetPath)) return;

        foreach (Object subAsset in AssetDatabase.LoadAllAssetsAtPath(assetPath))
        {
            if (subAsset is PseudoPrefabSO && AssetDatabase.IsSubAsset(subAsset))
            {
                PseudoPrefabSO pseudoPrefabSO = (PseudoPrefabSO)subAsset;
                if (string.IsNullOrEmpty(pseudoPrefabSO.prefabName) &&
                    string.IsNullOrEmpty(pseudoPrefabSO.bundleName) &&
                    string.IsNullOrEmpty(pseudoPrefabSO.assetPath) &&
                    pseudoPrefabSO.asset == selectedAsset)
                Object.DestroyImmediate(subAsset, true);
            }
        }

        PseudoPrefabSO assetSO = ScriptableObject.CreateInstance<PseudoPrefabSO>();
        assetSO.name = selectedAsset.name + "_so";
        assetSO.asset = selectedAsset;

        // 3. 核心：将 A 作为子资产，强行塞进目标资源的物理文件里
        AssetDatabase.AddObjectToAsset(assetSO, assetPath);

        // 4. 保存刷新
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("已为资源创建 PseudoPrefabSO：" + selectedAsset.name);
    }
}
