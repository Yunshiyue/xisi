using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class answer : MonoBehaviour
{

    [Header("选择题")]
    //public Button choiceA;
    //public Button choiceB;
    //public Button choiceC;
    //public Button choiceD;
    public TextAsset Question;


    [Header("判断题")]
    public Button right;
    public Button wrong;



    [Header("其他判断")]
    private bool isjudge;
    //private bool isrightAnswer;
    public int point = 0;
    public string rightAnswer;
    public bool isEndquestion = false;
    public bool isquest;


    [Header("题库")]
    public int index;
    List<string> textList = new List<string>();//将文件分成行后储存到这个列表里
    public Text questContent;
    public string choose;

    // Start is called before the first frame update
    void Awake()
    {
        GetTextContent(Question);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEndquestion && index<textList.Count)
        {
            
            if (isnumeric(textList[index]))
            {
                index++;
            }
            switch (textList[index])//判断题目类型，以及是否到达答案的标志
            {
                case "判断题":
                    isjudge = true;
                    index++;
                    isquest = true;
                    break;
                case "选择题":
                    isjudge = false;
                    index++;
                    break;
                case "answer":
                    index++;
                    rightAnswer = textList[index];
                    break;
                case "end":
                    isEndquestion = true;
                    break;
            }
            if (isjudge && !isEndquestion && isquest)//如果是判断题
            {
                Debug.Log("in the content");
                questContent.text = textList[index];//赋予题目
                isquest = false;
                index++;
                
            }
            //else if(!isjudge && !isEndquestion && isquest)//如果是选择题
            //{
            //    questContent.text = textList[index];
            //    index++;
            //    //Text textA = choiceA.transform.Find("Text").GetComponent<Text>();//给ABCD四个选项赋值
            //    //textA.text = textList[index];
            //    //index++;
            //    //Text textB = choiceB.transform.Find("Text").GetComponent<Text>();
            //    //textB.text = textList[index];
            //    //index++;
            //    //Text textC = choiceC.transform.Find("Text").GetComponent<Text>();
            //    //textC.text = textList[index];
            //    //Text textD = choiceD.transform.Find("Text").GetComponent<Text>();
            //    //textD.text = textList[index];
            //    //index++;
            //}
            
            
        }
        //if (choose.Trim().Equals(rightAnswer.Trim()) && rightAnswer != "")
        //{
        //    Debug.Log("Choosed");
        //    Debug.Log(choose);
        //    point = point + 10;
        //}

    }

    void GetTextContent(TextAsset file)
    {
        textList.Clear();
        index = 0;
        var lineData = file.text.Split('\n');

        foreach (var line in lineData)
        {
            textList.Add(line);
        }
    }
    public bool isnumeric(string str)
    {
        char[] ch = new char[str.Length];
        ch = str.ToCharArray();
        for (int i = 0; i<str.Length;i++) {
            if (ch[i] < 48 || ch[i] > 57)
                return false;
        }
        return true;
    }
    public void chooseRightAnswer()
    {
        point = point + 10;
    }
    public void ChooseA()
    {
        Debug.Log("It choose A");
        choose = "A";
        Debug.Log(choose);
        Debug.Log(rightAnswer);
        if (choose.Trim().Equals(rightAnswer))
        {
            Debug.Log("Enter right answer");
            chooseRightAnswer();
        }
    }
    public void ChooseB()
    {
        Debug.Log("It is choose B");
        choose = "B";
        Debug.Log(choose);
        Debug.Log(rightAnswer);
        if (choose.Trim().Equals(rightAnswer))
        {
            Debug.Log("Enter right answer");
            chooseRightAnswer();
        }
    }
    public void nextQuestion()
    {
        Debug.Log(point);
        isEndquestion = false;
    }
}
