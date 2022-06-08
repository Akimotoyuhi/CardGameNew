using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;

public class Charactor : MonoBehaviour
{
    #region field
    [SerializeField] Slider m_lifeSlider;
    [SerializeField] Slider m_blockSlider;
    [SerializeField] Text m_text;
    protected int m_maxLife;
    protected ReactiveProperty<int> m_currentLife = new ReactiveProperty<int>();
    protected ReactiveProperty<int> m_currentBlock = new ReactiveProperty<int>();
    #endregion
    #region property
    public IObservable<int> CurrentLifeObservable => m_currentLife;
    public IObservable<int> CurrentBlockObservable => m_currentBlock;
    #endregion

    public virtual void Setup()
    {
        m_lifeSlider.maxValue = m_maxLife;
        CurrentLifeObservable.Subscribe(life =>
        {
            m_lifeSlider.value = life;
            SetText();
        }).AddTo(this);
        CurrentBlockObservable.Subscribe(block =>
        {
            m_blockSlider.value = block;
            SetText();
        }).AddTo(this);
    }

    protected virtual void SetText()
    {
        if (m_currentBlock.Value > 0)
            m_text.text = $"{m_currentLife.Value}+{m_currentBlock} / {m_maxLife}";
        else
            m_text.text = $"{m_currentLife.Value} / {m_maxLife}";
    }

    protected virtual void Dead()
    {

    }
}
