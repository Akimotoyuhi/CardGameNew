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
            return null;
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
/// エフェクトのID
/// </summary>
public enum EffectID
{
    /// <summary>脱力</summary>
    Weekness,
    /// <summary>虚弱</summary>
    Frail,
}
/// <summary>
/// エフェクトが評価するパラメータ
/// </summary>
public enum EvaluationParamType
{
    Life,
    Power,
    Defence,
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