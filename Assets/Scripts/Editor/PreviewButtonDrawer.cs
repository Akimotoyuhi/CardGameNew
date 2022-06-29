using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(PreviewButtonAttribute))]
public class PreviewButtonDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);
        position.y += 20;
        position.height = 20;
        if (GUI.Button(position, "Preview"))
        {
            CardCustomWindow.ShowWindow();
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true) + 24;//à íuê›íË
    }
}
#endif