using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardClassSelector
{
    [SerializeReference, SubclassSelector] ICardClassSelector m_cardClassSelector;
    public List<HaveCardData> HaveCardData => m_cardClassSelector.Execute();
}
public struct HaveCardData
{
    //public CardClassType CardClassType
    //{
    //    get
    //    {
    //        if (CommonCardID != null)
    //            return CardClassType.Common;
    //        else if (OriginalCardID != null)
    //            return CardClassType.Original;
    //        else
    //            return CardClassType.AK;
    //    }
    //}
    //public CommonCardID CommonCardID { get; set; }
    //public OriginalCardID OriginalCardID { get; set; }
    //public AKCardID AKCardID { get; set; }
    public CardClassType CardCalssType { get; set; }
    public int CardID { get; set; }
    public CardUpGrade IsUpGrade { get; set; }
    //public List<int> GetCardID(CardClassType cardClassType)
    //{
    //    List<int> ret = new List<int>();
    //    switch (cardClassType)
    //    {
    //        case CardClassType.Common:
    //            CommonCardID.ForEach(c => ret.Add((int)c));
    //            break;
    //        case CardClassType.Original:
    //            OriginalCardID.ForEach(c => ret.Add((int)c));
    //            break;
    //        case CardClassType.AK:
    //            AKCardID.ForEach(c => ret.Add((int)c));
    //            break;
    //    }
    //    return ret;
    //}
}
public interface ICardClassSelector
{
    List<HaveCardData> Execute();
}
public class CommonCardClass : ICardClassSelector
{
    [SerializeField] List<CommonCardID> m_cardID;
    public List<HaveCardData> Execute()
    {
        List<HaveCardData> ret = new List<HaveCardData>();
        m_cardID.ForEach(c =>
        {
            HaveCardData hcd = new HaveCardData();
            hcd.CardCalssType = CardClassType.Common;
            hcd.CardID = (int)c;
            ret.Add(hcd);
        });
        return ret;
    }
}
public class OriginalCardClass : ICardClassSelector
{
    [SerializeField] List<OriginalCardID> m_cardID;

    public List<HaveCardData> Execute()
    {
        List<HaveCardData> ret = new List<HaveCardData>();
        m_cardID.ForEach(c =>
        {
            HaveCardData hcd = new HaveCardData();
            hcd.CardCalssType = CardClassType.Original;
            hcd.CardID = (int)c;
            ret.Add(hcd);
        });
        return ret;
    }
}
public class AKCardClass : ICardClassSelector
{
    [SerializeField] List<AKCardID> m_cardID;
    public List<HaveCardData> Execute()
    {
        List<HaveCardData> ret = new List<HaveCardData>();
        m_cardID.ForEach(c =>
        {
            HaveCardData hcd = new HaveCardData();
            hcd.CardCalssType = CardClassType.AK;
            hcd.CardID = (int)c;
            ret.Add(hcd);
        });
        return ret;
    }
}
public enum CardClassType
{
    Common,
    Original,
    AK,
}
public enum CardUpGrade
{
    NoUpGrade,
    AsseptUpGrade,
}
