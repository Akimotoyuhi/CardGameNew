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
    [SerializeField] RectTransform m_rectTransform;
    [SerializeField] Slider m_lifeSlider;
    [SerializeField] Slider m_blockSlider;
    /// <summary>体力とブロック値表示用テキスト</summary>
    [SerializeField] Text m_lifeText;
    /// <summary>effectViewPrefabの親</summary>
    [SerializeField] Transform m_effectViewParent;
    /// <summary>現在かかっているエフェクトを表示するプレハブ</summary>
    [SerializeField] EffectDisplay m_effectViewPrefab;
    /// <summary>ダメージテキストのプレハブ</summary>
    [SerializeField] DamageText m_datageTextPrefab;
    /// <summary>ダメージ表示テキストの表示位置</summary>
    [SerializeField] Vector2 m_damageTextPosition;
    /// <summary>ドロップ対象を強調表示する用</summary>
    [SerializeField] protected GameObject m_dropEriaObject;
    protected string m_name;
    protected int m_maxLife;
    protected ReactiveProperty<int> m_currentLife = new ReactiveProperty<int>();
    protected ReactiveProperty<int> m_currentBlock = new ReactiveProperty<int>();
    protected List<EffectBase> m_effects = new List<EffectBase>();
    protected List<DamageText> m_damageTexts = new List<DamageText>();
    protected bool m_isPlayer;
    protected bool m_isDead;
    protected Subject<Unit> m_deadSubject = new Subject<Unit>();
    #endregion
    #region property
    public int MaxLife => m_maxLife;
    public int CurrentLife => m_currentLife.Value;
    public int CurrentBlock => m_currentBlock.Value;
    public IObservable<int> CurrentLifeObservable => m_currentLife;
    public IObservable<int> CurrentBlockObservable => m_currentBlock;
    public List<EffectBase> Effects => new List<EffectBase>();
    public bool IsPlayer => m_isPlayer;
    public bool IsDead => m_isDead;
    public IObservable<Unit> DeadSubject => m_deadSubject;
    #endregion

    protected virtual void Setup()
    {
        //体力スライダーの設定
        m_lifeSlider.maxValue = m_maxLife;
        CurrentLifeObservable.Subscribe(life =>
        {
            m_lifeSlider.value = life;
            SetText();
            if (m_lifeSlider.value <= 0)
            {
                //m_currentLife.Value = 0;
                Dead();
            }
        }).AddTo(this);

        //ブロック値スライダーの設定
        CurrentBlockObservable.Subscribe(block =>
        {
            if (m_currentBlock.Value < 0)
                m_currentBlock.Value = 0;
            m_blockSlider.value = block;
            SetText();
        }).AddTo(this);

        m_dropEriaObject.SetActive(false);
        m_currentBlock.Value = 0;
        m_isDead = false;
    }

    /// <summary>
    /// 渡されたキャラクターデータを各値に入れる
    /// </summary>
    /// <param name="dataBase"></param>
    protected void SetData(CharactorDataBase dataBase)
    {
        m_name = dataBase.Name;
        m_image.sprite = dataBase.Sprite;
        m_maxLife = dataBase.MaxLife;
        m_currentLife.Value = dataBase.MaxLife;
        m_image.transform.localScale = Vector2.one;
        m_image.transform.localScale *= dataBase.ImageScaling;
    }

    /// <summary>
    /// HP上部のテキストの更新
    /// </summary>
    protected virtual void SetText()
    {
        if (m_currentBlock.Value > 0)
            m_lifeText.text = $"{m_currentLife.Value}+{m_currentBlock} / {m_maxLife}";
        else
            m_lifeText.text = $"{m_currentLife.Value} / {m_maxLife}";
    }

    /// <summary>
    /// ターン開始
    /// </summary>
    public virtual async UniTask TurnBegin(FieldEffect fieldEffect)
    {
        if (IsDead)
            return;
        //ターン開始時にブロック値はリセット
        if (fieldEffect.CurrentTurn != 1) //1ターン目はリセットしない
            m_currentBlock.Value = 0;

        //ターン開始時のエフェクトを評価しにいく
        ConditionalParametor cp = new ConditionalParametor();
        cp.Setup(fieldEffect.CurrentTurn, EvaluationParamType.Turn, EffectTiming.TurnBegin);
        List<ConditionalParametor> cps = new List<ConditionalParametor>();
        cps.Add(cp);
        EffectExecute(cps);

        await UniTask.Yield();
    }

    /// <summary>
    /// ターン終了
    /// </summary>
    public virtual async UniTask TurnEnd(FieldEffect fieldEffect)
    {
        if (IsDead)
            return;
        //ターン終了時のエフェクトを評価しにいく
        ConditionalParametor cp = new ConditionalParametor();
        cp.Setup(fieldEffect.CurrentTurn, EvaluationParamType.Turn, EffectTiming.TurnEnd);
        List<ConditionalParametor> cps = new List<ConditionalParametor>();
        cps.Add(cp);
        EffectExecute(cps);

        await UniTask.Yield();
    }

    /// <summary>
    /// 被ダメージ処理
    /// </summary>
    /// <param name="cmd"></param>
    public virtual void Damage(Command cmd)
    {
        if (IsDead)
            return;

        if (cmd.Effect != null)
            cmd.Effect.ForEach(e => AddEffect(e));

        if (cmd.Power > 0)
        {
            //ブロック値計算
            int dmg = m_currentBlock.Value - cmd.Power;

            DamageText dmgText = Instantiate(m_datageTextPrefab);
            m_damageTexts.Add(dmgText);
            dmgText.transform.SetParent(transform, false);
            //現在ブロック数が0より大きいならブロック成功値を表示
            if (m_currentBlock.Value > 0)
            {
                dmgText.Setup(
                    cmd.Power,
                    m_damageTextPosition,
                    DamageTextType.Block,
                    DG.Tweening.Ease.OutQuad,
                    () => m_damageTexts.Remove(dmgText));
            }
            //受けたダメージを表示
            if (dmg < 0)
            {
                dmgText.Setup(
                    dmg * -1,
                    m_damageTextPosition,
                    DamageTextType.Damage,
                    DG.Tweening.Ease.OutQuad,
                    () => m_damageTexts.Remove(dmgText));
            }

            //ブロック値を超えたダメージを受けた時
            if (dmg <= 0)
            {
                

                //ダメージを受ける
                m_currentLife.Value += dmg;
                if (m_currentLife.Value <= 0)
                    m_currentLife.Value = 0;
            }

            m_currentBlock.Value -= cmd.Power;
        }
        m_currentBlock.Value += cmd.Block;
    }

    /// <summary>
    /// 自身に新たにエフェクトを付与する
    /// </summary>
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
        EffectRemoveCheck();
    }

    /// <summary>
    /// 現在かかっているエフェクトの表示と更新
    /// </summary>
    protected void SetEffectDisplay()
    {
        //Debug.Log($"{m_effectViewParent.childCount}");
        for (int i = m_effectViewParent.childCount - 1; i >= 0; i--)
        {
            Destroy(m_effectViewParent.GetChild(i).gameObject);
        }
        m_effects.ForEach(e =>
        {
            EffectDisplay ev = EffectDisplay.Init(m_effectViewPrefab, e);
            ev.transform.SetParent(m_effectViewParent.transform, false);
        });
    }

    /// <summary>
    /// エフェクトの実行
    /// </summary>
    public Command EffectExecute(List<ConditionalParametor> conditionalParametors)
    {
        Command ret = new Command();
        //エフェクト数が0の場合は渡された値をそのままCommandに変換して返す
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
        EffectRemoveCheck();
        return ret;
    }

    /// <summary>
    /// 現在かかっているエフェクトを削除できるものだけ削除する
    /// </summary>
    private void EffectRemoveCheck()
    {
        for (int i = m_effects.Count - 1; i >= 0; i--)
        {
            if (m_effects[i].IsRemove)
                m_effects.RemoveAt(i);
        }
        SetEffectDisplay();
    }

    /// <summary>死亡処理</summary>
    protected abstract void Dead();

    private void OnDestroy()
    {
        m_damageTexts.ForEach(d => d.Kill());
        m_damageTexts.Clear();
    }
}
