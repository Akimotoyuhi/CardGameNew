using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Sample : MonoBehaviour
{
    [SerializeField] EffectSelector m_effectSelector;
    [SerializeField] EffectDisplay m_effectDisplay;

    private void Start()
    {
        EffectDisplay e = EffectDisplay.Init(m_effectDisplay, m_effectSelector.GetEffect);
        e.transform.SetParent(transform, false);
    }
}