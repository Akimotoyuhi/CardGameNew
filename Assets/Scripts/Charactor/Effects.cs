using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>�X�N���v�g�̖��O�ύX�p</summary>
public class Effects { }
public class Weakness : EffectBase
{
    public override string Tooltip => $"�E��\n�^����_���[�W��<color=#ff0000>25%�ቺ</color>�B{Turn}�^�[������";
    public override bool IsRemove => Turn <= 0;
    public override BuffType GetBuffType => BuffType.Debuff;
    public override EffectID GetEffectID => EffectID.Weakness;
    public override Command Effect(List<ConditionalParametor> evaluationParametors)
    {
        Command ret = new Command();
        bool powerDecFlag = true;
        bool turnDecFlag = true;
        int power = 0;
        foreach (var ep in evaluationParametors)
        {
            ret.SetCommand(ep);
            if (ep.EffectTiming == EffectTiming.Attacked && ep.EvaluationParamType == EvaluationParamType.Attack)
            {
                power = ep.Parametor;
            }
            else
            {
                power = ep.Parametor;
                powerDecFlag = false;
            }
            if (ep.EffectTiming != EffectTiming.TurnEnd)
            {
                turnDecFlag = false;
            }
        }
        //�U�������n���ꂽ�p�����[�^�^�C�v���A�^�b�N�Ȃ�l��25%���炵�ĕԂ�
        if (powerDecFlag)
        {
            float f = power * (1 - 0.25f);
            ret.Power = (int)f;
        }
        //�^�[���I�����Ȃ玝���^�[�����P���炷
        if (turnDecFlag)
        {
            Turn--;
        }
        return ret;
    }
}
public class Frail : EffectBase
{
    public override string Tooltip => $"����\n����u���b�N��<color=#ff0000>25%�ቺ</color>�B{Turn}�^�[������";
    public override bool IsRemove => Turn <= 0;
    public override BuffType GetBuffType => BuffType.Debuff;
    public override EffectID GetEffectID => EffectID.Frail;
    public override Command Effect(List<ConditionalParametor> evaluationParametors)
    {
        Command ret = new Command();
        bool blockDecFlag = true;
        bool turnDecFlag = true;
        int block = 0;
        foreach (var ep in evaluationParametors)
        {
            ret.SetCommand(ep);
            if (ep.EffectTiming == EffectTiming.Attacked && ep.EvaluationParamType == EvaluationParamType.Block)
            {
                block = ep.Parametor;
            }
            else
            {
                block = ep.Parametor;
                blockDecFlag = false;
            }
            if (ep.EffectTiming != EffectTiming.TurnEnd)
            {
                turnDecFlag = false;
            }
                
        }
        //�U�������n���ꂽ�p�����[�^�^�C�v���h��Ȃ�l��25%���炷
        if (blockDecFlag)
        {
            float f = block * (1 - 0.25f);
            ret.Block = (int)f;
        }
        //�^�[���I�����Ȃ玝���^�[�����P���炷
        if (turnDecFlag)
        {
            Turn--;
        }
        return ret;
    }
}
public class Strength : EffectBase
{
    public override string Tooltip
    {
        get
        {
            if (Turn > 0)
                return $"�^����_���[�W��<color=#0000ff>+{Turn}</color>";
            else
                return $"�^����_���[�W��<color=#ff0000>{Turn}</color>";
        }
    }
    public override bool IsRemove => Turn == 0;
    public override BuffType GetBuffType => BuffType.Buff;
    public override EffectID GetEffectID => EffectID.Strength;
    public override Command Effect(List<ConditionalParametor> evaluationParametors)
    {
        Command ret = new Command();
        bool powerIncFlag = true;
        int power = 0;
        foreach (var ep in evaluationParametors)
        {
            ret.SetCommand(ep);
            if (ep.EffectTiming == EffectTiming.Attacked && ep.EvaluationParamType == EvaluationParamType.Attack)
            {
                power = ep.Parametor;
            }
            else
            {
                power = ep.Parametor;
                powerIncFlag = false;
            }
        }
        if (powerIncFlag)
            ret.Power = power + Turn;
        return ret;
    }
}
public class Agile : EffectBase
{
    public override string Tooltip
    {
        get
        {
            if (Turn > 0)
                return $"����u���b�N��<color=#0000ff>+{Turn}</color>";
            else
                return $"����u���b�N��<color=#ff0000>{Turn}</color>";
        }
    }
    public override bool IsRemove => Turn == 0;
    public override BuffType GetBuffType => BuffType.Buff;
    public override EffectID GetEffectID => EffectID.Agile;
    public override Command Effect(List<ConditionalParametor> evaluationParametors)
    {
        Command ret = new Command();
        bool blockIncFlag = true;
        int block = 0;
        foreach (var ep in evaluationParametors)
        {
            ret.SetCommand(ep);
            if (ep.EffectTiming == EffectTiming.Attacked && ep.EvaluationParamType == EvaluationParamType.Block)
            {
                block = ep.Parametor;
            }
            else
            {
                block = ep.Parametor;
                blockIncFlag = false;
            }
        }
        if (blockIncFlag)
            ret.Block = block + Turn;
        return ret;
    }
}