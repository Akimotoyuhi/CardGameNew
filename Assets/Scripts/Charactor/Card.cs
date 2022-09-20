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
    [SerializeField] RectTransform m_rectTransform;
    [SerializeField] CardDescriptionInfomation m_cardDescriptionInfomation;
    [SerializeField] float m_releaseDistance;
    [SerializeField] float m_moveYDist;
    [SerializeField] float m_endDragMoveDuration;
    private int m_cost;
    private string m_tooltip;
    /// <summary>廃棄カードフラグ</summary>
    private bool m_isDispose;
    /// <summary>希薄カードフラグ</summary>
    private bool m_isEthereal;
    /// <summary>ストックカードフラグ</summary>
    private bool m_isStock;
    /// <summary>解放カードフラグ</summary>
    private bool m_isRelease;
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
    /// <summary>表示するカード詳細項目</summary>
    private CardDescriptionItem m_cardDescriptionItem;
    private Player m_player;
    private Subject<Unit> m_onClick = new Subject<Unit>();
    private Subject<UseType> m_onBeginDrag = new Subject<UseType>();
    private Subject<Unit> m_onEndDrag = new Subject<Unit>();
    private Subject<CommandsInfomation> m_cardExecute = new Subject<CommandsInfomation>();
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
    /// <summary>ドラッグが開始されたことを通知する</summary>
    public System.IObservable<UseType> OnBeginDragSubject => m_onBeginDrag;
    /// <summary>ドラッグ終了の通知</summary>
    public System.IObservable<Unit> OnEndDragSubject => m_onEndDrag;
    /// <summary>効果の発動を通知する</summary>
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
        m_isDispose = database.Dispose;
        m_isEthereal = database.Ethereal;
        m_costText.text = database.Cost;
        m_costColor = m_costText.color;
        m_rarity = database.Rarity;
        //今後コストxとか作りたいので文字列で取れるようにしとく
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
        m_cardDescriptionInfomation.SetActive = false;
        //ストック、解放のフラグチェック
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
    /// 背景画像とアイコン横の画像の設定
    /// </summary>
    /// <param name="backgroundSprites"></param>
    /// <param name="typeSprites"></param>
    private void SetSprites(List<CardData.RaritySprite> backgroundSprites)
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
        CommandsInfomation info = new CommandsInfomation();
        info.Setup(cmds, m_icon.sprite, m_tooltipText.text);
        m_cardExecute.OnNext(info);
        m_player.EffectExecute(m_commandUsedConditionalParametors);
        //一部変数の初期化
        Init();
        //プレイヤーのコストを減らす
        m_player.CurrentCost -= m_cost;
        if (m_isDispose)
            Destroy(gameObject);
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

    //以下インターフェース

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_cardDescriptionInfomation.SetActive = true;
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
        m_cardDescriptionInfomation.SetActive = false;
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
            m_onBeginDrag.OnNext(m_useType);
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