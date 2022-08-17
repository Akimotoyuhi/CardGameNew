using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// ƒ^ƒCƒgƒ‹‰æ–Ê‚ÌŠÇ—
/// </summary>
public class TitleManager : MonoBehaviour
{
    [SerializeField] PlayerData m_playerData;
    public PlayerData PlayerData => m_playerData;

    private void Start()
    {
        
    }

    public async void GameStart()
    {
         await TitleGUIManager.Fade(Color.black, 0.5f,
             () => SceneManager.LoadScene("GameScene"));
    }
}

public enum TitleState
{
    Title,
    CharactorSelect,
    CustomSelect,
}
