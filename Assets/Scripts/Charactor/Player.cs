using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

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

    public override async UniTask TurnBegin(int turn)
    {
        await UniTask.Yield();
    }

    public override async UniTask TurnEnd(int turn)
    {
        await UniTask.Yield();
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

    public bool GetDrop(ref List<Command> commands)
    {
        return true;
    }

    public UseType GetUseType() => UseType.Player;
}
