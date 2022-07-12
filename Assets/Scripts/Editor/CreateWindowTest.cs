using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
#if UNITY_EDITOR
public class CreateWindowTest : EditorWindow
{
    private bool m_flag;
    private bool m_flag2;
    private int m_popupIndex;
    private string[] m_array = new string[] { "a", "b", "c" };
    private string m_path;

    //任意のタブにTestWindowという項目を追加する　今回はCustomタブを新たに作る
    [MenuItem("Custom/TestWindow")]
    public static void ShowWindow()
    {
        //Windowを表示
        GetWindow(typeof(CreateWindowTest));
    }

    private void OnGUI()
    {
        GUILayout.Label("name", EditorStyles.label);

        m_flag = EditorGUILayout.BeginToggleGroup("", m_flag); //ここからToggleGroupを生成　ここから下は中身の内容
        EditorGUILayout.Toggle("Toggle", false);
        EditorGUILayout.Slider("Slider", 0, 0, 100);
        EditorGUILayout.EndToggleGroup();　　　　　　  //ToggleGroup終わり　これが無いとエラー

        if (GUILayout.Button("ボタン"))
        {
            Debug.Log("クリックされた");
        }
        m_flag2 = EditorGUILayout.Foldout(m_flag2, "aaa");
        if (m_flag2)
        {
            EditorGUI.indentLevel++;
            EditorGUI.LabelField(new Rect(0, 65, 130, 130), "トグルリストを表示");
            //GUILayout.Label("トグルリストを表示", EditorStyles.label);
        }

        Type type = typeof(ICommand);
        //PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
        //foreach (var pro in properties)
        //{
        //    if (GUILayout.Button(pro.Name))
        //    {
        //    }
        //}
        PropertyInfo p = type.GetProperty("CardClassType");
        if (p != null)
        {
            GUILayout.Label(p.Name);
        }
        else
        {
            GUILayout.Label("Null");
        }

        //EditorGUI.BeginChangeCheck();
        if (GUILayout.Button("Open"))
        {
            m_path = EditorUtility.OpenFilePanelWithFilters("画像を設定", Application.dataPath, new string[] { "Image files", "jpg,png" });
        }
        GUILayout.Label(m_path);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(m_path.Substring(m_path.IndexOf("Assets")));
        GUILayout.Box(texture);
        //EditorGUI.EndChangeCheck();

    }

    private void SelectedPopup(int popupIndex)
    {
        m_popupIndex = popupIndex;
        //if (EditorGUI.EndChangeCheck())
        //    Debug.Log(m_array[popupIndex]);
    }
}
#endif