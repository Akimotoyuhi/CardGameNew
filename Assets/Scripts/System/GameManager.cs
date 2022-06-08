using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] BattleManager m_battleManager;

    void Start()
    {
        m_battleManager.Setup();
    }
}
