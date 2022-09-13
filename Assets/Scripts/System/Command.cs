using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 何かしらになんかするときの情報受け渡し用構造体
/// </summary>
public struct Command
{
    public int Life { get; set; }
    /// <summary>攻撃力値</summary>
    public int Power { get; set; }
    /// <summary>ブロック値</summary>
    public int Block { get; set; }
    /// <summary>エフェクト</summary>
    public List<EffectBase> Effect { get; set; }
    /// <summary>このコマンド実行時に要する時間</summary>
    public float Duration { get; set; }
    /// <summary>効果対象</summary>
    public UseType UseType { get; set; }
    /// <summary>コマンドの種類</summary>
    public CommandType CommandType { get; set; }
    /// <summary>敵行動予定用</summary>
    public PlanType PlanType { get; set; }
    /// <summary>敵を対象とする場合の敵インデックス</summary>
    public int TargetEnemyIndex { get; set; }
    /// <summary>ストックしておくターン数</summary>
    public int StockTurn { get; set; }
    /// <summary>ストック終了時に実行される</summary>
    public bool IsStockRelease { get; set; }
    /// <summary>コマンド実行時に表示するパーティクル</summary>
    public ParticleID ParticleID { get; set; }
    /// <summary>コマンド実行時に鳴らすSE</summary>
    public SEID SEID { get; set; }
    public void SetCommand(ConditionalParametor conditionalParametor)
    {
        switch (conditionalParametor.EvaluationParamType)
        {
            case EvaluationParamType.Life:
                Life += conditionalParametor.Parametor;
                break;
            case EvaluationParamType.Attack:
                Power += conditionalParametor.Parametor;
                break;
            case EvaluationParamType.Block:
                Block += conditionalParametor.Parametor;
                break;
            case EvaluationParamType.Effect:
                Effect.Add(conditionalParametor.Effect);
                break;
            default:
                break;
        }
    }
}
/// <summary>戦闘中のフィールド効果</summary>
public struct FieldEffect
{
    public int CurrentTurn { get; set; }
}
#region コマンド設定クラス関連
/// <summary>
/// 付与するコマンドを設定するクラス
/// </summary>
[System.Serializable]
public class CommandSelect
{
    [SerializeReference, SubclassSelector] List<ICommand> m_commands;
    public List<Command> Execute()
    {
        var ret = new List<Command>();
        m_commands.ForEach(c => ret.Add(c.Execute()));
        return ret;
    }
}

/// <summary>
/// 設定するコマンドを切り替える為のインターフェース
/// </summary>
public interface ICommand
{
    Command Execute();
}

/// <summary>
/// 共通するコマンド設定項目を書くとこ
/// </summary>
public class CommandBase
{
    [SerializeField] ParticleID m_particleID = ParticleID.None;
    [SerializeField] SEID m_seID = SEID.None;
    [SerializeField] float m_duration;
    [SerializeField] int m_stockTurn = -1;
    [SerializeField] bool m_isStockRelease;
    [SerializeField] UseType m_useType = UseType.None;

    protected void Method(ref Command command)
    {
        command.Duration = m_duration;
        command.StockTurn = m_stockTurn;
        command.IsStockRelease = m_isStockRelease;
        command.ParticleID = m_particleID;
        command.SEID = m_seID;
        command.UseType = m_useType;
    }
}

public class AttackCommand : CommandBase, ICommand
{
    [SerializeField] int m_power;
    [SerializeField] bool m_isTrueDamage;
    [SerializeField] bool m_isHideType;

    public Command Execute()
    {
        Command ret = new Command();
        ret.Power = m_power;
        ret.CommandType = CommandType.Attack;
        Method(ref ret);
        if (m_isHideType)
            ret.PlanType = PlanType.Other;
        else
            ret.PlanType = PlanType.Attack;
        return ret;
    }
}

public class BlockCommand : CommandBase, ICommand
{
    [SerializeField] int m_block;
    [SerializeField] bool isTrueBlock;
    [SerializeField] bool m_isHideType;

