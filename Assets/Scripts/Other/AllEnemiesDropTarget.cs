using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class AllEnemiesDropTarget : MonoBehaviour, IDrop
{
    

    public void Setup()
    {

    }

    public bool GetDrop(ref List<Command> commands) => true;

    public UseType GetUseType() => UseType.AllEnemies;
}
