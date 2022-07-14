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
    public CardClassType CardCalssType { get; set; }
    public int CardID { get; set; }
    public CardUpGrade IsUpGrade { get; set; }
    public void Setup(CardClassType cardClassType, int id, CardUpGrade cardUpGrade)
    {
        CardCalssType = cardClassType;
        CardID = id;
        IsUpGrade = cardUpGrade;
    }
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
