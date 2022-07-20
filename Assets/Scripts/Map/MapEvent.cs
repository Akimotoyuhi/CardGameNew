using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEvent : MonoBehaviour
{
    [SerializeField] Rest m_rest;

    public Rest RestEvent => m_rest;







    [System.Serializable]
    public class Rest
    {
        [SerializeField] int m_healValue;
        [SerializeField] int m_upgradeNum;
        [SerializeField] int m_clearCardNum;
        /// <summary>�񕜗�</summary>
        public int Heal => m_healValue;
        /// <summary>��������</summary>
        public int UpgradeNum => m_upgradeNum;
        /// <summary>�폜����</summary>
        public int ClearNum => m_clearCardNum;
    }
}
