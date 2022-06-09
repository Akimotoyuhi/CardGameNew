using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Player : Charactor, IDrop
{
    private int m_cost;

    protected override void Setup()
    {
        base.Setup();
    }

    public void SetBaseData(PlayerDataBase dataBase)
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

    public bool GetDrop(ref List<Command> commands)
    {
        Debug.Log("GetDrop");
        commands.ForEach(c => Debug.Log($"{c.UseType}, {c.Power}, {c.Block}"));
        return true;
    }

    public UseType GetUseType() => UseType.Player;
}
