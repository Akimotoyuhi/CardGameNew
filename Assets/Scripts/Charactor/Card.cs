using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using System.Text.RegularExpressions;

/// <summary>
/// カードの実体
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
    private int m_cost;
    private string m_tooltip;
    private bool m_isDrag;
    private Sprite m_backgroundSprite;
    private UseType m_useType;
    private Rarity m_rarity;
    private List<CardType> m_cardType;
    private CardDataBase m_database;
    private List<Command> m_cardCommands;
    private Vector2 m_defPos;
    private Player m_player;
    private System.Action m_clickEvent;
    private Subject<Unit> m_onClick = new Subject<Unit>();
    private Subject<List<Command>> m_cardExecute = new Subject<List<Command>>();
    /// <summary>名前</summary>
    public string Name { get; private set; }
    /// <summary>状態</summary>
    public CardState CardState { get; set; }
    /// <summary>使用された事を通知する</summary>
    public System.IObservable<List<Command>> CardExecute => m_cardExecute;

    public void Setup(CardDataBase dataBase, List<CardData.RaritySprite> raritySprite, List<CardData.TypeSprite> typeSprite, Player player)
    {
        m_player = player;
        SetBaseData(dataBase);
        SetSprites(raritySprite, typeSprite);
        GetPlayerEffect();
    }

    public void Setup(CardDataBase dataBase, List<CardData.RaritySprite> raritySprite, List<CardData.TypeSprite> typeSprite, System.Action onClick)
    {
        m_clickEvent = onClick;
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
        m_nameText.text = database.Name;
        m_icon.sprite = database.Icon;
        m_tooltip = database.Tooltip;
        m_costText.text = database.Cost;
        m_rarity = database.Rarity;
        m_cardType = database.CardType;
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
            }
            s = s.Replace(m.Value, $"{m_cardCommands[index].Power}ダメージ");
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
            }
            s = s.Replace(m.Value, $"{m_cardCommands[index].Block}ブロック");
        }
        m_tooltipText.text = s;
    }

    //以下インターフェース

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
            m_rectTransform.anchoredPosition = new Vector2(eventData.position.x, eventData.position.y - 150);//カーソルがカード中央に来るように調整
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
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_clickEvent != null)
        {
            m_clickEvent();
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