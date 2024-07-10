#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    [CustomPropertyDrawer(typeof(Model.InventorySlot))]
    public class InventorySlotDrawer : PropertyDrawer
    {
        const float ItemIdWidth = 40;
        const float StackCountWidth = 40;
        const float HorizontalMargin = 10;
        readonly GUIContent _itemIdLabel = new(nameof(Model.InventorySlot.ItemId));
        readonly GUIContent _stackCountLabel = new(nameof(Model.InventorySlot.StackCount));
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, label, false);
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var itemProperty = property.FindPropertyRelative(nameof(Model.InventorySlot.Item));
            var itemIdProperty = property.FindPropertyRelative(nameof(Model.InventorySlot.ItemId));
#pragma warning disable 0618
            var stackCountProperty = property.FindPropertyRelative(nameof(Model.InventorySlot._stackCount));
#pragma warning restore 0618
            GUI.skin.box.CalcMinMaxWidth(label, out _, out var ItemLabelWidth);
            GUI.skin.box.CalcMinMaxWidth(_itemIdLabel, out _, out var ItemIdLabelWidth);
            GUI.skin.box.CalcMinMaxWidth(_stackCountLabel, out _, out var StackCountLabelWidth);
            var itemPropertyWidth = position.width - ItemIdLabelWidth - ItemIdWidth - StackCountLabelWidth - StackCountWidth - HorizontalMargin * 6;
            if (itemPropertyWidth <= 0)
            {
                itemPropertyWidth = position.width;
            }

            int indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUI.indentLevel = 0;

            position.x += HorizontalMargin;

            position.width = itemPropertyWidth;
            EditorGUIUtility.labelWidth = ItemLabelWidth;
            EditorGUI.PropertyField(position, itemProperty, label, false);
            EditorGUIUtility.labelWidth = labelWidth;
            position.x += HorizontalMargin + position.width;

            // ItemId property drawing
            {
                position.width = ItemIdLabelWidth;
                EditorGUIUtility.labelWidth = ItemIdLabelWidth;
                EditorGUI.LabelField(position, _itemIdLabel);
                position.x += HorizontalMargin + position.width;

                position.width = ItemIdWidth;
                GUI.enabled = false;
                EditorGUI.PropertyField(position, itemIdProperty, GUIContent.none, false);
                GUI.enabled = true;
                position.x += HorizontalMargin + position.width;
            }

            //position.width = StackCountLabelWidth;
            //EditorGUI.PrefixLabel(position, 5, new GUIContent(nameof(Model.InventorySlot.StackCount)));
            //EditorGUI.LabelField(position, nameof(Model.InventorySlot.StackCount));
            //position.x += HorizontalMargin + position.width;

            position.width = StackCountLabelWidth + StackCountWidth;
            EditorGUIUtility.labelWidth = StackCountLabelWidth;
            EditorGUI.BeginProperty(position, _stackCountLabel, stackCountProperty);
            {
                EditorGUI.BeginChangeCheck();
                int newVal = EditorGUI.IntField(position, _stackCountLabel, stackCountProperty.intValue);
                if (EditorGUI.EndChangeCheck())
                {
                    stackCountProperty.intValue = newVal;
                }
            }
            EditorGUI.EndProperty();
            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.indentLevel = indent;
        }
    }
}
#endif