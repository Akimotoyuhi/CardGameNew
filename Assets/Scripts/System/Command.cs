using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������ɂȂ񂩂���Ƃ��̏��󂯓n���p�\����
/// </summary>
public struct Command
{
    public int Life { get; set; }
    /// <summary>�U���͒l</summary>
    public int Power { get; set; }
    /// <summary>�u���b�N�l</summary>
    public int Block { get; set; }
    /// <summary>�G�t�F�N�g</summary>
    public List<EffectBase> Effect { get; set; }
    /// <summary>���̃R�}���h���s���ɗv���鎞��</summary>
    public float Duration { get; set; }
    /// <summary>���ʑΏ�</summary>
    public UseType UseType { get; set; }
    /// <summary>�R�}���h�̎��</summary>
    public CommandType CommandType { get; set; }
    /// <summary>�G�s���\��p</summary>
    public PlanType PlanType { get; set; }
    /// <summary>�G��ΏۂƂ���ꍇ�̓G�C���f�b�N�X</summary>
    public int TargetEnemyIndex { get; set; }
    /// <summary>�X�g�b�N���Ă����^�[����</summary>
    public int StockTurn { get; set; }
    /// <summary>�X�g�b�N�I�����Ɏ��s�����</summary>
    public bool IsStockRelease { get; set; }
    /// <summary>�X�g�b�N���ɕ\������摜</summary>
    public Sprite StockSprite { get; set; }
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
/// <summary>�퓬���̃t�B�[���h����</summary>
public struct Field
{
    public int CurrentTurn { get; set; }
}
#region �R�}���h�ݒ�N���X�֘A
/// <summary>
/// �t�^����R�}���h��ݒ肷��N���X
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
/// �ݒ肷��R�}���h��؂�ւ���ׂ̃C���^�[�t�F�[�X
/// </summary>
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
    [SerializeField] bool m_isHideType;
    [SerializeField] int m_stockTurn = -1;
    [SerializeField] bool m_isStockRelease;

    public Command Execute()
    {
        Command ret = new Command();
        ret.Power = m_power;
        ret.UseType = m_useType;
        ret.Duration = m_duration;
        ret.CommandType = CommandType.Attack;
        ret.StockTurn = m_stockTurn;
        ret.IsStockRelease = m_isStockRelease;
        if (m_isHideType)
            ret.PlanType = PlanType.Other;
        else
            ret.PlanType = PlanType.Attack;
        return ret;
    }
}

public class BlockCommand : ICommand
{
    [SerializeField] UseType m_useType;
    [SerializeField] int m_block;
    [SerializeField] bool isTrueBlock;
    [SerializeField] float m_duration;
    [SerializeField] bool m_isHideType;
    [SerializeField] int m_stockTurn = -1;
    [SerializeField] bool m_isStockRelease;

    public Command Execute()
    {
        Command ret = new Command();
        ret.Block = m_block;
        ret.UseType = m_useType;
        ret.Duration = m_duration;
        ret.PlanType = PlanType.Block;
        ret.CommandType = CommandType.Block;
        ret.StockTurn = m_stockTurn;
        ret.IsStockRelease = m_isStockRelease;
        if (m_isHideType)
            ret.PlanType = PlanType.Other;
        else
            ret.PlanType = PlanType.Block;
        return ret;
    }
}
public class EffectCommand : ICommand
{
    [SerializeField] UseType m_useType;
    [SerializeField] float m_duration;
    [SerializeField] EffectSelector m_effectSelector;
    [SerializeField] bool m_isHideType;
    [SerializeField] int m_stockTurn = -1;
    [SerializeField] bool m_isStockRelease;
    [SerializeField] PlanType m_otherBuffTypeCommandType = PlanType.Other;
    public Command Execute()
    {
        Command ret = new Command();
        List<EffectBase> effects = new List<EffectBase>();
        effects.Add(m_effectSelector.GetEffect);
        ret.Effect = effects;
        ret.Duration = m_duration;
        ret.CommandType = CommandType.Effect;
        ret.StockTurn = m_stockTurn;
        ret.IsStockRelease = m_isStockRelease;
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
                    Debug.LogWarning("�z��O�̃o�t�^�C�v�����o����܂���");
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
#region �R�}���h���s�����n
[System.Serializable]
public class Conditional
{
    [SerializeField, Range(0, 100), Tooltip("���I�m��")] int m_probability = 100;
    [SerializeField, Tooltip("�ő�s����\n-1�Ȃ琧���Ȃ�\n���݋@�\���ĂȂ��ł�")] int m_playNum = -1;
    [SerializeReference, SubclassSelector] List<IConditional> m_conditionals;
    /// <summary>
    /// �����̕]��
    /// </summary>
    public bool Evaluation(Field field, Player player, Enemy enemy)
    {
        //���I�m���̕]��
        int r = Random.Range(1, 101);
        if (m_probability < r)
            return false;

        //IConditional�̕]��
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
    /// <summary>�t�B�[���h���ʂ�]��</summary>
    bool FieldEvaluation(Field field);
    /// <summary>�v���C���[��]��</summary>
    bool PlayerEvaluation(Player player);
    /// <summary>�G��]��</summary>
    bool EnemyEvaluation(Enemy enemy);
}
/// <summary>�G�̗̑͂�]������</summary>
public class EnemyLifeConditional : IConditional
{
    [SerializeField] int m_life;
    [SerializeField] EvaluationType m_evaluationType;

    public bool EnemyEvaluation(Enemy enemy) =>
        ConditionalHelper.Evaluation(m_evaluationType, enemy.CurrentLife, m_life);
    public bool FieldEvaluation(Field field) => true;
    public bool PlayerEvaluation(Player player) => true;
}
/// <summary>���݂̃^�[������]������</summary>
public class TurnConditional : IConditional
{
    [SerializeField] int m_turn;
    [SerializeField] EvaluationType m_evaluationType;

    public bool EnemyEvaluation(Enemy enemy) => true;
    public bool FieldEvaluation(Field field) =>
        ConditionalHelper.Evaluation(m_evaluationType, field.CurrentTurn, m_turn);
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
    /// <summary>X����</summary>
    AtIntervalsOf
}
/// <summary>
/// �G�s���\��p
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
/// �R�}���h�̎��
/// </summary>
public enum CommandType
{
    Attack,
    Block,
    Effect,
}
#endregion
#endregion