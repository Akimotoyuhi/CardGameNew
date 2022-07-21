using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameManager : MonoBehaviour
{
    [SerializeField] BattleManager m_battleManager;
    [SerializeField] EventManager m_eventManager;
    [SerializeField] GUIManager m_guiManager;
    [SerializeField] MapManager m_mapManager;
    private ReactiveProperty<GameState> m_gameState = new ReactiveProperty<GameState>();
    private ReactiveProperty<string> m_infoText = new ReactiveProperty<string>();
    private ReactiveProperty<int> m_floor = new ReactiveProperty<int>();
    public static GameManager Instance { get; private set; }
    /// <summary>GameState���ύX���ꂽ����ʒm����</summary>
    public System.IObservable<GameState> GameStateObservable => m_gameState;
    /// <summary>�C���t�H���[�V�����e�L�X�g�̍X�V�p</summary>
    public string SetInfoText { set => m_infoText.Value = value; }
    /// <summary>�C���t�H���[�V�����e�L�X�g���X�V���ꂽ��ʒm����</summary>
    public System.IObservable<string> InfoTextUpdate => m_infoText;
    /// <summary>���݂̊K�w���X�V���ꂽ��ʒm����</summary>
    public System.IObservable<int> FloorUpdate => m_floor;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //�o�g���}�l�[�W���[�̃Z�b�g�A�b�v
        m_battleManager.Setup();
        m_battleManager.BattleFinished
            .Subscribe(_ => FloorFinished()).AddTo(m_battleManager);

        //GUI�}�l�[�W���[�̃Z�b�g�A�b�v
        m_guiManager.Setup();

        //�}�b�v�}�l�[�W���[�̃Z�b�g�A�b�v
        m_mapManager.Setup();
        m_mapManager.EncountObservable.Subscribe(ct => Encount(ct)).AddTo(m_mapManager);

        //�ŏ��̓}�b�v�Z���N�g��ʂ���n�܂�
        m_gameState.Value = GameState.MapSelect;
        m_floor.Value = 1;
    }

    /// <summary>
    /// �����ꂽ�}�b�v�̃}�X�ɉ����ĉ�������̏���������
    /// </summary>
    /// <param name="cellType"></param>
    private void Encount(CellType cellType)
    {
        switch (cellType)
        {
            case CellType.Rest:
                m_gameState.Value = GameState.Rest;
                break;
            default:
                //�퓬�}�X�������ꍇ
                m_gameState.Value = GameState.Battle;
                m_battleManager.Encount(m_mapManager.NowMapID, cellType);
                break;
        }
    }

    /// <summary>
    /// �K�w�̍X�V
    /// </summary>
    public void FloorFinished()
    {
        m_floor.Value++;
        Debug.Log($"���݃t���A {m_floor.Value}");
        m_gameState.Value = GameState.MapSelect;
    }

    /// <summary>
    /// �J�[�h�ꗗ��\������
    /// </summary>
    /// <param name="displayType">�\������`��</param>
    /// <param name="cards">�\������J�[�h</param>
    /// <param name="onClick">�J�[�h���N���b�N�����ۂ̐U�镑��</param>
    public void CardDisplay(CardDisplayType displayType, List<Card> cards, System.Action onClick) => 
        m_guiManager.CardDisplay(displayType, cards, onClick);
}

public enum GameState
{
    MapSelect,
    Battle,
    Rest,
}