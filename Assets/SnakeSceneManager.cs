using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SnakeSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject Snake;
    [SerializeField] private GameObject snakePrefab;
    [SerializeField] private GameObject arrow;
    private Animator animator;
    private DialogManager snakeDM;
    [SerializeField] private GameObject mouse;
    private DialogManager mouseDM;
    [SerializeField] private GameObject triggerArea;
    [SerializeField] private GameObject triggerAreaExit;
    [SerializeField] private Transform[] pathSnake;
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
        animator = snakePrefab.GetComponent<Animator>();
        animator.enabled = false;
        storyManager = GameObject.Find("StoryManager").GetComponent<StoryManager>();
        Snake.transform.Find("Model").gameObject.SetActive(false); // unrender fox at the start
        snakeDM = Snake.transform.Find("Speech Control").GetComponent<DialogManager>();
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
            Snake.transform.Find("Model").gameObject.SetActive(true);
            stage++;
        }
        if (stage == 2)
        {
            if (indexPath != pathSnake.Length)
            {
                animator.enabled = true;
                currentPath = pathSnake[indexPath];
                LookAtYaxis(Snake, currentPath.position);
                Snake.transform.position = Vector3.MoveTowards(Snake.transform.position, currentPath.position, storyManager.GetAnimalSpeed() * Time.deltaTime);
                if (Snake.transform.position == currentPath.position)
                {
                    indexPath++;
                }
            }
            else
            {
                animator.enabled = false;
                LookAtYaxis(Snake, mouse.transform.position);
                snakeDM.ShowCallSymbol(true);
                indexPath = 0;
                stage++;
            }
        }
        if (stage == 3)
        {
            // click on interact button when near fox
            if (Vector3.Distance(mouse.transform.position, Snake.transform.position) < storyManager.GetInteractDistance())
            {
                storyManager.ShowInteraction(true, "A snake saw the mouse, and the mouse looked good.");
            }
            else
            {
                storyManager.ShowInteraction(false, "");
                // TODO: display to move closer to the fox
            }
            if (storyManager.GetInteractIsClicked())
            {
                snakeDM.ShowCallSymbol(false);
                storyManager.ShowInteraction(false, "");
                storyManager.SetIsInDialogue(true);
                stage++;
            }
        }
        if (stage == 4)
        {
            DialogueFunc(snakeDM, "Where are you going to, little brown mouse? Come for a feast in my logpile house.");
        }
        if (stage == 5)
        {
            DialogueFunc(mouseDM, "It's terribly kind of you, Snake, but no I'm having a feast with a gruffalo.");
        }
        if (stage == 6)
        {
            DialogueFunc(snakeDM, "A gruffalo? What's a gruffalo?");
        }
        if (stage == 7)
        {
            DialogueFunc(mouseDM, "A gruffalo! Why, didn't you know? His eyes are orange, his tongue is black, He has purple prickles all over his back.");
        }
        if (stage == 8)
        {
            DialogueFunc(snakeDM, "Where are you meeting him?");
        }
        if (stage == 9)
        {
            DialogueFunc(mouseDM, "Here, by this lake,And his favourite food is scrambled snake.");
        }
        if (stage == 10)
        {
            DialogueFunc(snakeDM, "Scrambled snake! It's time I hid!");
        }
        if (stage == 11)
        {
            DialogueFunc(snakeDM, "Goodbye, little mouse,");
        }
        if (stage == 12)
        {
            snakeDM.CloseDIalogue();
            mouseDM.CloseDIalogue();
            if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                stage++;
            }
        }
        if (stage == 13)
        {
            if (indexPath != pathSnake.Length)
            {
                animator.enabled = true;
                currentPath = pathSnake[(pathSnake.Length - 1) - indexPath];
                LookAtYaxis(Snake, currentPath.position);
                Snake.transform.position = Vector3.MoveTowards(Snake.transform.position, currentPath.position, storyManager.GetAnimalSpeed() * Time.deltaTime);
                if (Snake.transform.position == currentPath.position)
                {
                    indexPath++;
                }
            }
            else
            {
                animator.enabled = false;
                Snake.transform.Find("Model").gameObject.SetActive(false);
                stage++;
            }
        }
        if (stage == 14)
        {
            DialogueFunc(mouseDM, "Silly old snake! Doesn't he know, There's no such thing as a gruffal...?");
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
                storyManager.GoToGraffeloScene();
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
