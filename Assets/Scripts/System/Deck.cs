using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �R�D�̐���
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
                Debug.Log("�R�D�؂�");
                //m_discard.ConvartToDeck(); //�R�D������������̂ĎD����J�[�h��߂�
                if (ChildCount == 0)
                {
                    Debug.Log("�f�b�L�����s��"); //�̂ĎD����J�[�h��߂��Ă��R�D���Ȃ��Ȃ�����̂���߂�
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
