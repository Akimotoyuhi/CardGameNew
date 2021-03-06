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
    /// <summary>敵を対象とする場合の敵インデックス</summary>
    public int TargetEnemyIndex { get; set; }
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
public struct Field
{
    public int CurrentTurn { get; set; }
}
#region コマンド設定クラス関連
[System.Serializable]
public class CommandSelect
{
    [SerializeReference, SubclassSelector] List<ICommand> m_commands;
    public List<ICommand> GetCommands => m_commands;
    public List<Command> Execute()
    {
        var ret = new List<Command>();
        m_commands.ForEach(c => ret.Add(c.Execute()));
        return ret;
    }
}

public interface ICommand
{
    Command Execute();
}

public class AttackCommand : ICommand
{
    [SerializeField] UseType m_useType = UseType.None;
    [SerializeField] int m_power;
    [SerializeField] bool m_isTrueDamage;
    [SerializeField] float m_duration;

    public Command Execute()
    {
        Command ret = new Command();
        ret.Power = m_power;
        ret.UseType = m_useType;
        ret.Duration = m_duration;
        return ret;
    }
}

public class BlockCommand : ICommand
{
    [SerializeField] UseType m_useType;
    [SerializeField] int m_block;
    [SerializeField] bool isTrueBlock;
    [SerializeField] float m_duration;

    public Command Execute()
    {
        Command ret = new Command();
        ret.Block = m_block;
        ret.UseType = m_useType;
        ret.Duration = m_duration;
        return ret;
    }
}
public class EffectCommand : ICommand
{
    [SerializeField] UseType m_useType;
    [SerializeField] float m_duration;
    [SerializeField] List<EffectSelector> m_effectSelector;
    public Command Execute()
    {
        Command ret = new Command();
        List<EffectBase> effects = new List<EffectBase>();
        m_effectSelector.ForEach(e => effects.Add(e.GetEffect));
        ret.Effect = effects;
        ret.Duration = m_duration;
        return ret;
    }
}
#endregion
#region コマンド実行条件系
[System.Serializable]
public class Conditional
{
    [SerializeField, Range(0, 100), Tooltip("抽選確率")] int m_probability = 100;
    [SerializeField, Tooltip("最大行動回数\n-1なら制限なし")] int m_playNum = -1;
    [SerializeReference, SubclassSelector] List<IConditional> m_conditionals;
    public bool Evaluation(Field field, Player player, Enemy enemy)
    {
        bool ret = true;
        foreach (var c in m_conditionals)
        {
            if (!c.EnemyEvaluation(enemy))
                return false;
            if (!c.PlayerEvaluation(player))
                return false;
            if (!c.FieldEvaluation(field))
                return false;
        }
        return ret;
    }
}
public interface IConditional
{
    /// <summary>フィールド効果を評価</summary>
    bool FieldEvaluation(Field field);
    /// <summary>プレイヤーを評価</summary>
    bool PlayerEvaluation(Player player);
    /// <summary>敵を評価</summary>
    bool EnemyEvaluation(Enemy enemy);
}
public class EnemyLifeConditional : IConditional
{
    [SerializeField] int m_life;
    [SerializeField] EvaluationType m_evaluationType;

    public bool EnemyEvaluation(Enemy enemy) => ConditionalHelper.Evaluation(m_evaluationType, enemy.CurrentLife, m_life);
    public bool FieldEvaluation(Field field) => true;
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
}
#endregion
#endregion