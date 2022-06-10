using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

public class GUIManager : MonoBehaviour
{
    [Header("í“¬‰æ–Ê")]
    [SerializeField] BattleManager m_battleManager;
    [SerializeField] Button m_turnEndButton;

    public void Setup()
    {
        m_turnEndButton.onClick.AddListener(() => m_battleManager.OnBattle());
    }
}
