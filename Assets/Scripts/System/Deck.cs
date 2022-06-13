using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 山札の制御
/// </summary>
public class Deck : CardControllerBase
{
    [SerializeField] Discard m_discard;
    [SerializeField] Hand m_hand;
    public List<Card> SetCard
    {
        set
        {
            List<int> vs = new List<int>();
            for (int i = 0; i < value.Count;)
            {
                int r = Random.Range(0, value.Count);
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
            for (int i = 0; i < vs.Count; i++)
            {
                value[vs[i]].transform.SetParent(CardParent, false);
                m_card.Add(value[vs[i]]);
            }
            m_card.ForEach(c => Debug.Log(c.Name));
        }
    }

    public void Draw(int drawNum)
    {
        for (int i = 0; i < drawNum; i++)
        {
            if (ChildCount <= 0)
            {
                Debug.Log("山札切れ");
                m_discard.ConvartToDeck(); //山札が無かったら捨て札からカードを戻す
                if (ChildCount == 0)
                {
                    Debug.Log("デッキ枚数不足"); //捨て札からカードを戻しても山札がないなら引くのをやめる
                    return;
                }
            }
            Card b = CardParent.GetChild(0).GetComponent<Card>();
            b.CardState = CardState.Play;
            //b.GetPlayerEffect
            b.transform.SetParent(m_hand.CardParent, false);
        }
    }
}
