using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;

public abstract class Charactor : MonoBehaviour
{
    #region field
    [SerializeField] protected Image m_image;
    [SerializeField] Slider m_lifeSlider;
    [SerializeField] Slider m_blockSlider;
    [SerializeField] Text m_text;
    protected string m_name;
    protected int m_maxLife;
    protected ReactiveProperty<int> m_currentLife = new ReactiveProperty<int>();
    protected ReactiveProperty<int> m_currentBlock = new ReactiveProperty<int>();
    protected bool m_isPlayer;
    private Subject<Unit> m_deadSubject = new Subject<Unit>();
    #endregion
    #region property
    public int MaxLife => m_maxLife;
    public int CurrentLife => m_currentLife.Value;
    public int CurrentBlock => m_currentBlock.Value;
    public IObservable<int> CurrentLifeObservable => m_currentLife;
    public IObservable<int> CurrentBlockObservable => m_currentBlock;
    public bool IsPlayer => m_isPlayer;
    public IObservable<Unit> DeadSubject => m_deadSubject;
    #endregion

    protected virtual void Setup()
    {
        m_lifeSlider.maxValue = m_maxLife;
        CurrentLifeObservable.Subscribe(life =>
        {
            m_lifeSlider.value = life;
            SetText();
            if (m_lifeSlider.value <= 0)
                m_deadSubject.OnNext(Unit.Default);
        }).AddTo(this);
        CurrentBlockObservable.Subscribe(block =>
        {
            if (m_currentBlock.Value < 0)
                m_currentBlock.Value = 0;
            m_blockSlider.value = block;
            SetText();
        }).AddTo(this);
        m_currentBlock.Value = 0;
    }

    protected void SetData(CharactorDataBase dataBase)
    {
        m_name = dataBase.Name;
        m_image.sprite = dataBase.Sprite;
        m_maxLife = dataBase.MaxLife;
        m_currentLife.Value = dataBase.MaxLife;
        m_image.transform.localScale *= dataBase.ImageScaling;
    }

    protected virtual void SetText()
    {
        if (m_currentBlock.Value > 0)
            m_text.text = $"{m_currentLife.Value}+{m_currentBlock} / {m_maxLife}";
        else
            m_text.text = $"{m_currentLife.Value} / {m_maxLife}";
    }

    public abstract void TurnBegin(int turn);
    public abstract void TurnEnd(int turn);

    public virtual void Damage(Command cmd)
    {
        if (cmd.Power > 0)
        {
            int dmg = m_currentBlock.Value -= cmd.Power;
            m_currentLife.Value += dmg;
        }
        m_currentBlock.Value += cmd.Block;
    }

    protected virtual void Dead()
    {
        m_deadSubject.OnNext(Unit.Default);
    }
}
