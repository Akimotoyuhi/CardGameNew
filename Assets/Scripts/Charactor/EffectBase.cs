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
            switch (m_effectID)
            {
                case EffectID.Weakness:
                    return new Weakness();
                case EffectID.Frail:
                    return new Frail();
                default:
                    throw new System.Exception("���݂��Ȃ��G�t�F�N�g���I�����ꂽ");
            }
        }
    }
}
public struct ConditionalParametor
{
    public int Parametor { get; set; }
    public EvaluationParamType EvaluationParamType { get; set; }
    public EffectTiming EffectTiming { get; set; }
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