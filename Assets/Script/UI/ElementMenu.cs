/**
 * @Description: ElementMenu类是切换元素菜单界面的控制类
 * @Author: CuteRed

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementMenu : MonoBehaviour
{
    private Dictionary<int, ElementAbilityManager.Element> elements = 
        new Dictionary<int, ElementAbilityManager.Element>();

    /// <summary>
    /// 主元素下拉菜单
    /// </summary>
    private Dropdown mainDropdown;
    private Image mainImg;

    /// <summary>
    /// A元素下拉菜单
    /// </summary>
    private Dropdown aDropdown;
    private Image aImg;

    /// <summary>
    /// B元素下拉菜单
    /// </summary>
    private Dropdown bDropdown;
    private Image bImg;

    private Text elementTip;

    private ElementAbilityManager elementAbilityManager;

    //四种主元素图片
    public Sprite fireMainImg;
    public Sprite iceMainImg;
    public Sprite thunderMainImg;
    public Sprite windMainImg;
    
    //四种辅元素图片
    public Sprite fireImg;
    public Sprite iceImg;
    public Sprite thunderImg;
    public Sprite windImg;
    public Sprite nullImg;

    private void Awake()
    {
        //初始化元素字典
        elements.Add(0, ElementAbilityManager.Element.Fire);
        elements.Add(1, ElementAbilityManager.Element.Ice);
        elements.Add(2, ElementAbilityManager.Element.Wind);
        elements.Add(3, ElementAbilityManager.Element.Thunder);
        elements.Add(4, ElementAbilityManager.Element.NULL);

        //初始化子选项
        mainDropdown = GameObject.Find("MainDropdown").GetComponent<Dropdown>();
        mainImg = GameObject.Find("MainImage").GetComponent<Image>();

        aDropdown = GameObject.Find("ADropdown").GetComponent<Dropdown>();
        aImg = GameObject.Find("AImage").GetComponent<Image>();

        bDropdown = GameObject.Find("BDropdown").GetComponent<Dropdown>();
        bImg = GameObject.Find("BImage").GetComponent<Image>();

        //监听事件
        mainDropdown.onValueChanged.AddListener(MainElementChangeUI);
        aDropdown.onValueChanged.AddListener(AElementChangeUI);
        bDropdown.onValueChanged.AddListener(BElementChangeUI);

        //初始化提示文本
        elementTip = GameObject.Find("ElementTipText").GetComponent<Text>();
        if (elementTip == null)
        {
            Debug.LogError("在" + gameObject.name + "中，找不到ElementTipText组件");
        }
        //元素重复提示文本消失
        elementTip.gameObject.SetActive(false);

        //初始化技能管理器
        elementAbilityManager = GameObject.Find("Player").GetComponent<ElementAbilityManager>();

    }

    private void OnEnable()
    {
        //元素重复提示文本消失
        elementTip.gameObject.SetActive(false);
    }
    
    private void MainElementChangeUI(int arg0)
    {
        switch (arg0)
        {
            case 0:
                mainImg.overrideSprite = fireMainImg;
                break;
            case 1:
                mainImg.overrideSprite = iceMainImg;
                break;
            case 2:
                mainImg.overrideSprite = windMainImg;
                break;
            case 3:
                mainImg.overrideSprite = thunderMainImg;
                break;
        }
    }private void AElementChangeUI(int arg0)
    {
        switch (arg0)
        {
            case 0:
                aImg.overrideSprite = fireImg;
                break;
            case 1:
                aImg.overrideSprite = iceImg;
                break;
            case 2:
                aImg.overrideSprite = windImg;
                break;
            case 3:
                aImg.overrideSprite = thunderImg;
                break;
            case 4:
                aImg.overrideSprite = nullImg;
                break;
        }
    }private void BElementChangeUI(int arg0)
    {
        switch (arg0)
        {
            case 0:
                bImg.overrideSprite = fireImg;
                break;
            case 1:
                bImg.overrideSprite = iceImg;
                break;
            case 2:
                bImg.overrideSprite = windImg;
                break;
            case 3:
                bImg.overrideSprite = thunderImg;
                break;
            case 4:
                bImg.overrideSprite = nullImg;
                break;
        }
    }

    /// <summary>
    /// 确定元素
    /// </summary>
    public void ConfirmElement()
    {
        //有元素重复
        if (mainDropdown.value == aDropdown.value || mainDropdown.value == bDropdown.value ||
            (aDropdown.value == bDropdown.value && aDropdown.value != 4))
        {
            elementTip.gameObject.SetActive(true);
        }
        else
        {
            //换元素
            elementAbilityManager.SwitchElement
                (elements[mainDropdown.value], elements[aDropdown.value], elements[bDropdown.value]);

            //隐藏界面
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 更改下拉菜单
    /// </summary>
    public void RefreshItem()
    {
        //更改下拉菜单
        mainDropdown.value = (int)elementAbilityManager.GetMainElement();
        aDropdown.value = (int)elementAbilityManager.GetAElement();
        bDropdown.value = (int)elementAbilityManager.GetBElement();
    }
}