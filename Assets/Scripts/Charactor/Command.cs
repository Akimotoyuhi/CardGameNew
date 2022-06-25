using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Command
{
    public int Life { get; set; }
    public int Power { get; set; }
    public int Block { get; set; }
    public List<EffectBase> Effect { get; set; }
    public UseType UseType { get; set; }
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
public struct Field
{
    public int CurrentTurn { get; set; }
}
#region �R�}���h�ݒ�N���X�֘A
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

public interface ICommand
{
    Command Execute();
}

public class AttackCommand : ICommand
{
    [SerializeField] UseType m_useType = UseType.None;
    [SerializeField] int m_power;
    [SerializeField] bool m_isTrueDamage;

    public Command Execute()
    {
        Command ret = new Command();
        ret.Power = m_power;
        ret.UseType = m_useType;
        return ret;
    }
}

public class BlockCommand : ICommand
{
    [SerializeField] UseType m_useType;
    [SerializeField] int m_block;
    [SerializeField] bool isTrueBlock;

    public Command Execute()
    {
        Command ret = new Command();
        ret.Block = m_block;
        ret.UseType = m_useType;
        return ret;
    }
}
public class EffectCommand : ICommand
{
    [SerializeField] UseType m_useType;
    [SerializeField] List<EffectSelector> m_effectSelector;
    public Command Execute()
    {
        Command ret = new Command();
        List<EffectBase> effects = new List<EffectBase>();
        m_effectSelector.ForEach(e => effects.Add(e.GetEffect));
        ret.Effect = effects;
        return ret;
    }
}
#endregion
#region �R�}���h���s�����n
[System.Serializable]
public class Conditional
{
    [SerializeField, Range(0, 100), Tooltip("���I�m��")] int m_probability = 100;
    [SerializeField, Tooltip("�ő�s����\n-1�Ȃ琧���Ȃ�")] int m_playNum = -1;
    [SerializeReference, SubclassSelector] List<IConditional> m_conditionals;
    public bool Evaluation(Field field, Player player, Enemy enemy)
    {
        bool ret = true;
        m_conditionals.ForEach(c =>
        {
            ret = c.EnemyEvaluation(enemy);
            ret = c.PlayerEvaluation(player);
            ret = c.FieldEvaluation(field);
        });
        return ret;
    }
}
public interface IConditional
{
    /// <summary>�t�B�[���h���ʂ�]��</summary>
    bool FieldEvaluation(Field field);
    /// <summary>�v���C���[��]��</summary>
    bool PlayerEvaluation(Player player);
    /// <summary>�G��]��</summary>
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

/// <summary>�l�ƕ]���������琬�ۂ��`�F�b�N����</summary>
public static class ConditionalHelper
{
    /// <summary>�����]��</summary>
    /// <param name="evaluationNum">�]������l</param>
    /// <param name="baseNum">�]�������l</param>
    /// <param name="evaluationType">�]������</param>
    /// <returns>������</returns>
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
/// <summary>�]������</summary>
public enum EvaluationType
{
    /// <summary>�]��������K�����Ȃ�</summary>
    Any,
    /// <summary>������</summary>
    Equal,
    /// <summary>�ȏ�</summary>
    High,
    /// <summary>�ȉ�</summary>
    Low,
}
#endregion
#endregion