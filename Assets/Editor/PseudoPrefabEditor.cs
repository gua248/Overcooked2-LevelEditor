using LevelEditor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PseudoPrefab), true)]
public class PseudoPrefabEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 1. 先绘制默认的 Inspector 内容
        DrawDefaultInspector();

        // 2. 底部加一段间距和按钮
        EditorGUILayout.Space();

        if (GUILayout.Button("Reload"))
        {
            // 支持多选：遍历所有选中的对象
            foreach (Object obj in targets)
            {
                PseudoPrefab pseudoPrefab = obj as PseudoPrefab;
                if (pseudoPrefab == null) continue;

                pseudoPrefab.ResetChild();
            }
        }
    }
}