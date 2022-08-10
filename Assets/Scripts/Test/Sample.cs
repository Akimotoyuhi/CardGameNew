using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UniRx;
using System;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        Method(1, 2);
    }

    private void Method<T>(T a, T b)
    {
        Debug.Log(a);
    }
}

public interface ITekitounaInterface
{

}