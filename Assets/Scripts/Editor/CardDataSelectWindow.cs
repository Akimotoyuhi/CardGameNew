using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;

public class CardDataSelectWindow : EditorWindow
{
    private static CardData m_carddata;
    private static List<bool> m_toggleFlag = new List<bool>();
    private CardDataSelectWindowState m_windowState;

    public static void ShowWindow(CardData cardData)
    {
        m_carddata = cardData;
        GetWindow(typeof(CardDataSelectWindow));
    }

    private void OnGUI()
    {
        if (m_carddata == null)
            return;

        GUILayout.BeginHorizontal();
        {
            //カードリスト表示モードとラベル編集モードの切り替え
            switch (m_windowState)
            {
                case CardDataSelectWindowState.CardList:
                    GUILayout.Label("CardList");
                    if (GUILayout.Button("LabelEdit"))
                    {
                        m_windowState = CardDataSelectWindowState.LabelEdit;
                    }
                    break;
                case CardDataSelectWindowState.LabelEdit:
                    GUILayout.Label("LabelEdit");
                    if (GUILayout.Button("ViewCardList"))
                    {
                        m_windowState = CardDataSelectWindowState.CardList;
                    }
                    break;
                default:
                    break;
            }
        }
        GUILayout.EndHorizontal();

        switch (m_windowState)
        {
            case CardDataSelectWindowState.CardList:
                //カードリストの表示
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
                            if (GUILayout.Button("削除"))
                            {
                                RemoveData(i);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                break;
            case CardDataSelectWindowState.LabelEdit:
                //ラベル編集モード
                for (int i = 0; i < m_carddata.DataBases.Count; i++)
                {
                    m_carddata.DataBases[i].m_label = GUILayout.TextField(m_carddata.DataBases[i].m_label);
                }
                break;
            default:
                break;
        }
        if (GUILayout.Button("AddNewCard"))
        {
            AddNewCard();
        }
    }

    private static void AddNewCard()
    {
        m_carddata.DataBases.Add(new CardDataBases());
        m_toggleFlag.Add(false);
    }

    private static void RemoveData(int index)
    {
        m_toggleFlag.RemoveAt(index);
        m_carddata.DataBases.RemoveAt(index);
    }
}

public enum CardDataSelectWindowState
{
    CardList,
    LabelEdit,
}
#endif