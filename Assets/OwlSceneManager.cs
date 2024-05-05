using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject Owl;
    [SerializeField] private GameObject owlPrefab;
    [SerializeField] private GameObject arrow;
    private Animator animator;
    private DialogManager owlDM;
    [SerializeField] private GameObject mouse;
    private DialogManager mouseDM;
    [SerializeField] private GameObject triggerArea;
    [SerializeField] private GameObject triggerAreaExit;
    [SerializeField] private Transform[] pathOwl;
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
        animator = owlPrefab.GetComponent<Animator>();
        animator.enabled = false;
        Owl.transform.Find("Model").gameObject.SetActive(true); // unrender fox at the start
        owlDM = Owl.transform.Find("Speech Control").GetComponent<DialogManager>();
        mouseDM = mouse.transform.Find("Speech Control").GetComponent<DialogManager>();
        triggerAreaExit.SetActive(false);
        stage = 0;
        dialogueStep = 0;
    }

    private void DialogueFunc(DialogManager DM, string s)
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
            if ((timer + timeBeforeAbleToSkipDialogue < Time.time) && (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0)) // move to next scene ** replace with touch screen
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
            storyManager.TextInstructionStart("The mouse go through the deep, dark wood.");
            if (triggerArea.GetComponent<TriggerArea>().IsMouseEnter())
            {
                storyManager.TextInstructionClose();
                stage++;
            }
            // waiting for mouse to come in the trag
        }
        if (stage == 1)
        {
            stage++;
        }
        if (stage == 2)
        {
            if (indexPath != pathOwl.Length)
            {
                animator.enabled = true;
                currentPath = pathOwl[indexPath];
                LookAtYaxis(Owl, currentPath.position);
                Owl.transform.position = Vector3.MoveTowards(Owl.transform.position, currentPath.position, storyManager.GetAnimalSpeed() * Time.deltaTime);
                if (Owl.transform.position == currentPath.position)
                {
                    indexPath++;
                }
            }
            else
            {
                animator.enabled = false;
                LookAtYaxis(Owl, mouse.transform.position);
                owlDM.ShowCallSymbol(true);
                indexPath = 0;
                stage++;
            }
        }
        if (stage == 3)
        {
            // click on interact button when near fox
            if (Vector3.Distance(mouse.transform.position, Owl.transform.position) < storyManager.GetInteractDistance())
            {
                storyManager.ShowInteraction(true, "An owl saw the mouse, and the mouse looked good.");
            }
            else
            {
                storyManager.ShowInteraction(false, "");
                // TODO: display to move closer to the fox
            }
            if (storyManager.GetInteractIsClicked())
            {
                owlDM.ShowCallSymbol(false);
                storyManager.ShowInteraction(false, "");
                storyManager.SetIsInDialogue(true);
                stage++;
            }
        }
        if (stage == 4)
        {
            DialogueFunc(owlDM, "Where are you going to, little brown mouse? Come and have tea in my treetop house.");
        }
        if (stage == 5)
        {
            DialogueFunc(mouseDM, "It's terribly kind of you, Owl, but no I'm going to have tea with a gruffalo.");
        }
        if (stage == 6)
        {
            DialogueFunc(owlDM, "A gruffalo? What's a gruffalo?");
        }
        if (stage == 7)
        {
            DialogueFunc(mouseDM, "A gruffalo! Why, didn't you know? He has knobbly knees, and turned -out toes, And a poisonous wart at the end of his nose.");
        }
        if (stage == 8)
        {
            DialogueFunc(owlDM, "Where are you meeting him?");
        }
        if (stage == 9)
        {
            DialogueFunc(mouseDM, "Here, by this stream, And his favourite food is owl ice cream.");
        }
        if (stage == 10)
        {
            DialogueFunc(owlDM, "Owl ice cream! Toowhit toowhoo!");
        }
        if (stage == 11)
        {
            DialogueFunc(owlDM, "Goodbye, little mouse,");
        }
        if (stage == 12)
        {
            owlDM.CloseDIalogue();
            mouseDM.CloseDIalogue();
            if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                stage++;
            }
        }
        if (stage == 13)
        {
            if (indexPath != pathOwl.Length)
            {
                animator.enabled = true;
                currentPath = pathOwl[(pathOwl.Length - 1) - indexPath];
                LookAtYaxis(Owl, currentPath.position);
                Owl.transform.position = Vector3.MoveTowards(Owl.transform.position, currentPath.position, storyManager.GetAnimalSpeed() * Time.deltaTime);
                if (Owl.transform.position == currentPath.position)
                {
                    indexPath++;
                }
            }
            else
            {
                animator.enabled = false;
                Owl.transform.Find("Model").gameObject.SetActive(false);
                stage++;
            }
        }
        if (stage == 14)
        {
            DialogueFunc(mouseDM, "Silly old Owl! Doesn't he know, There's no such thing as a gruffalo?");
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
                storyManager.GoToSnakeScene();
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
