using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class Enemy : Charactor, IDrop
{
    private int m_index;
    private EnemyDataBase m_dataBase;
    private EnemyID m_enemyID;
    private Subject<List<Command>> m_action = new Subject<List<Command>>();
    /// <summary>敵グループでの所属index</summary>
    public int Index { set => m_index = value; }
    /// <summary>敵死亡時に消滅にかける時間</summary>
    public float DeadDuration { private get; set; }
    /// <summary>この敵のID</summary>
    public EnemyID EnemyID => m_enemyID;
    /// <summary>敵行動時に発行されるイベント</summary>
    public System.IObservable<List<Command>> ActionSubject => m_action;

    protected override void Setup()
    {
        base.Setup();
    }

    public void SetBaseData(EnemyDataBase dataBase)
    {
        if (dataBase == null)
            return;
        m_dataBase = dataBase;
        m_image.sprite = dataBase.CharactorData.Sprite;
        m_maxLife = dataBase.CharactorData.MaxLife;
        m_currentLife.Value = dataBase.CharactorData.MaxLife;
        Setup();
    }

    public override async UniTask TurnBegin(int turn)
    {
        await base.TurnBegin(turn);
    }

    public override async UniTask TurnEnd(int turn)
    {
        await Action();
        await base.TurnEnd(turn);
    }

    /// <summary>
    /// 敵を行動させる
    /// </summary>
    /// <returns></returns>
    private async UniTask Action()
    {
        m_action.OnNext(m_dataBase.Action(new Field(), null, this));
        await UniTask.Delay(1);
    }

    protected override void Dead()
    {
        if (IsDead)
            return;
        m_isDead = true;
        m_image.DOColor(Color.clear, DeadDuration)
            .OnComplete(() => m_deadSubject.OnNext(Unit.Default));
    }

    //以下インターフェース

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
