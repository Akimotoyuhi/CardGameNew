using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �o�t�f�o�t���N���X
/// </summary>
public abstract class EffectBase
{
    public int Turn { get; set; }
    public abstract string Tooltip { get; }
    /// <summary>���̃G�t�F�N�g�������\��</summary>
    public abstract bool IsRemove { get; }
    /// <summary>�o�t���f�o�t������ȊO��</summary>
    public abstract BuffType GetBuffType { get; }
    public abstract EffectID GetEffectID { get; }
    /// <summary>
    /// �G�t�F�N�g���ʔ���
    /// </summary>
    /// <param name="effectTiming"></param>
    /// <param name="evaluationParametors"></param>
    /// <returns></returns>
    public abstract Command Effect(List<ConditionalParametor> evaluationParametors);
    public EffectBase Copy => (EffectBase)MemberwiseClone();
}
[System.Serializable]
public class EffectSelector
{
    [SerializeField] EffectID m_effectID;
    [SerializeField] int m_turn;
    public EffectBase GetEffect
    {
        get
        {
            EffectBase ret = null;
            switch (m_effectID)
            {
                case EffectID.Weakness:
                    ret = new Weakness();
                    break;
                case EffectID.Frail:
                    ret = new Frail();
                    break;
                default:
                    throw new System.Exception("���݂��Ȃ��G�t�F�N�g���I�����ꂽ");
            }
            ret.Turn = m_turn;
            return ret;
        }
    }
}
public struct ConditionalParametor
{
    /// <summary>���ʂ̐��l</summary>
    public int Parametor { get; set; }
    /// <summary>�t�^����G�t�F�N�g</summary>
    public EffectBase Effect { get; set; }
    /// <summary>Parametor�̒l�����Ȃ̂�</summary>
    public EvaluationParamType EvaluationParamType { get; set; }
    /// <summary>�G�t�F�N�g���]�������^�C�~���O</summary>
    public EffectTiming EffectTiming { get; set; }
    /// <summary>���ꂪ�]�����ꂽ�ꍇ�Ɍ��ʂ��g�������ɂ��邩</summary>
    public bool IsExecute { get; set; }
    public void Setup(int parametor, EvaluationParamType evaluationParamType, EffectTiming effectTiming, bool isExecute = false)
    {
        Parametor = parametor;
        EvaluationParamType = evaluationParamType;
        EffectTiming = effectTiming;
        IsExecute = isExecute;
    }
    public void Setup(EffectBase effectBase, EvaluationParamType evaluationParamType, EffectTiming effectTiming, bool isExecute = false)
    {
        Effect = effectBase;
        EvaluationParamType = evaluationParamType;
        EffectTiming = effectTiming;
        IsExecute = isExecute;
    }
    public void Setup(EvaluationParamType evaluationParamType, EffectTiming effectTiming, bool isExecute)
    {
        EvaluationParamType = evaluationParamType;
        EffectTiming = effectTiming;
        IsExecute = isExecute;
    }
}
#region Enums
/// <summary>
/// �G�t�F�N�g��ID
/// </summary>
public enum EffectID
{
    /// <summary>�E��</summary>
    Weakness,
    /// <summary>����</summary>
    Frail,
}
/// <summary>
/// �G�t�F�N�g���]������p�����[�^
/// </summary>
public enum EvaluationParamType
{
    Life,
    Attack,
    Block,
    Effect,
    Turn,
}
/// <summary>
/// �G�t�F�N�g�̕]���^�C�~���O
/// </summary>
public enum EffectTiming
{
    TurnBegin,
    TurnEnd,
    Attacked,
    Damaged,
    Blocked,

}
/// <summary>�o�t���f�o�t������ȊO��</summary>
public enum BuffType
{
    Buff,
    Debuff,
    Soreigai,
}
#endregion