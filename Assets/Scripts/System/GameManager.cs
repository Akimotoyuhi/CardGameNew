using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] BattleManager m_battleManager;
    [SerializeField] GUIManager m_guiManager;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        m_battleManager.Setup();
        m_guiManager.Setup();
    }
}
