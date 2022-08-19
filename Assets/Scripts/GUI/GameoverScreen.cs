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
    /// <summary>�^�C�g���փ{�^��</summary>
    [SerializeField] Button m_toTitleButton;
    /// <summary>���g���C�{�^��</summary>
    [SerializeField] Button m_retryButton;
    /// <summary>�e�L�X�g</summary>
    [SerializeField] Text m_text;
    /// <summary>�Q�[�����U���g���</summary>
    [SerializeField] GameObject m_gameResultPanel;
    /// <summary>�{�^���������Ă���ē��͉\�ɂȂ�܂ł̎���</summary>
    [SerializeField] float m_onClickDuration;
    /// <summary>�e�L�X�g�̕\���Ԋu</summary>
    [SerializeField] float m_textViewDuration;
    /// <summary>�Q�[���I�[�o�[��ʂɈڍs����܂łɂ����鎞��</summary>
    [SerializeField] float m_toGameoverScreenTime;
    /// <summary>�Q�[���I�[�o�[���ɕ\�������e�L�X�g</summary>
    [SerializeField, TextArea] string m_gameoverText;
    /// <summary>�Q�[���N���A���ɕ\�������e�L�X�g</summary>
    [SerializeField, TextArea] string m_clearText;
    /// <summary>�Q�[���I�[�o�[��ʂɈڍs����܂łɂ����鎞��</summary>
    public float ToGameoverScreenTime => m_toGameoverScreenTime;

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