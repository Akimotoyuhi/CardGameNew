using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Linq;

public class Enemy : Charactor, IDrop
{
    //[SerializeField] 行動予定表示クラス
    private int m_index;
    private bool m_isDispose;
    private EnemyDataBase m_dataBase;
    private EnemyID m_enemyID;
    /// <summary>このターンの行動</summary>
    private List<Command> m_currentTurnCommand = new List<Command>();
    private Subject<List<Command>> m_action = new Subject<List<Command>>();
    /// <summary>敵グループでの所属index</summary>
    public int Index { set => m_index = value; }
    /// <summary>敵死亡時に消滅にかける時間</summary>
    public float DeadDuration { private get; set; }
    /// <summary>行動終了フラグ</summary>
    public bool EndAction { get; set; }
    /// <summary>この敵のID</summary>
    public EnemyID EnemyID => m_enemyID;
    /// <summary>敵行動時に発行されるイベント</summary>
    public System.IObservable<List<Command>> ActionSubject => m_action;

    protected override void Setup()
    {
        base.Setup();
    }

    public void Dispose()
    {
        m_isDispose = true;
    }

    public void SetBaseData(EnemyDataBase dataBase)
    {
        if (dataBase == null)
            return;
        m_dataBase = dataBase;
        m_image.sprite = dataBase.CharactorData.Sprite;
        m_maxLife = dataBase.CharactorData.MaxLife;
        m_currentLife.Value = dataBase.CharactorData.MaxLife;
        m_isDispose = false;
        m_image.gameObject.SetActive(true);
        m_image.color = Color.white;
        Setup();
    }

    public override async UniTask TurnBegin(int turn)
    {
        SelectActionCommand();
        await base.TurnBegin(turn);
    }

    public override async UniTask TurnEnd(int turn)
    {
        EndAction = false;
        await Action();
        await UniTask.WaitUntil(() => EndAction);
        await base.TurnEnd(turn);
    }

    /// <summary>
    /// 敵を行動させる
    /// </summary>
    /// <returns></returns>
    private async UniTask Action()
    {
        m_action.OnNext(m_currentTurnCommand);
        await UniTask.Yield();
    }

    /// <summary>
    /// このターンの行動を選ぶ
    /// </summary>
    private void SelectActionCommand()
    {
        m_currentTurnCommand = m_dataBase.Action(new Field(), null, this); //とりあえずこれで

        //エフェクトの評価
        m_currentTurnCommand.ForEach(cmd =>
        {
            List<ConditionalParametor> cps = new List<ConditionalParametor>();
            ConditionalParametor cp = new ConditionalParametor();
            switch (cmd.CommandType)
            {
                case CommandType.Attack:
                    cp.Parametor = cmd.Power;
                    cp.Setup(cmd.Power, EvaluationParamType.Attack, EffectTiming.Attacked);
                    cps.Add(cp);
                    break;
                case CommandType.Block:
                    cp.Parametor = cmd.Block;
                    cp.Setup(cmd.Block, EvaluationParamType.Block, EffectTiming.Attacked);
                    cps.Add(cp);
                    break;
                //case CommandType.Effect:
                //    break;
                default:
                    break;
            }
            EffectExecute(cps);
        });
        SetPlan(m_currentTurnCommand);
    }

    /// <summary>
    /// 行動予定を表示される
    /// </summary>
    /// <param name="commands"></param>
    private void SetPlan(List<Command> commands)
    {
        //行う行動のPlanTypeから同種のものを省く
        var list = commands.Select(cmd => cmd.PlanType).Distinct().ToList();
        //plansを行動予定を表示するクラスに渡す
    }

    protected override void Dead()
    {
        if (IsDead)
            return;
        m_isDead = true;
        m_image.DOColor(Color.clear, DeadDuration)
            .OnComplete(() =>
            {
                m_deadSubject.OnNext(Unit.Default);
                m_image.gameObject.SetActive(false);
            });
    }

    //以下インターフェース

    public bool GetDrop(ref List<Command> commands)
    {
        commands.ForEach(c =>
        {
            if (c.UseType == UseType.Enemy)
                c.TargetEnemyIndex = m_index;
        });
        return true;
    }

    public UseType GetUseType() => UseType.Enemy;
}
