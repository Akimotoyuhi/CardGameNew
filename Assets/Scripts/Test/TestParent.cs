using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class TestParent : MonoBehaviour
{
    [SerializeField] string m_str;
    [SerializeField] int m_value;
    [SerializeField] string m_str2;
    [SerializeField] int m_value2;
    [SerializeField] Sample2 m_prefab;

    private void Start()
    {
        Sample2 s = Sample2.Init(m_prefab, m_str, m_value);
        s.transform.SetParent(transform, false);
        Debug.Log($"ê∂ê¨ÇµÇΩÅ@Sample1 Str : {s.Str}, Value : {s.Value}");

        s = Sample2.Init(m_prefab, m_str2, m_value2);
        s.transform.SetParent(transform, false);
        Debug.Log($"ê∂ê¨ÇµÇΩÅ@Sample2 Str : {s.Str}, Value : {s.Value}");
    }
}
