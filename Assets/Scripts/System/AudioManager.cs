using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public AudioManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
    }
}
/// <summary>
/// BGM��ID
/// </summary>
public enum BGMID
{
    None = -1,
}
/// <summary>
/// SE��ID
/// </summary>
public enum SEID
{
    None = -1,
}