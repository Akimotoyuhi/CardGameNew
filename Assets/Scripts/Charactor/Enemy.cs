using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Charactor
{


    protected override void Setup()
    {
        base.Setup();
    }

    public void SetBaseData(EnemyDataBase dataBase)
    {
        m_image.sprite = dataBase.CharactorData.Sprite;
        m_maxLife = dataBase.CharactorData.MaxLife;
        m_currentLife.Value = dataBase.CharactorData.MaxLife;
        Setup();
    }

    protected override void Dead()
    {
        base.Dead();
    }
}
