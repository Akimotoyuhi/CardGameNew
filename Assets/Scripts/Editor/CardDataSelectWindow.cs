using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class CardDataSelectWindow : EditorWindow
{
    private static CardData m_carddata;

    public static void ShowWindow(CardData cardData)
    {
        m_carddata = cardData;
        GetWindow(typeof(CardDataSelectWindow));
    }

    private void OnGUI()
    {
        for (int i = 0; i < m_carddata.DataBases.Count; i++)
        {
            if (GUILayout.Button(m_carddata.DataBases[i].Label))
            {
                CardCustomWindow.ShowWindow(m_carddata.DataBases[i]);
            }
        }
    }
}
#endif