    public Command Execute()
    {
        Command ret = new Command();
        ret.Block = m_block;
        ret.PlanType = PlanType.Block;
        ret.CommandType = CommandType.Block;
        Method(ref ret);
        if (m_isHideType)
            ret.PlanType = PlanType.Other;
        else
            ret.PlanType = PlanType.Block;
        return ret;
    }
}
public class EffectCommand : CommandBase, ICommand
{
    [SerializeField] EffectSelector m_effectSelector;
    [SerializeField] bool m_isHideType;
    [SerializeField] PlanType m_otherBuffTypeCommandType = PlanType.Other;
    public Command Execute()
    {
        Command ret = new Command();
        List<EffectBase> effects = new List<EffectBase>();
        effects.Add(m_effectSelector.GetEffect);
        ret.Effect = effects;
        ret.CommandType = CommandType.Effect;
        Method(ref ret);
        if (!m_isHideType)
        {
            switch (m_effectSelector.GetEffect.GetBuffType)
            {
                case BuffType.Buff:
                    ret.PlanType = PlanType.Auxiliary;
                    break;
                case BuffType.Debuff:
                    ret.PlanType = PlanType.Obstruction;
                    break;
                case BuffType.Soreigai:
                    ret.PlanType = m_otherBuffTypeCommandType;
                    break;
                default:
                    Debug.LogWarning("想定外のバフタイプが検出されました");
                    ret.PlanType = PlanType.Other;
                    break;
            }
        }
        else
            ret.PlanType = PlanType.Other;
        return ret;
    }
}
#endregion
#region コマンド実行条件系
[System.Serializable]
public class Conditional
{
    [SerializeField, Range(0, 100), Tooltip("抽選確率")] int m_probability = 100;
    [SerializeField, Tooltip("最大行動回数\n-1なら制限なし\n現在機能してないです")] int m_playNum = -1;
    [SerializeReference, SubclassSelector] List<IConditional> m_conditionals;
    /// <summary>
    /// 条件の評価
    /// </summary>
    public bool Evaluation(FieldEffect field, Player player, Enemy enemy)
    {
        //抽選確率の評価
        int r = Random.Range(1, 101);
        if (m_probability < r)
            return false;

        //IConditionalの評価
        foreach (var c in m_conditionals)
        {
            if (!c.EnemyEvaluation(enemy))
                return false;
            if (!c.PlayerEvaluation(player))
                return false;
            if (!c.FieldEvaluation(field))
                return false;
        }
        return true;
    }
}
public interface IConditional
{
    /// <summary>フィールド効果を評価</summary>
    bool FieldEvaluation(FieldEffect field);
    /// <summary>プレイヤーを評価</summary>
    bool PlayerEvaluation(Player player);
    /// <summary>敵を評価</summary>
    bool EnemyEvaluation(Enemy enemy);
}
/// <summary>敵の体力を評価する</summary>
public class EnemyLifeConditional : IConditional
{
    [SerializeField] int m_life;
    [SerializeField] EvaluationType m_evaluationType;

    public bool EnemyEvaluation(Enemy enemy) =>
        ConditionalHelper.Evaluation(m_evaluationType, enemy.CurrentLife, m_life);
    public bool FieldEvaluation(FieldEffect field) => true;
    public bool PlayerEvaluation(Player player) => true;
}
/// <summary>現在のターン数を評価する</summary>
public class TurnConditional : IConditional
{
    [SerializeField] int m_turn;
    [SerializeField] bool m_isFirstTurnOnry;
    [SerializeField] EvaluationType m_evaluationType;

    public bool EnemyEvaluation(Enemy enemy) => true;
    public bool FieldEvaluation(FieldEffect field)
    {
        if (m_isFirstTurnOnry)
        {
            if (field.CurrentTurn == 0)
                return true;
            else
                return false;
        }

        return ConditionalHelper.Evaluation(m_evaluationType, field.CurrentTurn, m_turn);
    }
    public bool PlayerEvaluation(Player player) => true;
}
/// <summary>値と評価条件から成否をチェックする</summary>
public static class ConditionalHelper
{
    /// <summary>条件評価</summary>
    /// <param name="evaluationNum">評価する値</param>
    /// <param name="baseNum">評価される値</param>
    /// <param name="evaluationType">評価条件</param>
    /// <returns>条件可否</returns>
    public static bool Evaluation(EvaluationType evaluationType, int evaluationNum, int baseNum)
    {
        switch (evaluationType)
        {
            case EvaluationType.Any:
                return true;
            case EvaluationType.Equal:
                if (baseNum == evaluationNum)
                    return true;
                break;
            case EvaluationType.High:
                if (baseNum <= evaluationNum)
                    return true;
                break;
            case EvaluationType.Low:
                if (baseNum >= evaluationNum)
                    return true;
                break;
            case EvaluationType.AtIntervalsOf:
                if (evaluationNum % baseNum == 0)
                    return true;
                break;
            default:
                break;
        }
        return false;
    }
}
#region enums
/// <summary>評価条件</summary>
public enum EvaluationType
{
    /// <summary>評価条件を適応しない</summary>
    Any,
    /// <summary>等しい</summary>
    Equal,
    /// <summary>以上</summary>
    High,
    /// <summary>以下</summary>
    Low,
    /// <summary>X毎に</summary>
    AtIntervalsOf
}
/// <summary>
/// 敵行動予定用
/// </summary>
public enum PlanType
{
    Attack,
    Block,
    Obstruction,
    Auxiliary,
    Other,
}
/// <summary>
/// コマンドの種類
/// </summary>
public enum CommandType
{
    Attack,
    Block,
    Effect,
}
#endregion
#endregion