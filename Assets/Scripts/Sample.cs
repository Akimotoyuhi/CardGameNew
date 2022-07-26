using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UniRx;
using System;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    [SerializeField] RectTransform m_rect;
    private Sequence m_sequence;

    private void Start()
    {
        m_sequence = DOTween.Sequence();
        m_sequence.Append(m_rect.DOAnchorPosX(500, 1))
            .Append(m_rect.DOAnchorPosX(-500, 1))
            .Append(m_rect.DOAnchorPosX(0, 0.5f))
            .SetLoops(-1);
        UniTask.Void(async () =>
        {
            Debug.Log("待機開始");
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            m_rect.gameObject.SetActive(false);
            Debug.Log("非表示");
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            m_rect.gameObject.SetActive(true);
            Debug.Log("再表示");
        });
    }
}
