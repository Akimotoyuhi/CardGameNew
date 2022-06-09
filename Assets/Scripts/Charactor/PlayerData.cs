using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] List<PlayerDataBase> m_dataBase;
    public List<PlayerDataBase> DataBase => m_dataBase;
}

[System.Serializable]
public class PlayerDataBase
{
    [SerializeField] string m_label;
    [SerializeField] CharactorDataBase m_characterData;
    [SerializeField] CardClassSelector m_cardClassSelector;
    public CharactorDataBase CharactorData => m_characterData;
    public CardClassSelector CardClassSelector => m_cardClassSelector;
}

public enum PlayerID
{
    Challenger,
    Magallan,
}