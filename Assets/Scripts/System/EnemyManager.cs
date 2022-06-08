using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] Enemy m_enemyPrefab;
    private List<Enemy> m_currentEnemies = new List<Enemy>();
    public void Setup()
    {

    }
}
