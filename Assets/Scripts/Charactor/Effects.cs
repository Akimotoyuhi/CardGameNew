using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>�X�N���v�g�̖��O�ύX�p</summary>
public class Effects { }
public class Weekness : EffectBase
{
    public override string Tooltip => $"�E��\n�^����_���[�W��<color=#ff0000>25%�ቺ</color>�B<color=#ffff00>{Turn}</color>�^�[������";
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
    public override string Tooltip => $"����\n����u���b�N��<color=#ff0000>25%�ቺ</color>�B<color=#ffff00>{Turn}</color>�^�[������";
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