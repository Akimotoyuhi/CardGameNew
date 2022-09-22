using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class AllEnemiesDropTarget : MonoBehaviour, IDrop
{
    [SerializeField] GameObject m_dropEriaObject;

    public void Setup()
    {
        m_dropEriaObject.SetActive(false);
    }

    public bool GetDrop(ref List<Command> commands) => true;

    public UseType GetUseType => UseType.AllEnemies;

    public void ShowDropEria(UseType useType)
    {
        if (GetUseType == useType)
        {
            m_dropEriaObject.SetActive(true);
        }
        else
        {
            m_dropEriaObject.SetActive(false);
        }
    }
}
