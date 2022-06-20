using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>スクリプトの名前変更用</summary>
public class Effects { }
public class Weekness : EffectBase
{
    public override string Tooltip => $"脱力\n与えるダメージが<color=#ff0000>25%低下</color>。<color=#ffff00>{Turn}</color>ターン持続";
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
    public override EffectID GetEffectID => EffectID.Weekness;
    public override Command Effect(List<ConditionalParametor> evaluationParametors)
    {
        throw new System.NotImplementedException();
    }
}
public class Frail : EffectBase
{
    public override string Tooltip => $"虚弱\n得るブロックが<color=#ff0000>25%低下</color>。<color=#ffff00>{Turn}</color>ターン持続";
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
        throw new System.NotImplementedException();
    }
}