using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public class CreateWindowTest : EditorWindow
{
    //�C�ӂ̃^�u��TestWindow�Ƃ������ڂ�ǉ�����@�����Custom�^�u��V���ɍ��
    [MenuItem("Custom/TestWindow")]
    public static void ShowWindow()
    {
        //Window��\��
        GetWindow(typeof(CreateWindowTest));
    }

    private void OnGUI()
    {
        GUILayout.Label("name", EditorStyles.label);
        
        EditorGUILayout.BeginToggleGroup("", false);�@//��������ToggleGroup�𐶐��@�������牺�͒��g�̓��e
            EditorGUILayout.Toggle("Toggle", false);
            EditorGUILayout.Slider("Slider", 0, 0, 100);
        EditorGUILayout.EndToggleGroup();�@�@�@�@�@�@ //ToggleGroup�I���@���ꂪ�����ƃG���[

        if (GUILayout.Button("�{�^��"))
        {
            Debug.Log("�N���b�N���ꂽ");
        }
    }
}
#endif