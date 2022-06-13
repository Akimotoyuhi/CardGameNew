using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class Sample : MonoBehaviour
{
    [SerializeField] Text m_text;

    async void Start()
    {
        await UniTask.Delay(1);
        string s = "‚ ‚ ‚ ‚ ‚ ‚ ‚ ‚ ‚ ‚ ‚ ‚ ‚ ‚ ‚ \n‚¢‚¢‚¢‚¢‚¢‚¢‚¢‚¢‚¢‚¢‚¢‚¢‚¢‚¢‚¢‚¢";
        m_text.DOText(s, 1);
    }

    void Update()
    {
        
    }
}
