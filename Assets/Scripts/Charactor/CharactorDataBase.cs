using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全キャラクターのベースデータ
/// </summary>
[System.Serializable]
public class CharactorDataBase
{
    [SerializeField] string m_name;
    [SerializeField] int m_maxLife;
    [SerializeField] Sprite m_sprite;
    public string Name => m_name;
    public int MaxLife => m_maxLife;
    public Sprite Sprite => m_sprite;
}
