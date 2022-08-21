using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// パーティクルを管理する
/// </summary>
[System.Serializable]
public class Particler : MonoBehaviour
{
    public void Setup(Component component)
    {

    }
}
/// <summary>
/// パーティクルID
/// </summary>
public enum ParticleID
{
    None = -1,
}