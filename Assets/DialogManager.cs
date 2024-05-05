using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject callSymbol;
    [SerializeField] private CanvasGroup canvasGroupLeft;
    [SerializeField] private GameObject canvasTextLeft;
    [SerializeField] private CanvasGroup canvasGroupRight;
    [SerializeField] private GameObject canvasTextRight;
    private TextMeshProUGUI textLog;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float wordSpeed;
    [SerializeField] private int stage;
    private int randLR;
    private string completeText;
    private int index;
    private int indexMax;

    private Transform camera;
    private float timer;
    private char[] charList;
    [SerializeField] private bool quickFinish;
    [SerializeField] private bool clearDia;
    [SerializeField] public bool isWriting { get; set;}

    // Start is called before the first frame update
    private void OnEnable()
    {
        camera = GameObject.Find("AR Camera").transform;
        callSymbol.SetActive(false);
        canvasGroupLeft.alpha = 0f;
        canvasGroupRight.alpha = 0f;
    }
    // Update is called once per frame

    public void ShowCallSymbol(bool b)
    {
        callSymbol.SetActive(b);
    }
    public void ShowDialogue(string s)
    {

        randLR = UnityEngine.Random.Range(0, 2);
        if (randLR == 0)
        {
            Debug.Log("Left");
            textLog = canvasTextLeft.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.Log("Right");
            textLog = canvasTextRight.GetComponent<TextMeshProUGUI>();
        }
        //set up event
        stage = 0;
        isWriting = true;
        //text minipulation
        completeText = s;
        charList = s.ToCharArray();
        index = 0;
        indexMax = charList.Length;
    }
    public void QuickFinishDialogue()
    {
        if (isWriting)
        {
            quickFinish = true;
        }
    }

    public void CloseDIalogue()
    {
        clearDia = true;
    }
    void Update()
    {
        gameObject.transform.LookAt(camera);
        if (clearDia == true && !isWriting)
        {
            if (canvasGroupLeft.alpha > 0f)
            {
                canvasGroupLeft.alpha -= Time.deltaTime * fadeSpeed;
            }
            if (canvasGroupRight.alpha > 0f)
            {
                canvasGroupRight.alpha -= Time.deltaTime * fadeSpeed;
            }
            if (canvasGroupRight.alpha == 0f && canvasGroupLeft.alpha == 0f)
            {
                clearDia = false;
            }
        }
        if (isWriting)
        {
            if (stage == 0)
            {
                //close previous stuff
                if (canvasGroupLeft.alpha > 0f)
                {
                    canvasGroupLeft.alpha -= Time.deltaTime * fadeSpeed;
                }
                if (canvasGroupRight.alpha > 0f)
                {
                    canvasGroupRight.alpha -= Time.deltaTime * fadeSpeed;
                }
                if (canvasGroupRight.alpha == 0f && canvasGroupLeft.alpha == 0f)
                {
                    Invoke("ProceedToStage1", 0.2f);
                }
            }
            if (stage == 1)
            {
                textLog.alpha = 0f;
                textLog.enableAutoSizing = true;
                textLog.text = completeText;
                Invoke("ProceedToStage2", 0.2f);
            }
            if (stage == 2)
            {
                textLog.enableAutoSizing = false;
                textLog.text = "";
                Invoke("ProceedToStage3", 0.2f);
            }
            if (stage == 3)
            {
                textLog.alpha = 1f;
                if (quickFinish)
                {
                    textLog.text = completeText;
                    if (randLR == 0)
                    {
                        canvasGroupLeft.alpha = 1f;
                    }
                    else
                    {
                        canvasGroupRight.alpha = 1f;
                    }
                    quickFinish = false;
                    isWriting = false;
                    return;
                }

                if (randLR == 0)
                {
                    if (canvasGroupLeft.alpha < 1f)
                    {
                        canvasGroupLeft.alpha += Time.deltaTime * fadeSpeed;
                    }
                }
                else
                {
                    if (canvasGroupRight.alpha < 1f)
                    {
                        canvasGroupRight.alpha += Time.deltaTime * fadeSpeed;
                    }
                }
                if (canvasGroupRight.alpha == 1f || canvasGroupLeft.alpha == 1f)
                {
                    if (Time.time > timer)
                    {
                        textLog.text += charList[index];
                        index += 1;
                        timer = Time.time + wordSpeed;
                    }
                    if (index == indexMax)
                    {
                        isWriting = false;
                    }
                }
            }
        }
    }
    public void ProceedToStage1()
    {
        stage = 1;
    }
    public void ProceedToStage2()
    {
        stage = 2;
    }
    public void ProceedToStage3()
    {
        stage = 3;
    }

}