using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectBase
{
    public int Turn { get; set; }
    public abstract Command Effect(Charactor charactor);
    public abstract bool GetBuffType { get; }
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
public enum EffectID
{

}
/// <summary>バフかデバフかそれ以外か</summary>
public enum BuffType
{
    Buff,
    Debuff,
    Soreigai,
}