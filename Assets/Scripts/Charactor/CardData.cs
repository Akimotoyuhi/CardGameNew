using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

/// <summary>
/// 全てのカードデータ
/// </summary>
[CreateAssetMenu(fileName = "CardData")]
public class CardData : ScriptableObject
{
    [SerializeField, PreviewButton] CardClassType m_cardClassType;
    [SerializeField] List<RaritySprite> m_raritySprite;
    [SerializeField] List<TypeSprite> m_typeSprite;
    [SerializeField] List<Provality> m_cardRewordProvality;
    [SerializeField] List<CardDataBases> m_dataBases;
    public CardClassType CardClassType => m_cardClassType;
    public List<RaritySprite> GetRaritySprite => m_raritySprite;
    public List<TypeSprite> GetTypeSprite => m_typeSprite;
    public List<CardDataBases> DataBases => m_dataBases;
    
    /// <summary>特定のレア度からランダムに任意の枚数取得</summary>
    public List<CardDataBase> GetCardDatas(int num, Rarity rarity, CardUpGrade cardUpGrade)
    {
        var dataList = m_dataBases.Where(t => t.GetCardData(cardUpGrade).Rarity == rarity).ToList();
        var nums = ToNumListNoCover(dataList.Count, num); //被りが無いように
        List<CardDataBase> ret = new List<CardDataBase>();
        nums.ForEach(i => ret.Add(dataList[i].GetCardData(cardUpGrade)));
        return ret;
    }

    /// <summary>戦った敵のタイプからランダムに任意の枚数取得</summary>
    public List<CardDataBase> GetCardDatas(int num, BattleType battleType, CardUpGrade cardUpGrade)
    {
        List<CardDataBase> ret = new List<CardDataBase>();
        Rarity r = Rarity.Normal;
        for (int i = 0; i < num; )
        {
            bool coveringFlag = false;
            foreach (var p in m_cardRewordProvality)
            {
                if (p.BattleType == battleType)
                {
                    r = p.Lottery();
                    break;
                }
            }
            //被りが無いように
            var addList = GetCardDatas(1, r, cardUpGrade);
            foreach (var re in ret)
            {
                if (re.Name == addList[0].Name)
                {
                    coveringFlag = true;
                    break;
                }
            }
            if (coveringFlag)
                continue;
            ret.AddRange(addList);
            i++;
        }
        return ret;
    }

    /// <summary>
    /// 被りなしの整数の配列を返す
    /// </summary>
    /// <param name="range">範囲</param>
    /// <param name="element">要素数</param>
    /// <returns>被りなしの整数のリスト</returns>
    private List<int> ToNumListNoCover(int range, int element)
    {
        List<int> vs = new List<int>();
        for (int i = 0; i < element;)
        {
            int r = Random.Range(0, range);
            bool b = true;
            foreach (var v in vs)
            {
                if (v == r)
                    b = false;
            }
            if (b)
            {
                vs.Add(r);
                i++;
            }
        }
        return vs;
    }

    /// <summary>レアリティに応じた画像</summary>
    [System.Serializable]
    public class RaritySprite
    {
        [SerializeField] Rarity m_rarity;
        [SerializeField] Sprite m_sprite;
        public Rarity Rarity => m_rarity;
        public Sprite Sprite => m_sprite;
    }
    /// <summary>CardTypeに応じた画像</summary>
    [System.Serializable]
    public class TypeSprite
    {
        [SerializeField] CardType m_cardType;
        [SerializeField] Sprite m_sprites;
        public CardType CardType => m_cardType;
        public Sprite Sprite => m_sprites;
    }
    /// <summary>報酬で出る際のレア度毎の出現確率</summary>
    [System.Serializable]
    public class Provality
    {
        [SerializeField] BattleType m_battleType;
        [SerializeField, Range(0, 100)] int m_normalCard;
        [SerializeField, Range(0, 100)] int m_rareCard;
        [SerializeField, Range(0, 100)] int m_superRareCard;
        public BattleType BattleType => m_battleType;
        /// <summary>抽選</summary>
        public Rarity Lottery()
        {
            int r = Random.Range(0, 100);
            if (r <= m_superRareCard)
                return Rarity.SuperRare;
            if (r <= m_rareCard)
                return Rarity.Rare;
            else
                return Rarity.Normal;
        }
    }
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
    [SerializeField, TextArea, Tooltip("攻撃力に置き換える場合は{dmg0}\nブロック値の場合は{blk0}\nの様に記述すること")]
    string m_tooltip;
    [SerializeField] UseType m_cardUseType = UseType.None;
    [SerializeField] Rarity m_rarity;
    [SerializeField] List<CardType> m_cardType;
    [SerializeField] CommandSelect m_cardCommands;
    public string Name => m_name;
    public Sprite Icon => m_icon;
    public string Cost => m_cost;
    public string Tooltip => m_tooltip;
    public UseType CardUseType => m_cardUseType;
    public Rarity Rarity => m_rarity;
    public List<CardType> CardType => m_cardType;
    public CommandSelect CardCommands => m_cardCommands;
    public void SetData(string name, Sprite icon, string cost, string tooltip, UseType useType, Rarity rarity, List<CardType> cardTypes)
    {
        m_name = name;
        m_icon = icon;
        m_cost = cost;
        m_tooltip = tooltip;
        m_rarity = rarity;
        m_cardUseType = useType;
        m_cardType = cardTypes;
    }
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
    TerminationOfTactics,
    /// <summary>ハンマリングオン</summary>
    HammerOn,
}
/// <summary>レアリティ</summary>
public enum Rarity
{
    Normal,
    Rare,
    SuperRare,
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