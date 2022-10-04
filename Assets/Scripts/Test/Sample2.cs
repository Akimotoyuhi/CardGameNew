using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Sample2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Text m_text;
    private string m_str;
    private int m_int;
    public string Str => m_str;
    public int Value => m_int;

    public static Sample2 Init(Sample2 sample2, string str, int value)
    {
        Sample2 ret = Instantiate(sample2);
        ret.Setup(str, value);
        return ret;
    }

    public void Setup(string str, int value)
    {
        m_str = str;
        m_int = value;
        m_text.text = m_int.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
        GameManager.Instance.SetInfoText = m_str;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
        GameManager.Instance.SetInfoText = "";
    }
}
