using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �L�����N�^�[���̃J�[�h���X�g��ݒ�o����悤�ɂ���N���X
/// </summary>
[System.Serializable]
public class CardClassSelector
{
    [SerializeReference, SubclassSelector] ICardClassSelector m_cardClassSelector;
    public List<HaveCardData> HaveCardData => m_cardClassSelector.Execute();
}
/// <summary>
/// �ǂ̃J�[�h�N���X�̂ǂ̃J�[�h�����w�肷��ׂ̍\����
/// </summary>
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
    [SerializeField] List<CardIDSelector> m_cardIDSelector;
    public List<HaveCardData> Execute()
    {
        List<HaveCardData> ret = new List<HaveCardData>();
        m_cardIDSelector.ForEach(c =>
        {
            HaveCardData hcd = new HaveCardData();
            hcd.CardCalssType = CardClassType.Common;
            hcd.CardID = (int)c.CardID;
            hcd.IsUpGrade = c.CardUpGrade;
            ret.Add(hcd);
        });
        return ret;
    }

    [System.Serializable]
    public class CardIDSelector
    {
        [SerializeField] CommonCardID m_cardid;
        [SerializeField] bool m_isUpgrade;
        public CommonCardID CardID => m_cardid;
        public CardUpGrade CardUpGrade => m_isUpgrade ? CardUpGrade.AsseptUpGrade : CardUpGrade.NoUpGrade;
    }
}
public class OriginalCardClass : ICardClassSelector
{
    [SerializeField] List<CardIDSelector> m_cardIDSelector;

    public List<HaveCardData> Execute()
    {
        List<HaveCardData> ret = new List<HaveCardData>();
        m_cardIDSelector.ForEach(c =>
        {
            HaveCardData hcd = new HaveCardData();
            hcd.CardCalssType = CardClassType.Original;
            hcd.CardID = (int)c.CardID;
            hcd.IsUpGrade = c.CardUpGrade;
            ret.Add(hcd);
        });
        return ret;
    }

    [System.Serializable]
    public class CardIDSelector
    {
        [SerializeField] OriginalCardID m_cardid;
        [SerializeField] bool m_isUpgrade;
        public OriginalCardID CardID => m_cardid;
        public CardUpGrade CardUpGrade => m_isUpgrade ? CardUpGrade.AsseptUpGrade : CardUpGrade.NoUpGrade;
    }
}
public class AKCardClass : ICardClassSelector
{
    [SerializeField] List<CardIDSelector> m_cardIDSelector;
    public List<HaveCardData> Execute()
    {
        List<HaveCardData> ret = new List<HaveCardData>();
        m_cardIDSelector.ForEach(c =>
        {
            HaveCardData hcd = new HaveCardData();
            hcd.CardCalssType = CardClassType.AK;
            hcd.CardID = (int)c.CardID;
            hcd.IsUpGrade = c.CardUpGrade;
            ret.Add(hcd);
        });
        return ret;
    }

    [System.Serializable]
    public class CardIDSelector
    {
        [SerializeField] AKCardID m_cardid;
        [SerializeField] bool m_isUpgrade;
        public AKCardID CardID => m_cardid;
        public CardUpGrade CardUpGrade => m_isUpgrade ? CardUpGrade.AsseptUpGrade : CardUpGrade.NoUpGrade;
    }
}
/// <summary>�J�[�h�N���X</summary>
public enum CardClassType
{
    Common,
    Original,
    AK,
}
/// <summary>�A�b�v�O���[�h�ς݂��ۂ�</summary>
public enum CardUpGrade
{
    NoUpGrade,
    AsseptUpGrade,
}
