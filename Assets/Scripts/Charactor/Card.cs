using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using System.Text.RegularExpressions;

/// <summary>
/// �J�[�h�̎���
/// </summary>
public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] Image m_background;
    [SerializeField] Text m_nameText;
    [SerializeField] Image m_icon;
    [SerializeField] Text m_tooltipText;
    [SerializeField] Text m_costText;
    [SerializeField] List<Image> m_cardTypeImages;
    [SerializeField] RectTransform m_rectTransform;
    [SerializeField] float m_releaseDistance;
    private int m_cost;
    private string m_tooltip;
    /// <summary>�h���b�O���t���O</summary>
    private bool m_isDrag;
    /// <summary>�w�i�摜</summary>
    private Sprite m_backgroundSprite;
    /// <summary>�g�p�Ώ�</summary>
    private UseType m_useType;
    /// <summary>���A�x</summary>
    private Rarity m_rarity;
    /// <summary>�J�[�h�̎��</summary>
    private List<CardType> m_cardType;
    private CardDataBase m_database;
    private List<Command> m_cardCommands;
    private Vector2 m_defPos;
    private Vector2 m_mouseClickPos;
    private Player m_player;
    private Subject<Unit> m_onClick = new Subject<Unit>();
    private Subject<List<Command>> m_cardExecute = new Subject<List<Command>>();
    /// <summary>���O</summary>
    public string Name { get; private set; }
    /// <summary>�{�^���Ƃ��ĕ\�����ꂽ�ۂɉ��Ԗڂɐ������ꂽ�����L�����Ă����p</summary>
    public int Index { get; set; }
    /// <summary>���</summary>
    public CardState CardState { get; set; }
    public System.IObservable<Unit> OnClickSubject => m_onClick;
    /// <summary>�g�p���ꂽ����ʒm����</summary>
    public System.IObservable<List<Command>> CardExecute => m_cardExecute;

    public void Setup(CardDataBase dataBase, List<CardData.RaritySprite> raritySprite, List<CardData.TypeSprite> typeSprite, Player player)
    {
        m_player = player;
        SetBaseData(dataBase);
        SetSprites(raritySprite, typeSprite);
        GetPlayerEffect();
    }

    //public void Setup(CardDataBase dataBase, List<CardData.RaritySprite> raritySprite, List<CardData.TypeSprite> typeSprite)
    //{
    //    SetBaseData(dataBase);
    //    SetSprites(raritySprite, typeSprite);
    //    GetPlayerEffect();
    //}

    public void Init()
    {
        m_cardCommands = m_database.CardCommands.Execute();
    }

    /// <summary>�^����ꂽ�J�[�h�f�[�^�����g�ɐݒ肷��</summary>
    /// <param name="database"></param>
    private void SetBaseData(CardDataBase database)
    {
        m_database = database;
        Name = database.Name;
        m_nameText.text = database.Name;
        m_icon.sprite = database.Icon;
        m_tooltip = database.Tooltip;
        m_costText.text = database.Cost;
        m_rarity = database.Rarity;
        m_cardType = database.CardType;
        //����R�X�gx�Ƃ���肽���̂ŕ�����Ŏ���悤�ɂ��Ă���
        try
        {
            m_cost = int.Parse(database.Cost);
        }
        catch
        {
            Debug.Log("�L���X�g�s�Ȓl�����o���ꂽ�̂ŁA�K���Ȓl���ݒ肳��܂���");
            m_cost = m_player.MaxCost;
        }
        m_useType = database.CardUseType;
        m_cardCommands = database.CardCommands.Execute();
    }

    /// <summary>
    /// �w�i�摜�ƃA�C�R�����̉摜�̐ݒ�
    /// </summary>
    /// <param name="backgroundSprites"></param>
    /// <param name="typeSprites"></param>
    private void SetSprites(List<CardData.RaritySprite> backgroundSprites, List<CardData.TypeSprite> typeSprites)
    {
        //�w�i�摜�̐ݒ�
        foreach (var b in backgroundSprites)
        {
            if (b.Rarity == m_rarity)
            {
                m_background.sprite = b.Sprite;
                break;
            }
        }

        //�J�[�h�^�C�v�̐ݒ�
        List<Sprite> sprites = new List<Sprite>();
        foreach (var myType in m_cardType)
        {
            foreach (var t in typeSprites)
            {
                if (myType == t.CardType)
                {
                    sprites.Add(t.Sprite);
                    continue;
                }
            }
        }
        for (int i = 0; i < m_cardTypeImages.Count; i++)
        {
            if (sprites.Count <= i)
            {
                m_cardTypeImages[i].color = Color.clear;
            }
            else
            {
                m_cardTypeImages[i].sprite = sprites[i];
            }
        }
    }

    /// <summary>
    /// �J�[�h���ʂ̎��s
    /// </summary>
    /// <param name="target"></param>
    private void Execute(IDrop target)
    {
        UseType ut = target.GetUseType();
        //�g�p�Ώۂ��Ⴏ��Ύg���Ȃ�
        if (ut != m_useType)
            return;
        //�R�X�g�s���Ȃ�g���Ȃ�
        if (m_player.CurrentCost < m_cost)
        {
            Debug.Log("�R�X�g����Ȃ�");
            return;
        }
        //���ʂ𑗂�
        List<Command> cmds = new List<Command>();
        m_cardCommands.ForEach(c => cmds.Add(c));
        target.GetDrop(ref cmds);
        m_cardExecute.OnNext(cmds);
        //�v���C���[�̃R�X�g�����炷
        m_player.CurrentCost -= m_cost;
    }

    /// <summary>
    /// �v���C���[�̃G�t�F�N�g��]�����J�[�h���ʂ𑝌���������A�e�L�X�g���X�V������
    /// </summary>
    public void GetPlayerEffect()
    {
        string s = m_tooltip;
        m_cardCommands = m_database.CardCommands.Execute();
        //�U���͂̌��ʒu������
        MatchCollection matchs = Regex.Matches(s, "{dmg([0-9]*)}");
        foreach (Match m in matchs)
        {
            int index = int.Parse(m.Groups[1].Value);
            if (m_player != null || CardState == CardState.Play)
            {
                List<ConditionalParametor> cps = new List<ConditionalParametor>();
                ConditionalParametor cp = new ConditionalParametor();
                cp.Setup(m_cardCommands[index].Power, EvaluationParamType.Attack, EffectTiming.Attacked);
                cps.Add(cp);
                Command c = m_cardCommands[index];
                c.Power = m_player.EffectExecute(cps).Power; //�v���C���[�̃G�t�F�N�g��]��
                m_cardCommands[index] = c;
            }
            s = s.Replace(m.Value, $"{m_cardCommands[index].Power}�_���[�W");
        }
        //�u���b�N�l�̌��ʒu������
        matchs = Regex.Matches(s, "{blk([0-9]*)}");
        foreach (Match m in matchs)
        {
            int index = int.Parse(m.Groups[1].Value);
            if (m_player != null || CardState == CardState.Play)
            {
                List<ConditionalParametor> cps = new List<ConditionalParametor>();
                ConditionalParametor cp = new ConditionalParametor();
                cp.Setup(m_cardCommands[index].Block, EvaluationParamType.Block, EffectTiming.Attacked);
                cps.Add(cp);
                Command c = m_cardCommands[index];
                c.Block = m_player.EffectExecute(cps).Block; //�v���C���[�̃G�t�F�N�g��]��
                m_cardCommands[index] = c;
            }
            s = s.Replace(m.Value, $"{m_cardCommands[index].Block}�u���b�N");
        }
        m_tooltipText.text = s;
    }

    //�ȉ��C���^�[�t�F�[�X

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!m_isDrag && CardState == CardState.Play)
            m_defPos = m_rectTransform.anchoredPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CardState == CardState.Play)
        {
            m_isDrag = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CardState == CardState.Play)
        {
            m_rectTransform.anchoredPosition = new Vector2(eventData.position.x, eventData.position.y - 150);//�J�[�\�����J�[�h�����ɗ���悤�ɒ���
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (CardState == CardState.Play)
        {
            m_rectTransform.anchoredPosition = m_defPos;
            m_isDrag = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CardState == CardState.Button)
        {
            m_mouseClickPos = eventData.position;
            m_isDrag = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (CardState == CardState.Button)
        {
            //�X�N���[�����N���b�N���󂯕t�����Ⴄ���炻�̑΍�
            float dist = Vector2.Distance(m_mouseClickPos, eventData.position);
            if (dist < m_releaseDistance)
            {
                m_onClick.OnNext(Unit.Default);
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        m_isDrag = false;
        //���ʂ�OnDrop�Ŏ��ƃJ�[�h�̕`�悪�Ώۂ̗��ɍs�����Ⴄ�̂�
        //�}�E�X�̈ʒu��Ray���΂��ăC���^�[�t�F�[�X�Ŕ��f����
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
/// <summary>�J�[�h�̐U�镑��</summary>
public enum CardState
{
    /// <summary>���̐U�镑���������Ȃ�</summary>
    None,
    /// <summary>�h���b�O&�h���b�v�Ŏg�p���邱�Ƃ��o����</summary>
    Play,
    /// <summary>�N���b�N���C�x���g�̂�</summary>
    Button,
}