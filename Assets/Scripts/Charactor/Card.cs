using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using DG.Tweening;
using System.Text.RegularExpressions;

/// <summary>
/// �J�[�h�̎���
/// </summary>
public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    #region Field
    [SerializeField] Image m_background;
    [SerializeField] Text m_nameText;
    [SerializeField] Image m_icon;
    [SerializeField] Text m_tooltipText;
    [SerializeField] Text m_costText;
    [SerializeField] RectTransform m_rectTransform;
    [SerializeField] CardDescriptionInfomation m_cardDescriptionInfomation;
    [SerializeField] float m_releaseDistance;
    [SerializeField] float m_moveYDist;
    [SerializeField] float m_endDragMoveDuration;
    private int m_cost;
    private string m_tooltip;
    /// <summary>�p���J�[�h�t���O</summary>
    private bool m_isDispose;
    /// <summary>�󔖃J�[�h�t���O</summary>
    private bool m_isEthereal;
    /// <summary>�X�g�b�N�J�[�h�t���O</summary>
    private bool m_isStock;
    /// <summary>����J�[�h�t���O</summary>
    private bool m_isRelease;
    /// <summary>�h���b�O���t���O</summary>
    private bool m_isDrag;
    /// <summary>�A�j���[�V�������t���O</summary>
    private bool m_isAnim;
    /// <summary>�w�i�摜�̐F</summary>
    private Color m_backgroundColor;
    /// <summary>�A�C�R���摜�̐F</summary>
    private Color m_iconColor;
    /// <summary>���O�e�L�X�g�̐F</summary>
    private Color m_nameTextColor;
    /// <summary>�c�[���`�b�v�e�L�X�g�̐F</summary>
    private Color m_tooltipColor;
    /// <summary>�R�X�g�e�L�X�g�̐F</summary>
    private Color m_costColor;
    /// <summary>�J�[�h�^�C�v�̐F</summary>
    private List<Color> m_cardTypeImagesColor = new List<Color>();
    /// <summary>�g�p�Ώ�</summary>
    private UseType m_useType;
    /// <summary>���A�x</summary>
    private Rarity m_rarity;
    /// <summary>�J�[�h�̎��</summary>
    private List<CardType> m_cardType;
    /// <summary>���̃J�[�h�̃f�[�^</summary>
    private CardDataBase m_database;
    /// <summary>�g�p���ɋN���鎖</summary>
    private List<Command> m_cardCommands;
    /// <summary>���ʎg�p��ɃG�t�F�N�g��]������ۂɎg��</summary>
    private List<ConditionalParametor> m_commandUsedConditionalParametors = new List<ConditionalParametor>();
    /// <summary>�f�t�H���g�ʒu</summary>
    private Vector2 m_defPos;
    /// <summary>�}�E�X�N���b�N�ʒu�̕ۑ��p</summary>
    private Vector2 m_mouseClickPos;
    /// <summary>�\������J�[�h�ڍ׍���</summary>
    private CardDescriptionItem m_cardDescriptionItem;
    private Player m_player;
    private Subject<Unit> m_onClick = new Subject<Unit>();
    private Subject<UseType> m_onBeginDrag = new Subject<UseType>();
    private Subject<CommandsInfomation> m_cardExecute = new Subject<CommandsInfomation>();
    #endregion
    #region Property
    /// <summary>���O</summary>
    public string Name { get; private set; }
    /// <summary>�{�^���Ƃ��ĕ\�����ꂽ�ۂɉ��Ԗڂɐ������ꂽ�����L�����Ă����p</summary>
    public int Index { get; set; }
    /// <summary>���</summary>
    public CardState CardState { get; set; }
    /// <summary>�{�^���Ƃ��ĕ\�����ꂽ�ۂ̃N���b�N���C�x���g</summary>
    public System.IObservable<Unit> OnClickSubject => m_onClick;
    /// <summary>�h���b�O���J�n���ꂽ���Ƃ�ʒm����</summary>
    public System.IObservable<UseType> OnBeginDragSubject => m_onBeginDrag;
    /// <summary>�g�p���ꂽ����ʒm����</summary>
    public System.IObservable<CommandsInfomation> CardExecute => m_cardExecute;
    #endregion

    public void Setup(CardDataBase dataBase, List<CardData.RaritySprite> raritySprite, Player player)
    {
        m_player = player;
        SetBaseData(dataBase);
        SetSprites(raritySprite);
        GetPlayerEffect();
    }

    public void Init()
    {
        //m_cardCommands = m_database.CardCommands.Execute();
        TranslucentUI(false);
        m_isDrag = false;
        m_isAnim = false;
        m_cardDescriptionInfomation.SetActive = false;
    }

    /// <summary>�^����ꂽ�J�[�h�f�[�^�����g�ɐݒ肷��</summary>
    /// <param name="database"></param>
    private void SetBaseData(CardDataBase database)
    {
        m_database = database;
        Name = database.Name;
        m_backgroundColor = m_background.color;
        m_nameText.text = database.Name;
        m_nameTextColor = m_nameText.color;
        m_icon.sprite = database.Icon;
        m_iconColor = m_icon.color;
        m_tooltip = database.Tooltip;
        m_tooltipColor = m_tooltipText.color;
        m_isDispose = database.Dispose;
        m_isEthereal = database.Ethereal;
        m_costText.text = database.Cost;
        m_costColor = m_costText.color;
        m_rarity = database.Rarity;
        //����R�X�gx�Ƃ���肽���̂ŕ�����Ŏ���悤�ɂ��Ƃ�
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
        m_cardDescriptionInfomation.SetActive = false;
        //�X�g�b�N�A����̃t���O�`�F�b�N
        foreach (var c in m_cardCommands)
        {
            if (m_isStock && m_isRelease)
                break;
            if (c.StockTurn >= 0)
                m_isStock = true;
            if (c.IsStockRelease)
                m_isRelease = true;
        }
        SetDescriptionInfo(database.CardDescription.CardDescriptionItem);
    }

    /// <summary>
    /// �w�i�摜�ƃA�C�R�����̉摜�̐ݒ�
    /// </summary>
    /// <param name="backgroundSprites"></param>
    /// <param name="typeSprites"></param>
    private void SetSprites(List<CardData.RaritySprite> backgroundSprites)
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
        //List<Sprite> sprites = new List<Sprite>();
        //foreach (var myType in m_cardType)
        //{
        //    foreach (var t in typeSprites)
        //    {
        //        if (myType == t.CardType)
        //        {
        //            sprites.Add(t.Sprite);
        //            continue;
        //        }
        //    }
        //}
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
        CommandsInfomation info = new CommandsInfomation();
        info.Setup(cmds, m_icon.sprite, m_tooltipText.text);
        m_cardExecute.OnNext(info);
        m_player.EffectExecute(m_commandUsedConditionalParametors);
        //�ꕔ�ϐ��̏�����
        Init();
        //�v���C���[�̃R�X�g�����炷
        m_player.CurrentCost -= m_cost;
        if (m_isDispose)
            Destroy(gameObject);
    }

    /// <summary>
    /// �v���C���[�̃G�t�F�N�g��]�����J�[�h���ʂ𑝌���������A�e�L�X�g���X�V������
    /// </summary>
    public void GetPlayerEffect()
    {
        string s = m_tooltip;
        m_cardCommands = m_database.CardCommands.Execute();
        List<ConditionalParametor> commandUsedConditionalParametors = new List<ConditionalParametor>();
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
                if (m_commandUsedConditionalParametors.Count > 0)
                {
                    cp = new ConditionalParametor();
                    cp.Setup(EvaluationParamType.Attack, EffectTiming.Attacked, true);
                    commandUsedConditionalParametors.Add(cp);
                }
            }
            s = s.Replace(m.Value, m_cardCommands[index].Power.ToString());
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
                if (m_commandUsedConditionalParametors.Count > 0)
                {
                    cp = new ConditionalParametor();
                    cp.Setup(EvaluationParamType.Block, EffectTiming.Attacked, true);
                    commandUsedConditionalParametors.Add(cp);
                }
            }
            s = s.Replace(m.Value, m_cardCommands[index].Block.ToString());
        }
        m_commandUsedConditionalParametors = commandUsedConditionalParametors;
        m_tooltipText.text = s;
        m_cardDescriptionInfomation.Setup(m_cardDescriptionItem, m_cardCommands);
    }

    private void SetDescriptionInfo(CardDescriptionItem cardDescriptionItem)
    {
        m_cardDescriptionItem.Setup(m_isDispose, m_isEthereal, m_isStock, m_isRelease);
        if (cardDescriptionItem.IsDispose)
            m_cardDescriptionItem.IsDispose = true;
        if (cardDescriptionItem.IsEthereal)
            m_cardDescriptionItem.IsEthereal = true;
        if (cardDescriptionItem.IsStock)
            m_cardDescriptionItem.IsStock = true;
        if (cardDescriptionItem.IsRelease)
            m_cardDescriptionItem.IsRelease = true;

    }

    /// <summary>
    /// �J�[�h�𔼓����ɂ����肵�Ȃ�������
    /// </summary>
    /// <param name="isTranslucent"></param>
    private void TranslucentUI(bool isTranslucent = true)
    {
        if (isTranslucent)
        {
            m_background.color = m_backgroundColor - new Color(0, 0, 0, m_backgroundColor.a / 2);
            m_costText.color = m_costColor - new Color(0, 0, 0, m_costColor.a / 2);
            m_icon.color = m_iconColor - new Color(0, 0, 0, m_iconColor.a / 2);
            m_nameText.color = m_nameTextColor - new Color(0, 0, 0, m_nameTextColor.a / 2);
            m_tooltipText.color = m_tooltipColor - new Color(0, 0, 0, m_tooltipColor.a / 2);
        }
        else
        {
            m_background.color = m_backgroundColor; 
            m_costText.color = m_costColor;
            m_icon.color = m_iconColor;
            m_nameText.color = m_nameTextColor;
            m_tooltipText.color = m_tooltipColor;
        }
    }

    public void SetParent(Transform parent, bool isUsed)
    {
        if (!isUsed && m_isEthereal)
            Destroy(gameObject);
        Init();
        transform.SetParent(parent, false);
    }

    //�ȉ��C���^�[�t�F�[�X

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_cardDescriptionInfomation.SetActive = true;
        if (!m_isDrag && CardState == CardState.Play)
        {
            if (!m_isAnim)
                m_defPos = m_rectTransform.anchoredPosition;
            m_isAnim = true;
            //�J�[�\�����ڐG�����炿����Ə�ɓ�����
            m_rectTransform.DOAnchorPos(new Vector2(m_rectTransform.anchoredPosition.x, m_rectTransform.anchoredPosition.y + m_moveYDist), m_endDragMoveDuration)
                .OnComplete(() => m_isAnim = false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_cardDescriptionInfomation.SetActive = false;
        if (!m_isDrag && CardState == CardState.Play)
        {
            m_isAnim = true;
            //�J�[�\�������ꂽ�猳�̈ʒu�ɖ߂�
            m_rectTransform.DOAnchorPos(m_defPos, m_endDragMoveDuration)
                .OnComplete(() => m_isAnim = false);

        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CardState == CardState.Play)
        {
            m_isDrag = true;
            m_onBeginDrag.OnNext(m_useType);
            TranslucentUI();
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
            m_rectTransform.DOAnchorPos(m_defPos, m_endDragMoveDuration)
                .OnComplete(() => m_isDrag = false);
            TranslucentUI(false);
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