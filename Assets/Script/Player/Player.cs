/**
 * @Description: 主角的控制脚本，继承自myUpdate类。拥有移动组件、攻击组件、防御组件等主要组件。
 *               负责输入控制和通知各组件控制信息，诸如移动、跳跃、释放技能等。
 * @Author: ridger

 * 

 * @Editor: ridger
 * @Edit: 下蹲修改为通过Request实现
 * 

 * @Eidtor: ridger
 * @Eidt: 增加了长按短按施法逻辑的控制，元素添加与归还的逻辑、施法中断、打断等逻辑。
 *        以及当主元素为风属性时，瞄准镜的控制逻辑。
 *        增加了当前主元素、辅元素等变量
 *        

 * @Eidtor: ridger
 * @Eidt: 将辅助技能first改为水盾以便测试
 * 

 * @Eidtor: ridger
 * @Eidt: 将辅助技能second改为残影闪回以便测试
 * 

 * @Editor: ridger
 * @Edit: 将Element枚举类型转到ElementAbilityManager中
 * 

 * @Editor: ridger
 * @Edit: 当主角死亡时场景重置，在movement脚本中实现
 * 
 * @Editor: CuteRed
 * @Edit: 为适应云游戏，改变了输入系统
 * 
 * @Editor: CuteRed
 * @Edit: 增加移动端支持
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;



[RequireComponent(typeof(MovementPlayer))]
//[RequireComponent(typeof(AttackPlayer))]
[RequireComponent(typeof(ElementAbilityManager))]
[RequireComponent(typeof(DefencePlayer))]
public class Player : myUpdate
{
    private MovementPlayer movementComponent;
    //private AttackPlayer attackComponent;
    private DefencePlayer defenceComponent;
    private ElementAbilityManager abilityManager;
    private InteractivePlayer interactivePlayer;
    private CanFight canFight;
    private string targetLayerName = "Enemy";

    private int priorityInType = 0;
    private UpdateType updateType = UpdateType.Player;
    //临时变量
    private Vector2 tempMovement = new Vector2(0, 0);
    private int lastHp;//记录上一帧的血量

    //心心数组
    private HPItem[] hpArray;

    //平台
    public PlatformJudge.Platfrom platform;

    //移动端需要的控件
    private Joystick joystick;
    /// <summary>
    /// 平台管理器
    /// </summary>
    private PlatformJudge platformJudge;
    /// <summary>
    /// 主元素按钮
    /// </summary>
    private MainElementButton aButton;
    private SupportingElementButton bButton;
    private SupportingElementButton cButton;
    /// <summary>
    /// 跳跃按钮
    /// </summary>
    private SimpleButton jumpButton;
    /// <summary>
    /// 交互按钮
    /// </summary>
    private SimpleButton interactButton;

    bool hasInitialPlatform = false;

    public override void Initialize()
    {
        //判断平台
        platformJudge = GameObject.Find("ControllerMode").GetComponent<PlatformJudge>();
        if (platformJudge == null)
        {
            Debug.LogError("找不到ControllerMode");
        }
        //platform = platformJudge.GetPlatform();
        Debug.Log(platform);
        
        
        canFight = GetComponent<CanFight>();
        if (canFight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到CanFight组件！");
        }

        //使用string数组初始化canFight能够检测到的层
        string[] targets = new string[1];
        targets[0] = targetLayerName;
        canFight.Initiailize(targets);


        movementComponent = GetComponent<MovementPlayer>();
        if(movementComponent == null)
        {
            Debug.LogError("在Player中，没有找到MovementPlayer脚本！");
        }
        abilityManager = GetComponent<ElementAbilityManager>();
        //attackComponent = GetComponent<AttackPlayer>();
        //if (attackComponent == null)
        //{
        //    Debug.LogError("在Player中，没有找到AttackPlayer脚本！");
        //}

        defenceComponent = GetComponent<DefencePlayer>();
        if(defenceComponent == null)
        {
            Debug.LogError("在Player中，没有找到DefencePlayer脚本！");
        }
        defenceComponent.Initialize(5);

        interactivePlayer = GetComponent<InteractivePlayer>();

        //初始化心心数
        GameObject HpPanel = GameObject.Find("HP Panel");

        hpArray = new HPItem[defenceComponent.getHpMax()];
        
        for (int i = 0; i < defenceComponent.getHp(); i++)
        {
            Transform hpItem = HpPanel.transform.GetChild(i);
            hpArray[i] = hpItem.GetComponent<HPItem>();
            hpArray[i].Getting();
        }

    }

    //根据设计，先进行受伤判断，再进行移动控制和技能控制
    public override void MyUpdate()
    {
        if(!hasInitialPlatform)
        {
            platform = platformJudge.GetPlatform();
            hasInitialPlatform = true;
            if (platform == PlatformJudge.Platfrom.ANDROID || platform == PlatformJudge.Platfrom.IOS || platform == PlatformJudge.Platfrom.WEB_MOBILE)
            {
                joystick = GameObject.Find("Variable Joystick").GetComponent<Joystick>();
                if (joystick == null)
                {
                    Debug.LogError("移动端获取摇杆失败！");
                }

                aButton = GameObject.Find("AButton").GetComponent<MainElementButton>();
                bButton = GameObject.Find("BButton").GetComponent<SupportingElementButton>();
                cButton = GameObject.Find("CButton").GetComponent<SupportingElementButton>();

                jumpButton = GameObject.Find("JumpButton").GetComponent<SimpleButton>();
                interactButton = GameObject.Find("InteractButton").GetComponent<SimpleButton>();
            }
        }
        MenuCheck();
        DefenceCheck();

        AbilityControl();
        
        abilityManager.SetCastDebugInfo();

        ChangeElementControl();

        MoveControl();
        InteractiveCheck();

    }

    private void AbilityControl()
    {
        if (platform == PlatformJudge.Platfrom.PC || platform == PlatformJudge.Platfrom.WEB_PC)
        {
            abilityManager.AbilityControl(Input.GetButton("MainElement"), Input.GetButtonDown("FirstOtherElement"), Input.GetButtonDown("SecondOtherElement"));
        }
        else
        {
            abilityManager.AbilityControl(aButton.IsTrigger(), bButton.IsTrigger(), cButton.IsTrigger());
        }
    }

    private void ChangeElementControl()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            abilityManager.NextMainElement();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            abilityManager.NextAElement();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            abilityManager.NextBElement();
        }

    }
    private void InteractiveCheck()
    {
        if (platform == PlatformJudge.Platfrom.PC || platform == PlatformJudge.Platfrom.WEB_PC)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                interactivePlayer.InteractiveWithClosetObject();
            }
        }
        else 
        { 
            if (interactButton.IsTrigger())
            {
                interactivePlayer.InteractiveWithClosetObject();
            }
        }

    }

    private void MenuCheck()
    {
        if (Input.GetButtonDown("ReloadScene"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    //防御组件检查，目前只统计信息，但并不对信息做什么处理
    private void DefenceCheck()
    {
        lastHp = defenceComponent.getHp();

        defenceComponent.AttackCheck();

        if (defenceComponent.getHpReduction() > 0)
        {
            ReductionChangeHpUI();
        }

        lastHp = defenceComponent.getHp();
        if (defenceComponent.getRecoverdHp() > 0)
        {
            RecoverChangeHpUI();
        }

        defenceComponent.Clear();
    }
    //掉血改变UI
    public void ReductionChangeHpUI()
    {
        for (int i = 0; i < defenceComponent.getHpReduction(); i++)
        {
            int index = lastHp - i-1;
            //Debug.Log("lost-- HP:" + defenceComponent.getHp() + ";index:" + index);
            hpArray[index].Lost();
        }
    }
    //回血改变UI
    public void RecoverChangeHpUI()
    {
        for (int i = 0; i < defenceComponent.getRecoverdHp(); i++)
        {
            int index = lastHp - i-1;
            //Debug.Log("recover-- HP:" + defenceComponent.getHp() + ";index:" + index);
            hpArray[index].Recover();
        }
    }
    //移动控制，包括X轴移动、跳跃、下蹲；通过向移动组件“请求”实现。
    private void MoveControl()
    {
        Debug.Log("111");
        if (platform == PlatformJudge.Platfrom.PC || platform == PlatformJudge.Platfrom.WEB_PC)
        {
            Debug.Log("222");
            tempMovement.x = Input.GetAxis("Horizontal");
            movementComponent.RequestMoveByFrame(tempMovement, MovementPlayer.MovementMode.PlayerControl, Space.Self);

            if (Input.GetButtonDown("Jump"))
            {
                movementComponent.RequestJump();
            }
            if (Input.GetButton("Crouch"))
            {
                movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.Crouch);
            }
    }
        else
        {
            Debug.Log("333");
            tempMovement.x = joystick.Horizontal;
            movementComponent.RequestMoveByFrame(tempMovement, MovementPlayer.MovementMode.PlayerControl, Space.Self);

            if (platformJudge.IsJump())
            {
                movementComponent.RequestJump();
                platformJudge.ClearJump();
            }
            if (joystick.Vertical < -0.5f)
            {
                movementComponent.RequestChangeControlStatus(0f, MovementPlayer.PlayerControlStatus.Crouch);
            }
        }

        
    }

    public override int GetPriorityInType()
    {
        return priorityInType;
    }

    public override UpdateType GetUpdateType()
    {
        return updateType;
    }
}
