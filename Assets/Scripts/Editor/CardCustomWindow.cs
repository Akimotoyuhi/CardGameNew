using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CardData))]
public class CardEditButton : Editor
{
    [System.Serializable]
    public class DataBinder
    {
        [SerializeReference]
        public CardData CardData;
    }
    private SerializedObject m_target;
    private CardDataBase m_database = null;

    private void OnEnable()
    {
        var node = target as CardData;
        if (m_database == null)
        {
            //m_database = ScriptableObject.CreateInstance<CardDataBase>();
        }
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CardData cardDataBase = target as CardData;

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