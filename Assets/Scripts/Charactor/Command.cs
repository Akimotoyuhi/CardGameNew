using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Command
{
    public int Power { get; set; }
    public int Block { get; set; }
    //public List<Effect> Effect { get; set; }
    public UseType UseType { get; set; }
    public int TargetEnemyIndex { get; set; }
}
