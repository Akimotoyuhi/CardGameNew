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
    [SerializeField] UseType m_cardUseType = UseType.None;
    [SerializeField] CommandSelect m_cardCommands;
    public string Name => m_name;
    public Sprite Icon => m_icon;
    public string Cost => m_cost;
    public string Tooltip => m_tooltip;
    public UseType CardUseType => m_cardUseType;
    public CommandSelect CardCommands => m_cardCommands;
}

[System.Serializable]
public class CommandSelect
{
    [SerializeReference, SubclassSelector] List<ICommand> m_commands;
    public List<Command> Execute()
    {
        var ret = new List<Command>();
        m_commands.ForEach(c => ret.Add(c.Execute()));
        return ret;
    }
}

public interface ICommand
{
    Command Execute();
}

public class AttackCommand : ICommand
{
    [SerializeField] UseType m_useType;
    [SerializeField] int m_power;
    [SerializeField] bool m_isTrueDamage;

    public Command Execute()
    {
        Command ret = new Command();
        ret.Power = m_power;
        ret.UseType = m_useType;
        return ret;
    }
}

public class BlockCommand : ICommand
{
    [SerializeField] UseType m_useType;
    [SerializeField] int m_block;
    [SerializeField] bool isTrueBlock;

    public Command Execute()
    {
        Command ret = new Command();
        ret.Block = m_block;
        ret.UseType = m_useType;
        return ret;
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
