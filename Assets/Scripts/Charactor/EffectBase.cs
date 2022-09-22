using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バフデバフ基底クラス
/// </summary>
public abstract class EffectBase
{
    public int Turn { get; set; }
    public abstract string Tooltip { get; }
    /// <summary>このエフェクトが除去可能か</summary>
    public abstract bool IsRemove { get; }
    /// <summary>バフかデバフかそれ以外か</summary>
    public abstract BuffType GetBuffType { get; }
    public abstract EffectID GetEffectID { get; }
    /// <summary>
    /// エフェクト効果発動
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
                case EffectID.Strength:
                    ret = new Strength();
                    break;
                case EffectID.Agile:
                    ret = new Agile();
                    break;
                case EffectID.Vitality:
                    ret = new Vitality();
                    break;
                case EffectID.Sturdy:
                    ret = new Sturdy();
                    break;
                default:
                    throw new System.Exception("存在しないエフェクトが選択された");
            }
            ret.Turn = m_turn;
            return ret;
        }
    }
}
public struct ConditionalParametor
{
    /// <summary>効果の数値</summary>
    public int Parametor { get; set; }
    /// <summary>付与するエフェクト</summary>
    public EffectBase Effect { get; set; }
    /// <summary>Parametorの値が何なのか</summary>
    public EvaluationParamType EvaluationParamType { get; set; }
    /// <summary>エフェクトが評価されるタイミング</summary>
    public EffectTiming EffectTiming { get; set; }
    /// <summary>これが評価された場合に効果を使った事にするか</summary>
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
/// エフェクトのID
/// </summary>
public enum EffectID
{
    /// <summary>脱力</summary>
    Weakness,
    /// <summary>虚弱</summary>
    Frail,
    /// <summary>筋力</summary>
    Strength,
    /// <summary>敏捷</summary>
    Agile,
    /// <summary>活力</summary>
    Vitality,
    /// <summary>頑丈</summary>
    Sturdy,
}
/// <summary>
/// エフェクトが評価するパラメータ
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
/// エフェクトの評価タイミング
/// </summary>
public enum EffectTiming
{
    TurnBegin,
    TurnEnd,
    Attacked,
    Damaged,
    Blocked,

}
/// <summary>バフかデバフかそれ以外か</summary>
public enum BuffType
{
    Buff,
    Debuff,
    Soreigai,
}
#endregion