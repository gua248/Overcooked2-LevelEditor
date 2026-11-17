using LevelEditorStub;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PseudoPrefabSOCategoryAttribute))]
public class CategoryDrawer : PropertyDrawer
{
    public override void OnGUI(
        Rect position, SerializedProperty property, GUIContent label)
    {
        System.Type objectType = fieldInfo.FieldType;

        if (objectType.IsArray)
        {
            objectType = objectType.GetElementType();
        }
        else if (objectType.IsGenericType &&
                 objectType.GetGenericTypeDefinition() ==
                 typeof(System.Collections.Generic.List<>))
        {
            objectType = objectType.GetGenericArguments()[0];
        }

        string[] categories = fieldInfo
            .GetCustomAttributes(typeof(PseudoPrefabSOCategoryAttribute), true)
            .Cast<PseudoPrefabSOCategoryAttribute>()
            .Select(a => a.Name)
            .Distinct()
            .ToArray();

        string searchFilter = string.Join(
            " ",
            categories.Select(c => "l:" + c).ToArray());
        if (!string.IsNullOrEmpty(searchFilter))
            searchFilter += " ";

        EditorGUI.BeginProperty(position, label, property);

        Rect fieldRect = EditorGUI.PrefixLabel(position, label);

        // fieldRect 已经处理过缩进
        int previousIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        int pickerID = GUIUtility.GetControlID(
            FocusType.Passive, fieldRect);

        // Unity 2017 IMGUI 对象字段右侧的选择按钮区域
        Rect pickerRect = new Rect(
            fieldRect.xMax - 19f,
            fieldRect.y,
            19f,
            fieldRect.height);

        Event e = Event.current;

        // 接收自定义筛选条件的原生选择器结果
        if (e.type == EventType.ExecuteCommand &&
            EditorGUIUtility.GetObjectPickerControlID() == pickerID)
        {
            if (e.commandName == "ObjectSelectorUpdated")
            {
                Object selected = EditorGUIUtility.GetObjectPickerObject();

                if (selected == null || objectType.IsInstanceOfType(selected))
                    if (Accepts(selected, categories))
                        property.objectReferenceValue = selected;

                e.Use();
            }
            else if (e.commandName == "ObjectSelectorClosed")
            {
                // 关闭时不赋值，也不交给原生 ObjectField 处理。
                e.Use();
            }
        }

        // 为原生 ObjectField 设置名称，以判断它是否获得键盘焦点
        string controlName = "CategoryField_" + pickerID;

        bool openByKeyboard =
            GUI.enabled &&
            e.type == EventType.KeyDown &&
            (e.keyCode == KeyCode.Return ||
             e.keyCode == KeyCode.KeypadEnter) &&
            GUI.GetNameOfFocusedControl() == controlName;

        bool openByMouse =
            GUI.enabled &&
            e.type == EventType.MouseDown &&
            e.button == 0 &&
            pickerRect.Contains(e.mousePosition);

        // 在 ObjectField 处理事件前，拦截小圆点点击和回车
        if (openByMouse || openByKeyboard)
        {
            var method = typeof(EditorGUIUtility).GetMethod(
                "ShowObjectPicker",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static);

            method.MakeGenericMethod(objectType).Invoke(null, new object[]
            {
                property.objectReferenceValue,
                false,
                searchFilter,
                pickerID
            });

            e.Use();
        }

        bool previousMixedValue = EditorGUI.showMixedValue;
        EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

        EditorGUI.BeginChangeCheck();

        GUI.SetNextControlName(controlName);

        Object value = EditorGUI.ObjectField(
            fieldRect,
            GUIContent.none,
            property.objectReferenceValue,
            objectType,
            false);

        // 原生拖拽、清空等操作也要经过标签检查
        if (EditorGUI.EndChangeCheck())
            if (value == null || objectType.IsInstanceOfType(value))
                if (Accepts(value, categories))
                    property.objectReferenceValue = value;

        EditorGUI.showMixedValue = previousMixedValue;

        EditorGUI.indentLevel = previousIndent;
        EditorGUI.EndProperty();
    }

    private static bool Accepts(Object asset, string[] categories)
    {
        if (asset == null)
            return true;

        string path = AssetDatabase.GetAssetPath(asset);
        // common 以外的资源不检查 label
        if (!path.StartsWith("Assets/common"))
            return true;

        Object mainAsset = AssetDatabase.LoadMainAssetAtPath(path);

        if (mainAsset == null)
            return false;

        string[] labels = AssetDatabase.GetLabels(mainAsset);

        foreach (string assetLabel in labels)
        {
            foreach (string category in categories)
            {
                if (string.Equals(
                    assetLabel,
                    category,
                    System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }
}