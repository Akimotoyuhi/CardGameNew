using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using DG.Tweening;
using System.Text.RegularExpressions;

/// <summary>
/// カードの実体
/// </summary>
public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    #region Field
    [SerializeField] Image m_background;
    [SerializeField] Text m_nameText;
    [SerializeField] Image m_icon;
    [SerializeField] Text m_tooltipText;
    [SerializeField] Text m_costText;
    [SerializeField] List<Image> m_cardTypeImages;
    [SerializeField] RectTransform m_rectTransform;
    [SerializeField] float m_releaseDistance;
    [SerializeField] float m_moveYDist;
    [SerializeField] float m_endDragMoveDuration;
    private int m_cost;
    private string m_tooltip;
    /// <summary>ドラッグ中フラグ</summary>
    private bool m_isDrag;
    /// <summary>アニメーション中フラグ</summary>
    private bool m_isAnim;
    /// <summary>背景画像の色</summary>
    private Color m_backgroundColor;
    /// <summary>アイコン画像の色</summary>
    private Color m_iconColor;
    /// <summary>名前テキストの色</summary>
    private Color m_nameTextColor;
    /// <summary>ツールチップテキストの色</summary>
    private Color m_tooltipColor;
    /// <summary>コストテキストの色</summary>
    private Color m_costColor;
    /// <summary>カードタイプの色</summary>
    private List<Color> m_cardTypeImagesColor = new List<Color>();
    /// <summary>使用対象</summary>
    private UseType m_useType;
    /// <summary>レア度</summary>
    private Rarity m_rarity;
    /// <summary>カードの種類</summary>
    private List<CardType> m_cardType;
    /// <summary>このカードのデータ</summary>
    private CardDataBase m_database;
    /// <summary>使用時に起こる事</summary>
    private List<Command> m_cardCommands;
    /// <summary>効果使用後にエフェクトを評価する際に使う</summary>
    private List<ConditionalParametor> m_commandUsedConditionalParametors = new List<ConditionalParametor>();
    /// <summary>デフォルト位置</summary>
    private Vector2 m_defPos;
    /// <summary>マウスクリック位置の保存用</summary>
    private Vector2 m_mouseClickPos;
    private Player m_player;
    private Subject<Unit> m_onClick = new Subject<Unit>();
    private Subject<List<Command>> m_cardExecute = new Subject<List<Command>>();
    #endregion
    #region Property
    /// <summary>名前</summary>
    public string Name { get; private set; }
    /// <summary>ボタンとして表示された際に何番目に生成されたかを記憶しておく用</summary>
    public int Index { get; set; }
    /// <summary>状態</summary>
    public CardState CardState { get; set; }
    /// <summary>ボタンとして表示された際のクリック時イベント</summary>
    public System.IObservable<Unit> OnClickSubject => m_onClick;
    /// <summary>使用された事を通知する</summary>
    public System.IObservable<List<Command>> CardExecute => m_cardExecute;
    #endregion

    public void Setup(CardDataBase dataBase, List<CardData.RaritySprite> raritySprite, List<CardData.TypeSprite> typeSprite, Player player)
    {
        m_player = player;
        SetBaseData(dataBase);
        SetSprites(raritySprite, typeSprite);
        GetPlayerEffect();
    }

    public void Init()
    {
        m_cardCommands = m_database.CardCommands.Execute();
    }

    /// <summary>与えられたカードデータを自身に設定する</summary>
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
        m_costText.text = database.Cost;
        m_costColor = m_costText.color;
        m_rarity = database.Rarity;
        m_cardType = database.CardType;
        for (int i = 0; i < m_cardTypeImages.Count; i++)
        {
            m_cardTypeImagesColor.Add(m_cardTypeImages[i].color);
        }
        //今後コストxとか作りたいので文字列で取れるようにしてある
        try
        {
            m_cost = int.Parse(database.Cost);
        }
        catch
        {
            Debug.Log("キャスト不可な値が検出されたので、適当な値が設定されました");
            m_cost = m_player.MaxCost;
        }
        m_useType = database.CardUseType;
        m_cardCommands = database.CardCommands.Execute();
    }

    /// <summary>
    /// 背景画像とアイコン横の画像の設定
    /// </summary>
    /// <param name="backgroundSprites"></param>
    /// <param name="typeSprites"></param>
    private void SetSprites(List<CardData.RaritySprite> backgroundSprites, List<CardData.TypeSprite> typeSprites)
    {
        //背景画像の設定
        foreach (var b in backgroundSprites)
        {
            if (b.Rarity == m_rarity)
            {
                m_background.sprite = b.Sprite;
                break;
            }
        }

        //カードタイプの設定
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
    /// カード効果の実行
    /// </summary>
    /// <param name="target"></param>
    private void Execute(IDrop target)
    {
        UseType ut = target.GetUseType();
        //使用対象じゃければ使えない
        if (ut != m_useType)
            return;
        //コスト不足なら使えない
        if (m_player.CurrentCost < m_cost)
        {
            Debug.Log("コスト足りない");
            return;
        }
        //効果を送る
        List<Command> cmds = new List<Command>();
        m_cardCommands.ForEach(c => cmds.Add(c));
        target.GetDrop(ref cmds);
        m_cardExecute.OnNext(cmds);
        m_player.EffectExecute(m_commandUsedConditionalParametors);
        //一部変数の初期化
        TranslucentUI();
        m_isDrag = false;
        m_isAnim = false;
        //プレイヤーのコストを減らす
        m_player.CurrentCost -= m_cost;
    }

    /// <summary>
    /// プレイヤーのエフェクトを評価しカード効果を増減させた後、テキストを更新させる
    /// </summary>
    public void GetPlayerEffect()
    {
        string s = m_tooltip;
        m_cardCommands = m_database.CardCommands.Execute();
        List<ConditionalParametor> commandUsedConditionalParametors = new List<ConditionalParametor>();
        //攻撃力の効果置き換え
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
                c.Power = m_player.EffectExecute(cps).Power; //プレイヤーのエフェクトを評価
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
        //ブロック値の効果置き換え
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
                c.Block = m_player.EffectExecute(cps).Block; //プレイヤーのエフェクトを評価
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
    }

    /// <summary>
    /// カードを半透明にしたりしなかったり
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
            for (int i = 0; i < m_cardTypeImages.Count; i++)
            {
                m_cardTypeImages[i].color = m_cardTypeImagesColor[i] - new Color(0, 0, 0, m_cardTypeImagesColor[i].a / 2);
            }
        }
        else
        {
            m_background.color = m_backgroundColor; 
            m_costText.color = m_costColor;
            m_icon.color = m_iconColor;
            m_nameText.color = m_nameTextColor;
            m_tooltipText.color = m_tooltipColor;
            for (int i = 0; i < m_cardTypeImages.Count; i++)
            {
                m_cardTypeImages[i].color = m_cardTypeImagesColor[i];
            }
        }
    }


    //以下インターフェース

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!m_isDrag && CardState == CardState.Play)
        {
            if (!m_isAnim)
                m_defPos = m_rectTransform.anchoredPosition;
            m_isAnim = true;
            //カーソルが接触したらちょっと上に動かす
            m_rectTransform.DOAnchorPos(new Vector2(m_rectTransform.anchoredPosition.x, m_rectTransform.anchoredPosition.y + m_moveYDist), m_endDragMoveDuration)
                .OnComplete(() => m_isAnim = false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!m_isDrag && CardState == CardState.Play)
        {
            m_isAnim = true;
            //カーソルが離れたら元の位置に戻す
            m_rectTransform.DOAnchorPos(m_defPos, m_endDragMoveDuration)
                .OnComplete(() => m_isAnim = false);

        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CardState == CardState.Play)
        {
            m_isDrag = true;
            TranslucentUI();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CardState == CardState.Play)
        {
            m_rectTransform.anchoredPosition = new Vector2(eventData.position.x, eventData.position.y - 150);//カーソルがカード中央に来るように調整
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
            //スクロール中クリックを受け付けちゃうからその対策
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
        //普通にOnDropで取るとカードの描画が対象の裏に行っちゃうので
        //マウスの位置にRayを飛ばしてインターフェースで判断する
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
/// <summary>カードの振る舞い</summary>
public enum CardState
{
    /// <summary>何の振る舞いも持たない</summary>
    None,
    /// <summary>ドラッグ&ドロップで使用することが出来る</summary>
    Play,
    /// <summary>クリック時イベントのみ</summary>
    Button,
}