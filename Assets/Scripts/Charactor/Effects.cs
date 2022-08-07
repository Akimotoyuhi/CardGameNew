using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>スクリプトの名前変更用</summary>
public class Effects { }
public class Weakness : EffectBase
{
    public override string Tooltip => $"脱力\n与えるダメージが<color=#ff0000>25%低下</color>。{Turn}ターン持続";
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
        //攻撃時かつ渡されたパラメータタイプがアタックなら値を25%減らして返す
        if (powerDecFlag)
        {
            float f = power * (1 - 0.25f);
            ret.Power = (int)f;
        }
        //ターン終了時なら持続ターンを１減らす
        if (turnDecFlag)
        {
            Turn--;
        }
        return ret;
    }
}
public class Frail : EffectBase
{
    public override string Tooltip => $"虚弱\n得るブロックが<color=#ff0000>25%低下</color>。{Turn}ターン持続";
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
        //攻撃時かつ渡されたパラメータタイプが防御なら値を25%減らす
        if (blockDecFlag)
        {
            float f = block * (1 - 0.25f);
            ret.Block = (int)f;
        }
        //ターン終了時なら持続ターンを１減らす
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
                return $"与えるダメージが<color=#0000ff>+{Turn}</color>";
            else
                return $"与えるダメージが<color=#ff0000>{Turn}</color>";
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
                return $"得るブロックが<color=#0000ff>+{Turn}</color>";
            else
                return $"得るブロックが<color=#ff0000>{Turn}</color>";
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
public class Vitality : EffectBase
{
    public override string Tooltip => $"活力\n与えるダメージが<color=#0000ff>25%増加</color>。{Turn}ターン持続";
    public override bool IsRemove => Turn <= 0;
    public override BuffType GetBuffType => BuffType.Buff;
    public override EffectID GetEffectID => EffectID.Vitality;
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
        if (powerDecFlag)
        {
            float f = power * (1 + 0.25f);
            ret.Power = (int)f;
        }
        if (turnDecFlag)
        {
            Turn--;
        }
        return ret;
    }
}
public class Sturdy : EffectBase
{
    public override string Tooltip => $"頑丈\n得るブロックが<color=#0000ff>25%増加</color>。{Turn}ターン持続";
    public override bool IsRemove => Turn <= 0;
    public override BuffType GetBuffType => BuffType.Buff;
    public override EffectID GetEffectID => EffectID.Sturdy;
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
        if (blockDecFlag)
        {
            float f = block * (1 + 0.25f);
            ret.Block = (int)f;
        }
        if (turnDecFlag)
        {
            Turn--;
        }
        return ret;
    }
}