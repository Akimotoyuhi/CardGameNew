using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData")]
public class CardData : ScriptableObject
{
    [SerializeField] List<CardDataBase> m_dataBases;
    public List<CardDataBase> DataBases => m_dataBases;
}

[System.Serializable]
public class CardDataBase
{
    [SerializeField] string m_name;
    [SerializeField] Sprite m_icon;
    [SerializeField] string m_cost;
    [SerializeField, TextArea] string m_tooltip;
    public string Name => m_name;
    public Sprite Icon => m_icon;
    public string Cost => m_cost;
    public string Tooltip => m_tooltip;
}
