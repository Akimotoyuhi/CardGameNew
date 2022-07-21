using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class Player : Charactor, IDrop
{
    private int m_maxCost = 3;
    private ReactiveProperty<int> m_currentCost = new ReactiveProperty<int>();
    private int m_maxDrowNum = 5;
    public int MaxCost => m_maxCost;
    public int CurrentCost { get => m_currentCost.Value; set => m_currentCost.Value = value; }
    public System.IObservable<int> CurrentCostObservable => m_currentCost;
    public int DrowNum => m_maxDrowNum;

    protected override void Setup()
    {
        GameManager.Instance.FloorUpdate.Subscribe(_ =>
        {
            m_effects.Clear();
            SetViewEffectUI();
        }).AddTo(this);
        base.Setup();
    }

    public void SetBaseData(PlayerDataBase dataBase)
    {
        m_maxCost = dataBase.MaxCost;
        SetData(dataBase.CharactorData);
        Setup();
    }

    public void HealEvent(int healValue)
    {
        m_currentLife.Value = healValue;
    }

    public override async UniTask TurnBegin(int turn)
    {
        m_currentCost.Value = m_maxCost;
        await base.TurnBegin(turn);
    }

    public override async UniTask TurnEnd(int turn)
    {
        await base.TurnEnd(turn);
    }

    public override void Damage(Command cmd)
    {
        if (cmd.UseType != UseType.Player)
            return;
        base.Damage(cmd);
    }

    protected override void Dead()
    {
        m_isDead = true;
        m_deadSubject.OnNext(Unit.Default);
    }

    //�ȉ��C���^�[�t�F�[�X

    public bool GetDrop(ref List<Command> commands)
    {
        return true;
    }

    public UseType GetUseType() => UseType.Player;
}
