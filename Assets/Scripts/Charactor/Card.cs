using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using System.Text.RegularExpressions;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] Text m_nameText;
    [SerializeField] Image m_icon;
    [SerializeField] Text m_tooltipText;
    [SerializeField] Text m_costText;
    [SerializeField] Image m_cardTypeImage;
    [SerializeField] RectTransform m_rectTransform;
    [SerializeField, Tooltip("CardType�Ɋ�Â����J�[�h�E��̃A�C�R���̉摜\n�v�f���ɍU��\n�h��\n�o�t\n�f�o�t\n����\n�̏�")]
    private List<CardTypeSpriteSettings> m_cardTypeSpriteSettings;
    [System.Serializable]
    public class CardTypeSpriteSettings
    {
        [SerializeField] Sprite m_sprite;
        [SerializeField] Color m_color = Color.white;
        public Sprite Sprite => m_sprite;
        public Color Color => m_color;
    }
    private int m_cost;
    private bool m_isDrag;
    private UseType m_useType;
    private Rarity m_rarity;
    private CardType m_cardType;
    private CardDataBase m_database;
    private List<Command> m_cardCommands;
    private Vector2 m_defPos;
    private Player m_player;
    private Subject<List<Command>> m_cardUsed = new Subject<List<Command>>();
    public string Name { get; private set; }
    public CardState CardState { get; set; }
    public System.IObservable<List<Command>> CardUsed => m_cardUsed;

    public void Setup(CardDataBase dataBase, Player player)
    {
        m_player = player;
        SetBaseData(dataBase);
        SetTooltip(m_tooltipText.text);
    }

    private void SetBaseData(CardDataBase database)
    {
        m_database = database;
        Name = database.Name;
        m_nameText.text = database.Name;
        m_icon.sprite = database.Icon;
        m_tooltipText.text = database.Tooltip;
        m_costText.text = database.Cost;
        m_rarity = database.Rarity;
        m_cardType = database.CardType;
        m_cardTypeImage.sprite = m_cardTypeSpriteSettings[(int)m_cardType].Sprite;
        m_cardTypeImage.color = m_cardTypeSpriteSettings[(int)m_cardType].Color;
        try
        {
            m_cost = int.Parse(database.Cost);
        }
        catch
        {
            Debug.LogError("�L���X�g�s�Ȓl�����o���ꂽ�̂ŁA�K���Ȓl���ݒ肳��܂���");
            m_cost = m_player.MaxCost;
        }
        m_useType = database.CardUseType;
        m_cardCommands = database.CardCommands.Execute();
    }

    /// <summary>
    /// �J�[�h���ʂ̎��s
    /// </summary>
    /// <param name="target"></param>
    private void Execute(IDrop target)
    {
        UseType ut = target.GetUseType();
        if (ut != m_useType)
            return;
        if (m_player.CurrentCost < m_cost)
        {
            Debug.Log("�R�X�g����Ȃ�");
            return;
        }
        List<Command> cmds = new List<Command>();
        m_database.CardCommands.Execute().ForEach(c => cmds.Add(c));
        target.GetDrop(ref cmds);
        m_cardUsed.OnNext(cmds);
        m_player.CurrentCost -= m_cost;
        Used();
    }

    public void SetTooltip(string text)
    {
        string s = text;
        MatchCollection matchs = Regex.Matches(s, "{pow([0-9]*)}");
        foreach (Match m in matchs)
        {
            int index = int.Parse(m.Groups[1].Value);
            s = s.Replace(m.Value, $"{m_cardCommands[index].Power}�_���[�W");
        }
        matchs = Regex.Matches(s, "{blk([0-9]*)}");
        foreach (Match m in matchs)
        {
            int index = int.Parse(m.Groups[1].Value);
            s = s.Replace(m.Value, $"{m_cardCommands[index].Block}�u���b�N");
        }
        m_tooltipText.text = s;
    }

    /// <summary>
    /// �g�p���ꂽ��̏���
    /// </summary>
    private void Used()
    {

    }

    //�ȉ��C���^�[�t�F�[�X

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!m_isDrag)
            m_defPos = m_rectTransform.anchoredPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_isDrag = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_rectTransform.anchoredPosition = new Vector2(eventData.position.x, eventData.position.y - 150);//�J�[�\�����J�[�h�����ɗ���悤�ɒ���
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_rectTransform.anchoredPosition = m_defPos;
        m_isDrag = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnDrop(PointerEventData eventData)
    {
        var result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, result);
        foreach (var hit in result)
        {
            IDrop target = hit.gameObject.GetComponent<IDrop>();
            if (target == null)
                continue;
            Execute(target);
            return;
        }
    }
}

public enum CardState
{
    None,
    Play,
}