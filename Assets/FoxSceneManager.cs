using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FoxSceneManager : MonoBehaviour
{
    private bool isFoxReady;
    [SerializeField] private GameObject Fox;
    [SerializeField] private GameObject FoxPrefab;
    [SerializeField] private GameObject arrow;
    private Animator animator;
    private DialogManager foxDM;
    [SerializeField] private GameObject mouse;
    private DialogManager mouseDM;
    [SerializeField] private GameObject triggerArea;
    [SerializeField] private GameObject triggerAreaExit;
    [SerializeField] private Transform[] pathFox;
    [SerializeField] private int indexPath;
    [SerializeField] private Transform currentPath;
    private float timeBeforeAbleToSkipDialogue = 1f;
    private StoryManager storyManager;
    [SerializeField] private int stage;
    [SerializeField] private int dialogueStep;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        storyManager = GameObject.Find("StoryManager").GetComponent<StoryManager>();
        Fox.transform.Find("Model").gameObject.SetActive(false); // unrender fox at the start
        animator = FoxPrefab.GetComponent<Animator>();
        animator.enabled = false;
        foxDM = Fox.transform.Find("Speech Control").GetComponent<DialogManager>();
        mouseDM = mouse.transform.Find("Speech Control").GetComponent<DialogManager>();
        triggerAreaExit.SetActive(false);
        stage = 0;
        dialogueStep = 0;
    }
    
    private void DialogueFunc(DialogManager DM,string s)
    {
        if (dialogueStep == 0)
        {
            DM.ShowCallSymbol(false); 
            DM.ShowDialogue(s);
            dialogueStep++;
            timer = Time.time;
        }
        if (dialogueStep == 1) // waiting for dialogue to finish
        {
            if (!DM.isWriting)
            {
                dialogueStep++;
            }
            if ( (timer + timeBeforeAbleToSkipDialogue < Time.time) && (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0)) // move to next scene ** replace with touch screen
            {
                timer = Time.time;
                DM.QuickFinishDialogue();
                dialogueStep++;
            }
        }
        if (dialogueStep == 2)
        {
            if ((timer + timeBeforeAbleToSkipDialogue < Time.time) && (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                stage++;
                dialogueStep = 0;
                DM.CloseDIalogue();
            }
            //somehow to to next stage
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (stage == 0)
        {
            storyManager.TextInstructionStart("The mouse take a stroll through the deep, dark wood.");
            if (triggerArea.GetComponent<TriggerArea>().IsMouseEnter()){
                storyManager.TextInstructionClose();
                stage++;
            }
            // waiting for mouse to come in the trag
        }
        if (stage == 1)
        {
            Fox.transform.Find("Model").gameObject.SetActive(true);
            stage++;
        }
        if (stage == 2)
        {
            if (indexPath != pathFox.Length)
            {
                animator.enabled = true;
                currentPath = pathFox[indexPath];
                LookAtYaxis(Fox, currentPath.position);
                Fox.transform.position = Vector3.MoveTowards(Fox.transform.position, currentPath.position, storyManager.GetAnimalSpeed() * Time.deltaTime);
                if (Fox.transform.position == currentPath.position)
                {
                    indexPath++;
                }
            }
            else
            {
                animator.enabled = false;
                LookAtYaxis(Fox, mouse.transform.position);
                foxDM.ShowCallSymbol(true);
                indexPath = 0;
                stage++;
            }
        }
        if (stage == 3)
        {
            // click on interact button when near fox
            if (Vector3.Distance(mouse.transform.position,Fox.transform.position) < storyManager.GetInteractDistance()) 
            {
                Debug.Log("in view");
                storyManager.ShowInteraction(true,"A fox saw the mouse, and the mouse looked good.");
            }
            else
            {
                Debug.Log(Vector3.Distance(mouse.transform.position, Fox.transform.position));
                storyManager.ShowInteraction(false,"");
                // TODO: display to move closer to the fox
            }
            if (storyManager.GetInteractIsClicked()) 
            { 
                foxDM.ShowCallSymbol(false);
                storyManager.ShowInteraction(false, "");
                storyManager.SetIsInDialogue(true);
                stage++;
            }
        }
        if (stage == 4)
        {
            DialogueFunc(foxDM, "Where are you going to, little brown mouse? Come and have lunch in my underground house");
        }
        if (stage == 5)
        {
            DialogueFunc(mouseDM, "It's terribly kind of you, Fox, but no I'm going to have lunch with a gruffalo.");
        }
        if (stage == 6)
        {
            DialogueFunc(foxDM, "A gruffalo? What's a gruffalo?");
        }
        if (stage == 7)
        {
            DialogueFunc(mouseDM, "A gruffalo! Why, didn't you know? He has terrible tusks, and terrible claws, And terrible teeth in his terrible jaws.");
        }
        if (stage == 8)
        {
            DialogueFunc(foxDM, "Where are you meeting him?");
        }
        if (stage == 9)
        {
            DialogueFunc(mouseDM, "Here, by these rocks, And his favourite food is roasted fox.");
        }
        if (stage == 10)
        {
            DialogueFunc(foxDM, "Roasted fox! I'm off!");
        }
        if (stage == 11)
        {
            DialogueFunc(foxDM, "Goodbye, little mouse,");
        }
        if (stage == 12)
        {
            foxDM.CloseDIalogue();
            mouseDM.CloseDIalogue();
            if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                stage++;
            }
        }
        if (stage == 13)
        {
            if (indexPath != pathFox.Length)
            {
                animator.enabled = true;
                currentPath = pathFox[(pathFox.Length-1)-indexPath];
                LookAtYaxis(Fox, currentPath.position);
                Fox.transform.position = Vector3.MoveTowards(Fox.transform.position, currentPath.position, storyManager.GetAnimalSpeed() * Time.deltaTime);
                if (Fox.transform.position == currentPath.position)
                {
                    indexPath++;
                }
            }
            else
            {
                animator.enabled = false;
                Fox.transform.Find("Model").gameObject.SetActive(false);
                stage++;
            }
        }
        if (stage == 14)
        {
            DialogueFunc(mouseDM, "Silly old Fox! Doesn't he know, There's no such thing as a gruffalo?");
        }
        if (stage == 15)
        {
            mouseDM.CloseDIalogue();
            if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                storyManager.SetIsInDialogue(false);
                arrow.SetActive(true);
                stage++;
            }
        }
        if (stage == 16)
        {
            if (!triggerAreaExit.activeSelf)
            {
                triggerAreaExit.SetActive(true);
            }
            if (triggerAreaExit.GetComponent<TriggerArea>().IsMouseEnter())
            {
                storyManager.GoToOwlScene();
                //exit
            }
        }
    }
    private void LookAtYaxis(GameObject self, Vector3 target)
    {
        Vector3 direction = Vector3.Normalize(target - self.transform.position);
        Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
        self.transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
