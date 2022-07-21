using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class EventManager : MonoBehaviour
{
    [SerializeField] Rest m_rest;
    /// <summary>‹xŒeƒ}ƒX</summary>
    public Rest RestEvent => m_rest;
    private ReactiveProperty<MapEventState> m_mapState = new ReactiveProperty<MapEventState>();
    public System.IObservable<MapEventState> MapEventStateObservable => m_mapState;

    public void Setup()
    {

    }







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

public enum MapEventState
{
    Rest,
    Upgrade,
    Clear,
}