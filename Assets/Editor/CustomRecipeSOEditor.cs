using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomRecipeSO), true)]
public class CustomRecipeSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty recipeType = serializedObject.FindProperty("type");

        // 获取序列化属性的迭代器
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        // 顺着脚本里的字段顺序，从上往下挨个遍历绘制
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false; // 只遍历顶层字段，不进入子属性内部

            // 跳过 Unity 默认自带的 "m_Script" 脚本路径字段
            if (iterator.name == "m_Script")
            {
                continue;
            }

            if (ShouldHideTypeField((CustomRecipeSO.RecipeType)recipeType.intValue, iterator.name))
            {
                continue;
            }

            // 正常绘制当前字段（保留原本的 Header 和 Space 装饰）
            EditorGUILayout.PropertyField(iterator, true);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private static readonly string[] cookedFields = new string[] { "cookingStepSO", "cookingStepIconSO", "cookingStepIcon", "cookingProgress" };
    private static readonly string[] mixedFields = new string[] { "mixingIconSO", "mixingIcon", "mixingProgress" };
    private static bool ShouldHideTypeField(CustomRecipeSO.RecipeType recipeType, string fieldName)
    {
        switch (recipeType)
        {
            case CustomRecipeSO.RecipeType.Ingredient:
            case CustomRecipeSO.RecipeType.Item:
                return fieldName == "optionalSOs" || cookedFields.Contains(fieldName) || mixedFields.Contains(fieldName);
            case CustomRecipeSO.RecipeType.Composite:
                return cookedFields.Contains(fieldName) || mixedFields.Contains(fieldName);
            case CustomRecipeSO.RecipeType.Cooked:
                return mixedFields.Contains(fieldName);
            case CustomRecipeSO.RecipeType.Mixed:
                return cookedFields.Contains(fieldName);
            default:
                return false;
        }
    }
}