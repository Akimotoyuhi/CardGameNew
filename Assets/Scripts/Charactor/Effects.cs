using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>スクリプトの名前変更用</summary>
public class Effects { }
public class Weakness : EffectBase
{
    public override string Tooltip => $"脱力\n与えるダメージが<color=#ff0000>25%低下</color>。{Turn}ターン持続";
    public override bool IsRemove
    {
        get
        {
            if (Turn <= 0)
                return true;
            else
                return false;
        }
    }
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
            if (ep.EffectTiming == EffectTiming.Attacked && ep.EvaluationParamType == EvaluationParamType.Attack)
            {
                Debug.Log("a");
                power = ep.Parametor;
                continue;
            }
            else
            {
                power = ep.Parametor;
                powerDecFlag = false;
            }
            if (ep.EffectTiming == EffectTiming.TurnEnd)
                continue;
            else
                turnDecFlag = false;
        }
        if (powerDecFlag)
        {
            Debug.Log($"評価前{power}");
            float f = power * (1 - 0.25f);
            Debug.Log($"評価gお{f}");
            ret.Power = (int)f;
        }
        if (turnDecFlag)
            Turn--;
        return ret;
    }
}
public class Frail : EffectBase
{
    public override string Tooltip => $"虚弱\n得るブロックが<color=#ff0000>25%低下</color>。{Turn}ターン持続";
    public override bool IsRemove
    {
        get
        {
            if (Turn <= 0)
                return true;
            else
                return false;
        }
    }
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
            if (ep.EffectTiming == EffectTiming.Attacked && ep.EvaluationParamType == EvaluationParamType.Block)
            {
                block = ep.Parametor;
                continue;
            }
            else
                blockDecFlag = false;
            if (ep.EffectTiming == EffectTiming.TurnEnd)
                continue;
            else
                turnDecFlag = false;
        }
        if (blockDecFlag)
        {
            float f = block * (1 - 0.25f);
            ret.Block = (int)f;
        }
        if (turnDecFlag)
            Turn--;
        return ret;
    }
}