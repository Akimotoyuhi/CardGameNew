using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CardData))]
public class CardEditButton : Editor
{
    private SerializedProperty m_cardDatabase;
    private void OnEnable()
    {
        m_cardDatabase = serializedObject.FindProperty("");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //CardDataBase cardDataBase = target as CardDataBase;

        if (GUILayout.Button("Edit"))
        {
            CardCustomWindow.ShowWindow();
        }
    }
}

public class CardCustomWindow : EditorWindow
{
    [MenuItem("Custom/CardEdit")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CardCustomWindow));
    }

    private void OnGUI()
    {
        
    }
}
#endif