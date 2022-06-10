using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class Enemy : Charactor, IDrop
{
    private int m_index;
    private Subject<List<Command>> m_action = new Subject<List<Command>>();
    public int Index { set => m_index = value; }
    /// <summary>敵行動時に発行されるイベント</summary>
    public System.IObservable<List<Command>> ActionSubject => m_action;

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

    public override void TurnBegin(int turn)
    {
    }

    public override void TurnEnd(int turn)
    {
    }

    public async UniTask Action()
    {
        Debug.Log("何かしらの行動");
        await UniTask.Delay(System.TimeSpan.FromSeconds(1));
    }

    protected override void Dead()
    {
        base.Dead();
    }

    public bool GetDrop(ref List<Command> commands)
    {
        commands.ForEach(c =>
        {
            if (c.UseType == UseType.Enemy)
                c.TargetEnemyIndex = m_index;
        });
        return true;
    }

    public UseType GetUseType() => UseType.Enemy;
}
