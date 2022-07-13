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
    private float m_buttonWidthSize = 200;

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
            m_toggleFlag[i] = EditorGUILayout.Foldout(m_toggleFlag[i], m_carddata.DataBases[i].m_label);
            if (m_toggleFlag[i])
            {
                //トグルの中身
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("強化前"))
                    {
                        Debug.Log(m_carddata.DataBases[i].CardData.Name);
                        CardCustomWindow.ShowWindow(m_carddata.DataBases[i].CardData, m_carddata.GetRaritySprite, m_carddata.GetTypeSprite);
                    }
                    if (GUILayout.Button("強化後"))
                    {
                        Debug.Log(m_carddata.DataBases[i].CardData.Name);
                        CardCustomWindow.ShowWindow(m_carddata.DataBases[i].UpgradeData, m_carddata.GetRaritySprite, m_carddata.GetTypeSprite);
                    }
                    if (GUILayout.Button("Remove"))
                    {
                        RemoveData(m_carddata.DataBases[i]);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        if (GUILayout.Button("AddNewCard"))
        {
            AddNewCard();
        }
    }

    private static void AddNewCard()
    {
        m_carddata.DataBases.Add(new CardDataBases());
    }

    private static void RemoveData(CardDataBases data)
    {
        m_carddata.DataBases.Remove(data);
    }
}
#endif