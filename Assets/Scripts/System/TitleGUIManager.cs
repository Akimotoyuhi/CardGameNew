using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// �^�C�g����ʂ�UI�̑��������
/// </summary>
public class TitleGUIManager : MonoBehaviour
{
    [SerializeField] TitleManager m_titleManager;
    [SerializeField] Button m_gameStartButton;
    [SerializeField] Button m_charactorSelectButton;
    [SerializeField] Image m_fadeImage;
    private static Image FadeImage { get; set; }

    private void Start()
    {
        FadeImage = m_fadeImage;

        //�L�����N�^�[�I���{�^���̐ݒ�
        

        //�Q�[���J�n�{�^���̐ݒ�
        m_gameStartButton.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ => m_titleManager.GameStart());
    }

    public static async UniTask Fade(Color color, float duration, System.Action onComplete = null)
    {
        if (color != Color.clear)
            FadeImage.raycastTarget = true;
        else
            FadeImage.raycastTarget = false;
        await FadeImage.DOColor(color, duration).OnComplete(() =>
        {
            if (onComplete != null)
                onComplete();
        });
    }
}
