using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "CardData")]
public class CardData : ScriptableObject
{
    [SerializeField] CardClassType m_cardClassType;
    [SerializeField] List<CardDataBase> m_dataBases;
    private Subject<CardData> m_cardEditSubject = new Subject<CardData>();
    public CardClassType CardClassType => m_cardClassType;
    public List<CardDataBase> DataBases => m_dataBases;
    public Subject<CardData> CardEditSubject => m_cardEditSubject;
}

[System.Serializable]
public class CardDataBase
{
    [SerializeField] string m_name;
    [SerializeField] Sprite m_icon;
    [SerializeField] string m_cost;
    [SerializeField, TextArea, Tooltip("攻撃力に置き換える場合は{pow0}\nブロック値の場合は{blk0}\nの様に記述すること")]
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
/// <summary>全キャラ共通のカードのID</summary>
public enum CommonCardID
{

}
/// <summary>オリジナルキャラのカードID</summary>
public enum OriginalCardID
{
    /// <summary>斬撃</summary>
    Slashing,
    /// <summary>防御</summary>
    Defence,
}
/// <summary></summary>
public enum AKCardID
{
    /// <summary>ストライク</summary>
    Strike,
    /// <summary>防御</summary>
    Defence,
    /// <summary>ターミネート</summary>
    TerminationOfTactics
}
/// <summary>レアリティ</summary>
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
/// <summary>どういった用途のカードなのかを表したもの</summary>
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