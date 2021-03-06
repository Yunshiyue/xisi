/**
 * @Description: IceConeCannon类为冰锥发射器，每间隔一段时间发射一枚冰锥
 * @Author: CuteRed

 *     
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceConeCannon : MonoBehaviour
{
    private PoolManager poolManager;
    private CanFight canFight;

    [Header("伤害参数")]
    public float intervalTime = 3.0f;
    private float passTime = 0.0f;
    public Vector2 flyingDirection = Vector2.zero;

    private void Start()
    {
        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
        if (poolManager == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取PoolManager失败");
        }

        canFight = GetComponent<CanFight>();
        if (canFight == null)
        {
            Debug.LogError("在" + gameObject.name + "中，获取CanFight失败");
        }

        //使用string数组初始化canFight能够检测到的层
        string[] targets = new string[1];
        targets[0] = "Player";
        canFight.Initiailize(targets);
    }

    private void Update()
    {
        //计时
        passTime += Time.deltaTime;
        if (passTime > intervalTime)
        {
            passTime = 0.0f;
            FireIceCone();
        }
    }

    /// <summary>
    /// 发射雷球
    /// </summary>
    private void FireIceCone()
    {
        GameObject iceCone = poolManager.GetGameObject(IceCone.ICE_CONE);
        IceCone a = iceCone.GetComponent<IceCone>();
        a.SetThrower(gameObject);
        a.SetStartPosition(transform.position);
        a.SetTargetLayerName("Player");
        a.SetDamage(2);
        a.SetMaxExistTime(8.0f);

        a.SetDirection(flyingDirection);
    }
}
