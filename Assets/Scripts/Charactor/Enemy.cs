using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Linq;

public class Enemy : Charactor, IDrop
{
    [SerializeField] PlanDisplay m_planDisplayPrefab;
    [SerializeField] Transform m_planDisplayParent;
    private int m_index;
    private bool m_isDispose;
    private EnemyDataBase m_dataBase;
    private EnemyID m_enemyID;
    /// <summary>���̃^�[���̍s��</summary>
    private List<Command> m_currentTurnCommand = new List<Command>();
    private Subject<CommandsInfomation> m_action = new Subject<CommandsInfomation>();
    /// <summary>�G�O���[�v�ł̏���index</summary>
    public int Index { set => m_index = value; }
    /// <summary>�G���S���ɏ��łɂ����鎞��</summary>
    public float DeadDuration { private get; set; }
    /// <summary>�s���I���t���O</summary>
    public bool EndAction { get; set; }
    /// <summary>���̓G��ID</summary>
    public EnemyID EnemyID => m_enemyID;
    /// <summary>�G�s�����ɔ��s�����C�x���g</summary>
    public System.IObservable<CommandsInfomation> ActionSubject => m_action;

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
    /// �G���s��������
    /// </summary>
    /// <returns></returns>
    private async UniTask Action()
    {
        CommandsInfomation ci = new CommandsInfomation();
        ci.Setup(m_currentTurnCommand, m_image.sprite, "");
        m_action.OnNext(ci);
        await UniTask.Yield();
    }

    /// <summary>
    /// ���̃^�[���̍s����I��
    /// </summary>
    private void SelectActionCommand()
    {
        m_currentTurnCommand = m_dataBase.Action(new Field(), null, this); //�Ƃ肠���������

        //�G�t�F�N�g�̕]��
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
    /// �s���\���\��������
    /// </summary>
    /// <param name="commands"></param>
    private void SetPlan(List<Command> commands)
    {
        for (int i = m_planDisplayParent.childCount - 1; i >= 0; i--)
        {
            Destroy(m_planDisplayParent.GetChild(i).gameObject);
        }

        //PlanData�ɒl��������̂��̂�e��
        List<PlanData> pds = new List<PlanData>();
        foreach (var cmd in commands)
        {
            PlanData pd = new PlanData();
            pd.PlanType = cmd.PlanType;
            switch (cmd.CommandType)
            {
                case CommandType.Attack:
                    pd.Text = cmd.Power.ToString();
                    break;
                case CommandType.Block:
                    pd.Text = cmd.Block.ToString();
                    break;
                default:
                    pd.Text = "";
                    break;
            }

            //���ɍU���h��ȊO�̓���̍s���\�肪����Βǉ����Ȃ�
            bool flag = false;
            if (pd.PlanType == PlanType.Attack || pd.PlanType == PlanType.Block) { }
            else
            {
                foreach (var p in pds)
                {
                    if (p.PlanType == pd.PlanType)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            
            if (!flag)
                pds.Add(pd);
        }

        pds.ForEach(p =>
        {
            PlanDisplay planDisplay = Instantiate(m_planDisplayPrefab);
            planDisplay.Setup(p);
            planDisplay.transform.SetParent(m_planDisplayParent, false);
        });
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

    //�ȉ��C���^�[�t�F�[�X

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
