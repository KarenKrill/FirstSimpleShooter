using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Optional<>))]
public class OptionalEditorDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var valueProperty = property.FindPropertyRelative("Value");
        return EditorGUI.GetPropertyHeight(valueProperty);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var valueProperty = property.FindPropertyRelative("Value");
        var enabledProperty = property.FindPropertyRelative("Enabled");
        var enablePropertyWidth = EditorGUI.GetPropertyHeight(enabledProperty) + 6; // width with horizontal indents
        EditorGUI.BeginProperty(position, label, property);
        position.width -= enablePropertyWidth;
        EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
        EditorGUI.PropertyField(position, valueProperty, label, true);
        EditorGUI.EndDisabledGroup();
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        position.x += position.width + enablePropertyWidth;
        position.width = position.height = EditorGUI.GetPropertyHeight(enabledProperty);
        position.x -= position.width;
        EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
