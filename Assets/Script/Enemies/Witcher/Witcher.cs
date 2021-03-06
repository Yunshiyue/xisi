using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MovementWitcher))]
[RequireComponent(typeof(WitcherAttack))]
[RequireComponent(typeof(DefenceEnemies))]
public class Witcher : Enemies 
{
    //完成任务的对象，任务内容为杀死一只巫师
    public GameObject NPC;
    private TalkUI talker;


    private MovementWitcher movementComponent;
    private WitcherAttack attackComponent;

    GameObject player;

    private float actionCD = 0f;
    private bool canFire = true;
    private bool canBlink = false;
    private bool canBack = true;
    public override void Initialize()
    {
        if (NPC != null)//如果有NPC布置了击杀巫师的任务
        {
            //完成任务需求的对象
            talker = NPC.GetComponent<TalkUI>();
        }

        priorityInType = 1;

        player = GameObject.Find("Player");

        movementComponent = GetComponent<MovementWitcher>();
        if (movementComponent == null)
        {
            Debug.LogError("在Witcher中，没有找到Movement脚本！");
        }
        attackComponent = GetComponent<WitcherAttack>();
        if (attackComponent == null)
        {
            Debug.LogError("在Witcher中，没有找到Attack脚本！");
        }

        defenceComponent = GetComponent<DefenceEnemies>();
        if (defenceComponent == null)
        {
            Debug.LogError("在Witcher中，没有找到Defence脚本！");
        }
        //设置最大生命值
        defenceComponent.Initialize(3);
    }

    public override void MyUpdate()
    {
        //为了测试暂关闭防御组件
        DefenceCheck();
        
        //MoveControl();
    }
    
    //移动流程
    //private void MoveControl()
    //{   
    //    //没看到player时在初始位置来回移动
    //    movementComponent.RequestMoveByFrame(WitcherMovement.MovementMode.Normal);
    //    movementComponent.setIsGravity(true);

    //    if (actionCD > 0)
    //    {
    //        actionCD -= Time.deltaTime;
    //    }
    //    if (movementComponent.getIsSeePlayer())//看到player后进入攻击状态
    //    {     
    //        //Debug.Log("blink!");
    //        //movementComponent.RequestMoveByFrame(WitcherMovemet.MovementMode.Ability);
    //        //isSeePlayer = false;
    //        //attackComponent.PushFire();

    //        //改变移动范围 
    //        movementComponent.ChangeRange(player.transform.position.x + 1, player.transform.position.x - 1);
    //        AttackControl();
    //    }
    //    else
    //    {
    //        if (canBack)
    //        { 
    //            Invoke("RangeMoveBack", 2f);
    //            canBack = false;
    //        }
           
    //    }
    //}

    private void DefenceCheck()
    {
        defenceComponent.AttackCheck();
        if (defenceComponent.getIsDead())
        {
            gameObject.SetActive(false);
        }
        defenceComponent.Clear();
    }

    private void RangeMoveBack()
    {
        movementComponent.ChangeRange(movementComponent.GetOriginX() + 0.5f, movementComponent.GetOriginX() - 0.5f);
        canBack = true;
    }
    //攻击流程
    private void AttackControl()
    {
        //if (actionCD > 0)
        //{
        //    actionCD -= Time.deltaTime;
        //}
        if (actionCD <= 0) {
            if (canFire)
            {
                attackComponent.PushFire();
                movementComponent.setIsGravity(false);
                //canFire = false;
                //canBlink = true;
                actionCD = 1f;
            }
        }
         
        //else if (canBlink)
        //{
        //    movementComponent.RequestMoveByFrame(WitcherMovement.MovementMode.Ability);
        //    movementComponent.setIsGravity(true);
        //    canFire = true;
        //    canBlink = false;
        //    actionCD = 1.5f;
        //}
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }
    protected  override void OnDisable()//接受了杀死巫师的任务会调用这个方法
    {
        if (NPC != null)
        {
            talker.killCount++;
            if (talker.killCount >= 1)
            {
                talker.isFinishMission = true;
                talker.isEndDialog2 = true;
            }
            
        }
        
    }
    
}
