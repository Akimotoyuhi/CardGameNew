using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UniRx;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Sample : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //[SerializeReference, SubclassSelector] ISample m_sample;

    private void Start()
    {
        //Debug.Log($"power:{m_sample.Execute().Power}  def:{m_sample.Execute().Block}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
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
