using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardClassSelector
{
    [SerializeReference, SubclassSelector] ICardClassSelector m_cardClassSelector;
    public CardClass CardClass => m_cardClassSelector.Execute();
}
public struct CardClass
{
    public CardClassType CardClassType
    {
        get
        {
            if (CommonCardID != null)
                return CardClassType.Common;
            else if (OriginalCardID != null)
                return CardClassType.Original;
            else
                return CardClassType.AK;
        }
    }
    public List<CommonCardID> CommonCardID { get; set; }
    public List<OriginalCardID> OriginalCardID { get; set; }
    public List<AKCardID> AKCardID { get; set; }
    public List<int> GetCardID(CardClassType cardClassType)
    {
        List<int> ret = new List<int>();
        switch (cardClassType)
        {
            case CardClassType.Common:
                CommonCardID.ForEach(c => ret.Add((int)c));
                break;
            case CardClassType.Original:
                OriginalCardID.ForEach(c => ret.Add((int)c));
                break;
            case CardClassType.AK:
                AKCardID.ForEach(c => ret.Add((int)c));
                break;
        }
        return ret;
    }
}
public interface ICardClassSelector
{
    CardClass Execute();
    CardClassType CardClassType { get; }
}
public class CommonCardClass : ICardClassSelector
{
    [SerializeField] List<CommonCardID> m_cardID;
    public CardClassType CardClassType => CardClassType.Common;
    public CardClass Execute()
    {
        CardClass ret = new CardClass();
        ret.CommonCardID = m_cardID;
        return ret;
    }
}
public class OriginalCardClass : ICardClassSelector
{
    [SerializeField] List<OriginalCardID> m_cardID;
    public CardClassType CardClassType => CardClassType.Original;
    public CardClass Execute()
    {
        CardClass ret = new CardClass();
        ret.OriginalCardID = m_cardID;
        return ret;
    }
}
public class AKCardClass : ICardClassSelector
{
    [SerializeField] List<AKCardID> m_cardID;
    public CardClassType CardClassType => CardClassType.AK;
    public CardClass Execute()
    {
        CardClass ret = new CardClass();
        ret.AKCardID = m_cardID;
        return ret;
    }
}
public enum CardClassType
{
    Common,
    Original,
    AK,
}
