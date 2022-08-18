using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using DG.Tweening;

[System.Serializable]
public class GameoverScreen
{
    /// <summary>タイトルへボタン</summary>
    [SerializeField] Button m_toTitleButton;
    /// <summary>リトライボタン</summary>
    [SerializeField] Button m_retryButton;
    /// <summary>テキスト</summary>
    [SerializeField] Text m_text;
    /// <summary>ゲームリザルト画面</summary>
    [SerializeField] GameObject m_gameResultPanel;
    /// <summary>ボタンを押してから再入力可能になるまでの時間</summary>
    [SerializeField] float m_onClickDuration;
    /// <summary>テキストの表示間隔</summary>
    [SerializeField] float m_textViewDuration;
    /// <summary>ゲームオーバー時に表示されるテキスト</summary>
    [SerializeField] string m_gameoverText;
    /// <summary>ゲームクリア時に表示されるテキスト</summary>
    [SerializeField] string m_clearText;

    public void Setup(Component component)
    {
        m_toTitleButton.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(m_onClickDuration))
            .Subscribe(_ => GameManager.Instance.ToTitle())
            .AddTo(component);

        m_retryButton.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(m_onClickDuration))
            .Subscribe(_ => GameManager.Instance.Restart())
            .AddTo(component);
    }

    public void SetActive(bool value, GameEndType? gameEndType = null)
    {
        SetButtonInteractable(false);
        m_gameResultPanel.SetActive(value);
        if (gameEndType != null)
        {
            m_text.text = "";
            switch (gameEndType)
            {
                case GameEndType.Gameover:
                    m_text.DOText(m_gameoverText, m_textViewDuration)
                        .OnComplete(() => SetButtonInteractable(true));
                    break;
                case GameEndType.Clear:
                    m_text.DOText(m_clearText, m_textViewDuration)
                        .OnComplete(() => SetButtonInteractable(true));
                    break;
                default:
                    break;
            }
        }
    }

    private void SetButtonInteractable(bool value)
    {
        m_retryButton.interactable = value;
        m_toTitleButton.interactable = value;
    }
}