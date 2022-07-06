using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;

public class CardDataSelectWindow : EditorWindow
{
    private static CardData m_carddata;
    private List<bool> m_toggleFlag = new List<bool>();

    public static void ShowWindow(CardData cardData)
    {
        m_carddata = cardData;
        GetWindow(typeof(CardDataSelectWindow));
    }

    private void OnGUI()
    {
        if (m_carddata == null)
            return;
        for (int i = 0; i < m_carddata.DataBases.Count; i++)
        {
            bool flag = false;
            m_toggleFlag.Add(flag);
            m_toggleFlag[i] = EditorGUILayout.Foldout(m_toggleFlag[i], m_carddata.DataBases[i].Label);
            if (m_toggleFlag[i])
            {
                if (GUILayout.Button("‹­‰»‘O"))
                {
                    CardCustomWindow.ShowWindow(m_carddata.DataBases[i].CardData, m_carddata.GetRaritySprite, m_carddata.GetTypeSprite);
                }
                if (GUILayout.Button("‹­‰»Œã"))
                {
                    CardCustomWindow.ShowWindow(m_carddata.DataBases[i].UpgradeData, m_carddata.GetRaritySprite, m_carddata.GetTypeSprite);
                }
            }
        }
    }
}
#endif