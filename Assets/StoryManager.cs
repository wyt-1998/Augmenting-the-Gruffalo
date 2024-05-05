using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    private enum scene { Menu,FindQR,Fox,Snake,Owl, Graffelo,End}
    [Header("Setting")]
    [SerializeField] private float InteractDistance = 0.7f;
    [SerializeField] private float animalSpeed = 0.07f;
    [Header("Managing Scene")]
    private bool QRFound = false;
    [SerializeField] private scene currentScene;
    [SerializeField] private bool isInDialogue;

    [Header("Scene Prefab")]
    [SerializeField] private GameObject foxScenePrefab;
    [SerializeField] private GameObject owlScenePrefab;
    [SerializeField] private GameObject snakeScenePrefab;
    [SerializeField] private GameObject graffeloScenePrefab;
    private GameObject triggerPoint; // store position that mouse need to walk to for calcuation of the distance
    private GameObject currentScenePrefab;



    [Header("Managing Manu Prefab")]
    [SerializeField] private GameObject sceneUILockButton;
    [SerializeField] private GameObject sceneUIUnlockButton;
    [SerializeField] private GameObject sceneUILockAndUnlock;
    [SerializeField] private bool isLock = false;
    [SerializeField] private GameObject sceneUITextInstruction;
    [SerializeField] private CanvasGroup sceneUITextInstructionCanvasGroup;
    [SerializeField] private GameObject sceneUIMainManu;
    [SerializeField] private GameObject sceneUIGameStage;
    [SerializeField] private GameObject sceneUIFindQR;
    [SerializeField] private GameObject sceneUIEventTrigger;
    [SerializeField] private GameObject sceneUIReturnButton;
    [SerializeField] private GameObject sceneUIBackCheck;

    private Vector3 lastScenePosition;
    private Quaternion lastSceneRotation;

    private bool InteractIsClicked;
    private float interactTimer;
    [SerializeField] private bool fadeInstruction = false;
    [SerializeField] private bool instructionShown = false;
    //need to put how many subscene does each scene have

    public void LockButton()
    {
        isLock = false;
        sceneUILockButton.SetActive(false);
        sceneUIUnlockButton.SetActive(true);
    }
    public void UnlockButton() 
    { 
        isLock = true;
        sceneUILockButton.SetActive(true);
        sceneUIUnlockButton.SetActive(false);
    }
    public void SetIsInDialogue(bool b)
    {
        isInDialogue = b;
    }
    public bool GetIsInDialogue()
    {
        return isInDialogue;
    }
    public void FoundQR()
    {
        QRFound = true;
    }
    public float GetAnimalSpeed()
    {
        return animalSpeed;
    }
    public void InteractButtonClicked()
    {
        InteractIsClicked = true;
        interactTimer = Time.time +1f;
    }
    public float GetInteractDistance()
    {
        return InteractDistance;
    }
    public bool GetIsLocked()
    {
        return isLock;
    }
    public void TextInstructionStart(string t)
    {
        
        if (!sceneUITextInstruction.activeSelf)
        {
            sceneUITextInstruction.SetActive(true);
        }
        sceneUITextInstruction.GetComponent<TextMeshProUGUI>().text = t;
        if (!instructionShown)
        {
            fadeInstruction = true;
        }
    }
    public void TextInstructionClose()
    {
        if (instructionShown)
        {
            fadeInstruction = true;
        }
    }
    public bool GetInteractIsClicked()
    {
        return InteractIsClicked;
    }

    public void GoToOwlScene()
    {
        lastScenePosition = currentScenePrefab.transform.Find("Scene").gameObject.transform.position;
        lastSceneRotation = currentScenePrefab.transform.Find("Scene").gameObject.transform.rotation;
        Destroy(currentScenePrefab);
        currentScenePrefab = null;
        currentScene = scene.Owl;
    }
    public void BackButton()
    {
        //show a confirmation button
        sceneUIBackCheck.SetActive(true);
    }
    public void BackButtonConfirmation(bool b)
    {
        sceneUIBackCheck.SetActive(false);
        if (b)
        {
            SetIsInDialogue(false);
            Destroy(currentScenePrefab);
            if (sceneUIEventTrigger.activeSelf)
            {
                ShowInteraction(false, "");
            }
            currentScenePrefab = null;
            currentScene = scene.Menu;
        }
    }
    public void GoToSnakeScene()
    {
        lastScenePosition = currentScenePrefab.transform.Find("Scene").gameObject.transform.position;
        lastSceneRotation = currentScenePrefab.transform.Find("Scene").gameObject.transform.rotation;
        Destroy(currentScenePrefab);
        currentScenePrefab = null;
        currentScene = scene.Snake;
    }
    public void GoToGraffeloScene()
    {
        lastScenePosition = currentScenePrefab.transform.Find("Scene").gameObject.transform.position;
        lastSceneRotation = currentScenePrefab.transform.Find("Scene").gameObject.transform.rotation;
        Destroy(currentScenePrefab);
        currentScenePrefab = null;
        currentScene = scene.Graffelo;
    }
    public void ShowInteraction(bool b,string s)
    {
        if (b)
        {
            if (!sceneUIEventTrigger.activeSelf)
            {
                sceneUIEventTrigger.GetComponent<EventTriggerTextScript>().SetText(s);
                sceneUIEventTrigger.SetActive(true);
            }
        }
        else
        {
            if (sceneUIEventTrigger.activeSelf)
            {
                sceneUIEventTrigger.SetActive(false);
            }
        }

    }
    public void QuitButton()
    {
        Application.Quit();
    }
    void Start()
    {
        sceneUITextInstructionCanvasGroup.alpha = 0f;
    }
    public void StartButton()
    {
        //QRFound = false;
        LockButton();
        currentScene = scene.FindQR;
        sceneUIMainManu.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (fadeInstruction && !instructionShown) // case to fade in instruction
        {
            if (sceneUITextInstructionCanvasGroup.alpha < 1f)
            {
                sceneUITextInstructionCanvasGroup.alpha += Time.deltaTime * 5f;
            }
            else
            {
                fadeInstruction = false;
                instructionShown = true;
            }
        }
        else if (fadeInstruction && instructionShown) // case to fade out instruction
        {
            if (sceneUITextInstructionCanvasGroup.alpha > 0f)
            {
                sceneUITextInstructionCanvasGroup.alpha -= Time.deltaTime * 5f;
            }
            else
            {
                if (sceneUITextInstruction.activeSelf)
                {
                    sceneUITextInstruction.SetActive(false);
                }
                instructionShown = false;
                fadeInstruction = false;
            }
        }

        if (InteractIsClicked && Time.time > interactTimer)
        {
            InteractIsClicked = false;
        }

        if (currentScene == scene.Menu)
        {
            if (!sceneUIMainManu.activeSelf)
            {
                sceneUIMainManu.SetActive(true);
            }
            if (sceneUIReturnButton.activeSelf)
            {
                sceneUIReturnButton.SetActive(false);
            }
            if (sceneUILockAndUnlock.activeSelf)
            {
                sceneUILockAndUnlock.SetActive(false);
            }
        }

        if (currentScene == scene.FindQR)
        {
            if (!sceneUIFindQR.activeSelf)
            {
                sceneUIFindQR.SetActive(true);
            }
            if (sceneUILockAndUnlock.activeSelf)
            {
                sceneUILockAndUnlock.SetActive(false);
            }
            if (!sceneUIReturnButton.activeSelf)
            {
                sceneUIReturnButton.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Space) || QRFound)
            {
                sceneUIFindQR.SetActive(false);
                currentScene = scene.Fox;
            }
        }

        if (currentScene == scene.Fox)
        {
            if (currentScenePrefab == null)
            {
                currentScenePrefab = Instantiate(foxScenePrefab);
            }
            if (!sceneUIReturnButton.activeSelf)
            {
                sceneUIReturnButton.SetActive(true);
            }
            if (!sceneUILockAndUnlock.activeSelf)
            {
                sceneUILockAndUnlock.SetActive(true);
            }
        }

        if (currentScene == scene.Owl)
        {
            if (currentScenePrefab == null)
            {
                currentScenePrefab = Instantiate(owlScenePrefab);
                var temp = currentScenePrefab.transform.Find("Scene").gameObject;
                temp.transform.position = lastScenePosition;
                temp.transform.rotation = lastSceneRotation;
                //temp.transform.rotation = lastScenePosition.rotation;
            }
            if (!sceneUIReturnButton.activeSelf)
            {
                sceneUIReturnButton.SetActive(true);
            }
            if (!sceneUILockAndUnlock.activeSelf)
            {
                sceneUILockAndUnlock.SetActive(true);
            }
        }

        if (currentScene == scene.Snake)
        {
            if (currentScenePrefab == null)
            {
                currentScenePrefab = Instantiate(snakeScenePrefab);
                var temp = currentScenePrefab.transform.Find("Scene").gameObject;
                temp.transform.position = lastScenePosition;
                temp.transform.rotation = lastSceneRotation;
                //temp.transform.rotation = lastScenePosition.rotation;
            }
            if (!sceneUIReturnButton.activeSelf)
            {
                sceneUIReturnButton.SetActive(true);
            }
            if (!sceneUILockAndUnlock.activeSelf)
            {
                sceneUILockAndUnlock.SetActive(true);
            }
        }
        if (currentScene == scene.Graffelo)
        {
            if (currentScenePrefab == null)
            {
                currentScenePrefab = Instantiate(graffeloScenePrefab);
                var temp = currentScenePrefab.transform.Find("Scene").gameObject;
                temp.transform.position = lastScenePosition;
                temp.transform.rotation = lastSceneRotation;
                //temp.transform.rotation = lastScenePosition.rotation;
            }
            if (!sceneUIReturnButton.activeSelf)
            {
                sceneUIReturnButton.SetActive(true);
            }
            if (!sceneUILockAndUnlock.activeSelf)
            {
                sceneUILockAndUnlock.SetActive(true);
            }
        }
    }
}
