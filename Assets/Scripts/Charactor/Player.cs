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
        SetData(dataBase.CharactorData);
        Setup();
    }

    public override void Damage(Command cmd)
    {
        if (cmd.UseType != UseType.Player)
            return;
        base.Damage(cmd);
    }

    protected override void Dead()
    {
        base.Dead();
    }

    //以下インターフェース

    public bool GetDrop(ref List<Command> commands, ref Enemy enemy)
    {
        Debug.Log("GetDrop");
        commands.ForEach(c => Debug.Log($"{c.UseType}, {c.Power}, {c.Block}"));
        return true;
    }

    public UseType GetUseType() => UseType.Player;
}
