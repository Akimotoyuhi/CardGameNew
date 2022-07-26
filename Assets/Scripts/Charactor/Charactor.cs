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
    [SerializeField] Text m_text;
    [SerializeField] Transform m_effectViewParent;
    [SerializeField] EffectView m_effectViewPrefab;
    [SerializeField] DamageText m_datageTextPrefab;
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
        //�̗̓X���C�_�[�̐ݒ�
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

        //�u���b�N�l�X���C�_�[�̐ݒ�
        CurrentBlockObservable.Subscribe(block =>
        {
            if (m_currentBlock.Value < 0)
                m_currentBlock.Value = 0;
            m_blockSlider.value = block;
            SetText();
        }).AddTo(this);

        m_currentBlock.Value = 0;
        m_isDead = false;
    }

    /// <summary>
    /// �n���ꂽ�L�����N�^�[�f�[�^���e�l�ɓ����
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
    /// HP�㕔�̃e�L�X�g�̍X�V
    /// </summary>
    protected virtual void SetText()
    {
        if (m_currentBlock.Value > 0)
            m_text.text = $"{m_currentLife.Value}+{m_currentBlock} / {m_maxLife}";
        else
            m_text.text = $"{m_currentLife.Value} / {m_maxLife}";
    }

    /// <summary>
    /// �^�[���J�n
    /// </summary>
    public virtual async UniTask TurnBegin(int turn)
    {
        //�^�[���J�n���Ƀu���b�N�l�̓��Z�b�g
        m_currentBlock.Value = 0;

        //�^�[���J�n���̃G�t�F�N�g��]�����ɂ���
        ConditionalParametor cp = new ConditionalParametor();
        cp.Setup(turn, EvaluationParamType.Turn, EffectTiming.TurnBegin);
        List<ConditionalParametor> cps = new List<ConditionalParametor>();
        cps.Add(cp);
        EffectExecute(cps);

        await UniTask.Yield();
    }

    /// <summary>
    /// �^�[���I��
    /// </summary>
    public virtual async UniTask TurnEnd(int turn)
    {
        //�^�[���I�����̃G�t�F�N�g��]�����ɂ���
        ConditionalParametor cp = new ConditionalParametor();
        cp.Setup(turn, EvaluationParamType.Turn, EffectTiming.TurnEnd);
        List<ConditionalParametor> cps = new List<ConditionalParametor>();
        cps.Add(cp);
        EffectExecute(cps);

        await UniTask.Yield();
    }

    /// <summary>��_���[�W����</summary>
    public virtual void Damage(Command cmd)
    {
        if (cmd.Effect != null)
            cmd.Effect.ForEach(e => AddEffect(e));
        if (cmd.Power > 0)
        {
            int dmg = m_currentBlock.Value -= cmd.Power;
            DamageText dmgText = Instantiate(m_datageTextPrefab);
            m_damageTexts.Add(dmgText);
            dmgText.transform.SetParent(transform, false);
            dmgText.Setup(dmg, new Vector2(50, 50), 2, DamageTextType.Damage, DG.Tweening.Ease.OutQuad, () =>
            {
                m_damageTexts.Remove(dmgText);
            });
            m_currentLife.Value += dmg;
            if (m_currentLife.Value <= 0)
                m_currentLife.Value = 0;
        }
        m_currentBlock.Value += cmd.Block;
    }

    /// <summary>
    /// �V���ɃG�t�F�N�g��t�^����
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
        SetViewEffectUI();
    }

    /// <summary>
    /// ���݂������Ă���G�t�F�N�g��\������
    /// </summary>
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

    /// <summary>
    /// �G�t�F�N�g�̎��s
    /// </summary>
    public Command EffectExecute(List<ConditionalParametor> conditionalParametors)
    {
        Command ret = new Command();
        //�G�t�F�N�g����0�̏ꍇ�͓n���ꂽ�l�����̂܂�Command�ɕϊ����ĕԂ�
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
        SetViewEffectUI();
        return ret;
    }

    /// <summary>���S����</summary>
    protected abstract void Dead();

    private void OnDestroy()
    {
        m_damageTexts.ForEach(d => d.Kill());
        m_damageTexts.Clear();
    }
}
