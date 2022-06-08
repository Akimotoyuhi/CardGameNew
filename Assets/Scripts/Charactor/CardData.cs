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
    [SerializeReference, SubclassSelector] List<ICommand> m_cardCommand;
    public string Name => m_name;
    public Sprite Icon => m_icon;
    public string Cost => m_cost;
    public string Tooltip => m_tooltip;
    public List<ICommand> CardCommand => m_cardCommand;
}

public interface ICommand
{
    void Execute();
}

public class AttackCommand : ICommand
{
    [SerializeField] UseType m_useType;
    [SerializeField] int m_power;
    [SerializeField] bool m_isTrueDamage;

    public void Execute()
    {

    }
}

public enum UseType
{
    None = -1,
    Player,
    Enemy,
    AllEnemies,
    System,
}
