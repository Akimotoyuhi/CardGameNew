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
    [SerializeReference, SubclassSelector] ISample m_sample;

    private void Start()
    {
        Debug.Log($"power:{m_sample.Execute().Power}  def:{m_sample.Execute().Block}");
    }
}

public interface ISample
{
    public Command Execute();
}

public class Oya
{
    [SerializeField] int m_power;

    protected void Method(ref Command command)
    {
        command.Power = m_power;
    }
}

public class A : Oya, ISample
{
    [SerializeField] int m_def;

    public Command Execute()
    {
        Command ret = new Command();
        ret.Block = m_def;
        Method(ref ret);
        return ret;
    }
}

public class B : ISample
{
    [SerializeField] int m_def;
    public Command Execute()
    {
        Command ret = new Command();
        ret.Block = m_def;
        return ret;
    }
}
