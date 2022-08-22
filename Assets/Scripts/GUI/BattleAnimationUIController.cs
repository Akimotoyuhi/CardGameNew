using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// 戦闘中にアニメーションさせるUI達の制御
/// </summary>
[System.Serializable]
public class BattleAnimationUIController
{
    [SerializeField] Text m_battleStartText;
    [SerializeField] Color m_battleStartTextColor;
    [SerializeField] string m_battleStartTextValue;
    [SerializeField] float m_battleStartTextFadeinDuration;
    [SerializeField] float m_battleStartTextKeepDuration;
    [SerializeField] float m_battleStartTextFadeoutghtDuration;
    [SerializeField] Text m_turnText;
    [SerializeField] Color m_playerFazeTextColor;
    [SerializeField] Vector2 m_playerFazeTurnTextMoveBeginPosition;
    [SerializeField] string m_plaerFazeTextValue;
    [SerializeField] float m_playerFazeTextInMoveDuration;
    [SerializeField] float m_playerFazeTextKeepDuration;
    [SerializeField] float m_playerFazeTextOutMoveDuration;
    [SerializeField] Color m_enemyFazeTextColor;
    [SerializeField] Vector2 m_enemyFazeTurnTextMoveBeginPosition;
    [SerializeField] string m_enemyFazeTextValue;
    [SerializeField] float m_enemyFazeTextInMoveDuration;
    [SerializeField] float m_enemyFazeTextKeepDuration;
    [SerializeField] float m_enemyFazeTextOutMoveDuration;

    public void Setup()
    {
        m_battleStartText.color = Color.clear;
        m_battleStartText.text = m_battleStartTextValue;
        m_turnText.color = Color.clear;
    }

    /// <summary>
    /// アニメーションさせる
    /// </summary>
    /// <param name="animationTextType"></param>
    /// <returns></returns>
    public async UniTask ActiveText(BattleAnimationUIMoveTextType animationTextType)
    {
        float f;
        switch (animationTextType)
        {
            case BattleAnimationUIMoveTextType.BattleStart:
                f = m_battleStartTextKeepDuration * 1000; //秒をミリ秒に変換
                await m_battleStartText.DOColor(m_battleStartTextColor, m_battleStartTextFadeinDuration);
                await UniTask.Delay((int)f);
                await m_battleStartText.DOColor(Color.clear, m_battleStartTextFadeoutghtDuration);
                break;
            case BattleAnimationUIMoveTextType.PlayerFaze:
                m_turnText.rectTransform.anchoredPosition = m_playerFazeTurnTextMoveBeginPosition;
                m_turnText.color = m_playerFazeTextColor;
                m_turnText.text = m_plaerFazeTextValue;
                f = m_playerFazeTextKeepDuration * 1000;
                await m_turnText.rectTransform.DOAnchorPos(Vector2.zero, m_playerFazeTextInMoveDuration);
                await UniTask.Delay((int)f);
                await m_turnText.rectTransform.DOAnchorPos(m_playerFazeTurnTextMoveBeginPosition * -1, m_playerFazeTextOutMoveDuration);
                break;
            case BattleAnimationUIMoveTextType.EnemyFaze:
                m_turnText.rectTransform.anchoredPosition = m_enemyFazeTurnTextMoveBeginPosition;
                m_turnText.color = m_enemyFazeTextColor;
                m_turnText.text = m_enemyFazeTextValue;
                f = m_enemyFazeTextKeepDuration * 1000;
                await m_turnText.rectTransform.DOAnchorPos(Vector2.zero, m_enemyFazeTextInMoveDuration);
                await UniTask.Delay((int)f);
                await m_turnText.rectTransform.DOAnchorPos(m_enemyFazeTurnTextMoveBeginPosition * -1, m_enemyFazeTextOutMoveDuration);
                break;
            default:
                break;
        }
    }
}

public enum BattleAnimationUIMoveTextType
{
    BattleStart,
    PlayerFaze,
    EnemyFaze,
}
