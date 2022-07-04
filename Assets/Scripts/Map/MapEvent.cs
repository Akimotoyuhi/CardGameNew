using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEvent : MonoBehaviour
{
    [SerializeField] Rest m_rest;









    [System.Serializable]
    public class Rest
    {
        [SerializeField] int m_healValue;
        [SerializeField] int m_upgradeNum;
        [SerializeField] int m_clearCardNum;

        public void Healing()
        {

        }

        public void Upgrade()
        {

        }

        public void Clear()
        {

        }
    }
}
