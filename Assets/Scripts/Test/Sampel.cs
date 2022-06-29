using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sampel : MonoBehaviour
{
    [System.Serializable]
    public class AAAA
    {
        [SerializeField, PreviewButton] int m_num;
        //[SerializeField] int m_num;
    }
    [SerializeField] List<AAAA> m_aaaa;

    void Start()
    {
        
    }
}
