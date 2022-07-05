using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "CardData")]
public class CardData : ScriptableObject
{
    [SerializeField, PreviewButton] CardClassType m_cardClassType;
    [SerializeField] List<CardDataBases> m_dataBases;
    public CardClassType CardClassType => m_cardClassType;
    public List<CardDataBases> DataBases => m_dataBases;
}

[System.Serializable]
public class CardDataBases
{
    [SerializeField] string m_label;
    [SerializeField] CardDataBase m_database;
    [SerializeField] CardDataBase m_upgradeData;
    public string Label => m_label;
    public CardDataBase CardData => m_database;
    public CardDataBase UpgradeData => m_upgradeData;
    public CardDataBase GetCardData(CardUpGrade isUpgrade)
    {
        switch (isUpgrade)
        {
            case CardUpGrade.NoUpGrade:
                return m_database;
            case CardUpGrade.AsseptUpGrade:
                return m_upgradeData;
            default:
                return null;
        }
    }
}

[System.Serializable]
public class CardDataBase
{
    [SerializeField] string m_name;
    [SerializeField] Sprite m_icon;
    [SerializeField] string m_cost;
    [SerializeField, TextArea, Tooltip("�U���͂ɒu��������ꍇ��{dmg0}\n�u���b�N�l�̏ꍇ��{blk0}\n�̗l�ɋL�q���邱��")]
    string m_tooltip;
    [SerializeField] UseType m_cardUseType = UseType.None;
    [SerializeField] Rarity m_rarity;
    [SerializeField] CardType m_cardType;
    [SerializeField] CommandSelect m_cardCommands;
    public string Name => m_name;
    public Sprite Icon => m_icon;
    public string Cost => m_cost;
    public string Tooltip => m_tooltip;
    public UseType CardUseType => m_cardUseType;
    public Rarity Rarity => m_rarity;
    public CardType CardType => m_cardType;
    public CommandSelect CardCommands => m_cardCommands;
}

#region enums
/// <summary>�S�L�������ʂ̃J�[�h��ID</summary>
public enum CommonCardID
{

}
/// <summary>�I���W�i���L�����̃J�[�hID</summary>
public enum OriginalCardID
{
    /// <summary>�a��</summary>
    Slashing,
    /// <summary>�h��</summary>
    Defence,
}
/// <summary></summary>
public enum AKCardID
{
    /// <summary>�X�g���C�N</summary>
    Strike,
    /// <summary>�h��</summary>
    Defence,
    /// <summary>�^�[�~�l�[�g</summary>
    TerminationOfTactics
}
/// <summary>���A���e�B</summary>
public enum Rarity
{
    Normal,
    Rare,
    SuperRare,
    UltimateRare,
    Special,
    BadState,
    Curse,
}
/// <summary>�ǂ��������p�r�̃J�[�h�Ȃ̂���\��������</summary>
public enum CardType
{
    Attack,
    Block,
    Buff,
    Debuff,
    SelfInjury,
}

public enum UseType
{
    None = -1,
    Player,
    Enemy,
    AllEnemies,
    System,
}
#endregion