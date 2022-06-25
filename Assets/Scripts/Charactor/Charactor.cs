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
    [SerializeField] Transform m_effectViewParent;
    [SerializeField] EffectView m_effectViewPrefab;
    protected string m_name;
    protected int m_maxLife;
    protected ReactiveProperty<int> m_currentLife = new ReactiveProperty<int>();
    protected ReactiveProperty<int> m_currentBlock = new ReactiveProperty<int>();
    protected List<EffectBase> m_effects = new List<EffectBase>();
    protected bool m_isPlayer;
    private Subject<Unit> m_deadSubject = new Subject<Unit>();
    #endregion
    #region property
    public int MaxLife => m_maxLife;
    public int CurrentLife => m_currentLife.Value;
    public int CurrentBlock => m_currentBlock.Value;
    public IObservable<int> CurrentLifeObservable => m_currentLife;
    public IObservable<int> CurrentBlockObservable => m_currentBlock;
    public List<EffectBase> Effects => new List<EffectBase>();
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

    /// <summary>
    /// ターン開始
    /// </summary>
    /// <param name="turn"></param>
    /// <returns></returns>
    public virtual async UniTask TurnBegin(int turn)
    {
        ConditionalParametor cp = new ConditionalParametor();
        cp.EffectTiming = EffectTiming.TurnBegin;
        cp.EvaluationParamType = EvaluationParamType.Turn;
        cp.Parametor = turn;
        List<ConditionalParametor> cps = new List<ConditionalParametor>();
        cps.Add(cp);
        EffectExecute(cps);
        await UniTask.Yield();
    }

    /// <summary>
    /// ターン終了
    /// </summary>
    /// <param name="turn"></param>
    /// <returns></returns>
    public virtual async UniTask TurnEnd(int turn)
    {
        ConditionalParametor cp = new ConditionalParametor();
        cp.EffectTiming = EffectTiming.TurnEnd;
        cp.EvaluationParamType = EvaluationParamType.Turn;
        cp.Parametor = turn;
        List<ConditionalParametor> cps = new List<ConditionalParametor>();
        cps.Add(cp);
        EffectExecute(cps);
        await UniTask.Yield();
    }

    /// <summary>被ダメージ処理</summary>
    /// <param name="cmd"></param>
    public virtual void Damage(Command cmd)
    {
        if (cmd.Effect != null)
            cmd.Effect.ForEach(e => AddEffect(e));
        if (cmd.Power > 0)
        {
            int dmg = m_currentBlock.Value -= cmd.Power;
            m_currentLife.Value += dmg;
        }
        m_currentBlock.Value += cmd.Block;
    }

    public void AddEffect(EffectBase effect)
    {
        bool addFlag = true;
        for (int i = 0; i < m_effects.Count; i++)
        {
            if (m_effects[i].GetEffectID == effect.GetEffectID)
            {
                m_effects[i].Turn += effect.Copy.Turn;
                addFlag = false;
            }
        }
        if (addFlag)
        {
            m_effects.Add(effect.Copy);
        }
        SetViewEffectUI();
    }

    protected void SetViewEffectUI()
    {
        for (int i = 0; i < m_effectViewParent.childCount; i++)
        {
            Destroy(m_effectViewParent.GetChild(i).gameObject);
        }
        m_effects.ForEach(e =>
        {
            EffectView ev = Instantiate(m_effectViewPrefab);
            ev.Setup(e);
            ev.transform.SetParent(m_effectViewParent.transform, false);
        });
    }

    public Command EffectExecute(List<ConditionalParametor> conditionalParametors)
    {
        Command ret = new Command();
        if (m_effects.Count == 0)
        {
            conditionalParametors.ForEach(cp =>
            {
                ret.SetCommand(cp);
            });
        }
        else
        {
            m_effects.ForEach(e =>
            ret = e.Effect(conditionalParametors));
        }
        return ret;
    }

    /// <summary>死亡処理</summary>
    protected virtual void Dead()
    {
        m_deadSubject.OnNext(Unit.Default);
    }
}
