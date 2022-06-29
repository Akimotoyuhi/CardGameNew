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
        //Editボタンが押されたらwindowを開く
        if (GUI.Button(position, "Edit"))
        {
            var v = property.serializedObject.FindProperty("m_name");
            Debug.Log(v);
            //CardCustomWindow.ShowWindow();
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true) + 24;//位置設定
    }
}
#endif