using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public class CreateWindowTest : EditorWindow
{
    private bool m_flag;

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
        
        m_flag = EditorGUILayout.BeginToggleGroup("", m_flag);　//ここからToggleGroupを生成　ここから下は中身の内容
            EditorGUILayout.Toggle("Toggle", false);
            EditorGUILayout.Slider("Slider", 0, 0, 100);
        EditorGUILayout.EndToggleGroup();　　　　　　  //ToggleGroup終わり　これが無いとエラー

        if (GUILayout.Button("ボタン"))
        {
            Debug.Log("クリックされた");
        }
    }
}
#endif