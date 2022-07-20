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
        /// <summary>‰ñ•œ—Ê</summary>
        public int Heal => m_healValue;
        /// <summary>‹­‰»–‡”</summary>
        public int UpgradeNum => m_upgradeNum;
        /// <summary>íœ–‡”</summary>
        public int ClearNum => m_clearCardNum;
    }
}
