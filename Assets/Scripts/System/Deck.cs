using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 山札の制御
/// </summary>
public class Deck : CardControllerBase
{
    public override void Setup()
    {
        base.Setup();
    }

    public void Draw(int drawNum)
    {
        for (int i = 0; i < drawNum; i++)
        {
            if (ChildCount <= 0)
            {
                Debug.Log("山札切れ");
                //m_discard.ConvartToDeck(); //山札が無かったら捨て札からカードを戻す
                if (ChildCount == 0)
                {
                    Debug.Log("デッキ枚数不足"); //捨て札からカードを戻しても山札がないなら引くのをやめる
                    return;
                }
            }
            int r = Random.Range(0, ChildCount);
            //Card b = m_cardParent.GetChild(r).GetComponent<Card>();
            //b.CardState = CardState.Play;
            //b.GetPlayerEffect();
            //m_cardParent.GetChild(r).SetParent(m_hand.CardParent, false);
        }
    }
}
