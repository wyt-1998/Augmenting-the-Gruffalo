using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GraffeloSceneManager : MonoBehaviour
{
    //Animals Gameobject
    [Header("Animal gameobject")]
    private float animalMoveSpeed = 0.07f;
    [SerializeField] private GameObject mouse;
    [SerializeField] private GameObject mouseMovement;
    [SerializeField] private GameObject fox;
    [SerializeField] private GameObject owl;
    [SerializeField] private GameObject snake;
    [SerializeField] private GameObject graffelo;

    private DialogManager mouseDM;
    private DialogManager foxDM;
    private DialogManager owlDM;
    private DialogManager snakeDM;
    private DialogManager graffeloDM;
    private TextMeshProUGUI interactbutton;

    [Header("Scene gameobject")]
    [SerializeField] private GameObject scene_1;
    [SerializeField] private GameObject scene_2; // with snake
    [SerializeField] private GameObject scene_3; // with owl
    [SerializeField] private GameObject scene_4; // fox

    [SerializeField] private GameObject scene2Position;
    [SerializeField] private GameObject scene3Position;
    [SerializeField] private GameObject scene4Position;

    [SerializeField] private GameObject arrow1;
    [SerializeField] private GameObject arrow2;
    [SerializeField] private GameObject arrow3;

    [SerializeField] private GameObject graffeloScenePrefab;
    [Header("Scene 1")]
    [SerializeField] private GameObject triggerArea;
    [SerializeField] private GameObject triggerAreaExit;
    [SerializeField] private Transform[] InitialPathGrafello;
    [Header("Scene 2")]
    [SerializeField] private GameObject triggerArea2;
    [SerializeField] private GameObject triggerAreaExit2;
    [SerializeField] private Transform[] snakePath;
    [Header("Scene 3")]
    [SerializeField] private GameObject triggerArea3;
    [SerializeField] private GameObject triggerAreaExit3;
    [SerializeField] private Transform[] owlPath;
    [Header("Scene 4")]
    [SerializeField] private GameObject triggerArea4;
    [SerializeField] private Transform[] foxPath;
    [SerializeField] private GameObject graffeloRunAwayPos;

    [SerializeField] private float distanceGraffeloFollow;

    private int indexPath;
    private Transform currentPath;
    private float timeBeforeAbleToSkipDialogue = 0.5f;
    private StoryManager storyManager;

    [Header("Current Info")]
    [SerializeField] private int stage;
    [SerializeField] private int dialogueStep;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        storyManager = GameObject.Find("StoryManager").GetComponent<StoryManager>();

        mouseDM = mouse.transform.Find("Speech Control").GetComponent<DialogManager>();
        foxDM = fox.transform.Find("Speech Control").GetComponent<DialogManager>();
        owlDM = owl.transform.Find("Speech Control").GetComponent<DialogManager>();
        snakeDM = snake.transform.Find("Speech Control").GetComponent<DialogManager>();
        graffeloDM = graffelo.transform.Find("Speech Control").GetComponent<DialogManager>();

        graffelo.transform.Find("Model").gameObject.SetActive(false);
        snake.transform.Find("Model").gameObject.SetActive(false);
        fox.transform.Find("Model").gameObject.SetActive(false);
        triggerAreaExit.SetActive(false);
        stage = 0;
        dialogueStep = 0;


        arrow1.SetActive(false);
        arrow2.SetActive(false);
        arrow3.SetActive(false);
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
            // wait for mouse to enter the trigger zone
            if (triggerArea.GetComponent<TriggerArea>().IsMouseEnter())
            {
                stage++;
            }
        }
        if (stage == 1)
        {
            graffelo.transform.Find("Model").gameObject.SetActive(true);
            stage++;
        }
        if (stage == 2)
        {
            if (indexPath != InitialPathGrafello.Length)
            {
                graffelo.GetComponent<Animator>().enabled = true;
                currentPath = InitialPathGrafello[indexPath];
                LookAtYaxis(graffelo, currentPath.position);
                graffelo.transform.position = Vector3.MoveTowards(graffelo.transform.position, currentPath.position, animalMoveSpeed * Time.deltaTime);
                if (graffelo.transform.position == currentPath.position)
                {
                    indexPath++;
                }
            }
            else
            {
                LookAtYaxis(graffelo, mouse.transform.position);
                graffelo.GetComponent<Animator>().enabled = false;
                graffeloDM.ShowCallSymbol(true);
                indexPath = 0;
                stage++;
            }
        }
        if (stage == 3)
        {
            // click on interact button when near fox
            if (Vector3.Distance(mouse.transform.position, graffelo.transform.position) < storyManager.GetInteractDistance())
            {
                storyManager.ShowInteraction(true, "OH..Is Gruffalo real?!?!");
            }
            else
            {
                storyManager.ShowInteraction(false, "");
                // TODO: display to move closer to the fox
            }
            if (storyManager.GetInteractIsClicked())
            {
                graffeloDM.ShowCallSymbol(false);
                storyManager.ShowInteraction(false, "");
                storyManager.SetIsInDialogue(true);
                stage++;
            }
        }
        if (stage == 4)
        {
            //DialogueFunc(mouseDM, "Oh help! Oh no! It's a Gruffalo!");
            stage++;
        }
        if (stage == 5)
        {
            DialogueFunc(graffeloDM, "My favourite food!");
        }
        if (stage == 6)
        {
            DialogueFunc(graffeloDM, "You'll taste good on a slice of bread!");
        }
        if (stage == 7)
        {
            DialogueFunc(mouseDM, "Good?");
        }
        if (stage == 8)
        {
            DialogueFunc(mouseDM, "Don't call me good! I'm the scariest creature in this wood.");
        }
        if (stage == 9)
        {
            DialogueFunc(mouseDM, "Just walk behind me and soon you'll see, Everyone is afraid of me.");
        }
        if (stage == 10)
        {
            DialogueFunc(graffeloDM, "All right. You go ahead and I'll follow after.");
        }
        if (stage == 11)
        {
            graffeloDM.CloseDIalogue();
            mouseDM.CloseDIalogue();
            if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                storyManager.SetIsInDialogue(false);
                arrow1.SetActive(true);
                stage++;
            }
        }
        if (stage >= 12 && (stage <= 64)) // follow mouse after stage 12
        {
            // look at
            LookAtYaxis(graffelo, mouse.transform.position);
            // graffelo follow the mouse
            if (Vector3.Distance(mouse.transform.position, graffelo.transform.position) > distanceGraffeloFollow)
            {
                Vector3 direction = Vector3.Normalize(mouse.transform.position - graffelo.transform.position);
                graffelo.transform.position = Vector3.MoveTowards(graffelo.transform.position, mouse.transform.position, animalMoveSpeed * Time.deltaTime);
                graffelo.GetComponent<Animator>().enabled = true;
            }
            else
            {
                graffelo.GetComponent<Animator>().enabled = false;
            }
        }
        if (stage == 12)
        {
            if (!triggerAreaExit.activeSelf)
            {
                triggerAreaExit.SetActive(true);
            }
            if (triggerAreaExit.GetComponent<TriggerArea>().IsMouseEnter())
            {
                //render next scene
                storyManager.SetIsInDialogue(true);
                stage++;
                //exit
            }
        }
        if (stage == 13)
        {
            // scene 2 appear
            scene_2.SetActive(true);
            if (scene_2.transform.position.y < scene2Position.transform.position.y)
            {
                scene_2.transform.position += new Vector3 (0, 0.07f, 0) * Time.deltaTime;
            }
            else
            {
                stage++;
            }
        }
        if (stage == 14)
        {
            scene_1.SetActive(false);
            if (scene_2.transform.position != scene2Position.transform.position)
            {
                scene_2.transform.position = Vector3.MoveTowards(scene_2.transform.position, scene2Position.transform.position, 0.25f * Time.deltaTime);
                graffelo.transform.parent = scene_2.transform;
                mouse.transform.parent = scene_2.transform;
                mouseMovement.transform.parent = scene_2.transform;
            }
            else
            {
                storyManager.SetIsInDialogue(false);
                stage++;
            }
        }
        if (stage == 15)
        {
            if (triggerArea2.GetComponent<TriggerArea>().IsMouseEnter())
            {
                stage++;
            }
        }
        if (stage == 16)
        {
            snake.transform.Find("Model").gameObject.SetActive(true);
            snake.transform.Find("Model").transform.Find("Snake").GetComponent<Animator>().enabled = true;
            stage++;
        }
        if (stage == 17)
        {
            if (indexPath != snakePath.Length)
            {
                currentPath = snakePath[indexPath];
                LookAtYaxis(snake, currentPath.position);
                snake.transform.position = Vector3.MoveTowards(snake.transform.position, currentPath.position, animalMoveSpeed * Time.deltaTime);
                if (snake.transform.position == currentPath.position)
                {
                    indexPath++;
                }
            }
            else
            {
                LookAtYaxis(snake, mouse.transform.position);
                snakeDM.ShowCallSymbol(true);
                snake.transform.Find("Model").transform.Find("Snake").GetComponent<Animator>().enabled = false;
                indexPath = 0;
                stage++;
            }
        }
        if (stage == 18)
        {
            // click on interact button when near fox
            if (Vector3.Distance(mouse.transform.position, snake.transform.position) < storyManager.GetInteractDistance())
            {
                storyManager.ShowInteraction(true, "You have been approched by Snake");
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
        if (stage == 19)
        {
            DialogueFunc(graffeloDM, "I hear a hiss in the leaves ahead.");
        }
        if (stage == 20)
        {
            DialogueFunc(mouseDM, "It's Snake.");
        }
        if (stage == 21)
        {
            DialogueFunc(mouseDM, "Why, Snake, hello!");
        }
        if (stage == 22)
        {
            DialogueFunc(snakeDM, "Oh crumbs!");
        }
        if (stage == 23)
        {
            DialogueFunc(snakeDM, "Goodbye, little mouse.");
        }
        if (stage == 24)
        {
            snakeDM.CloseDIalogue();
            mouseDM.CloseDIalogue();
            graffeloDM.CloseDIalogue();
            if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                stage++;
                snake.transform.Find("Model").transform.Find("Snake").GetComponent<Animator>().enabled = true;
            }
        }
        if (stage == 25)
        {
            if (indexPath != snakePath.Length)
            {
                currentPath = snakePath[(snakePath.Length - 1) - indexPath];
                LookAtYaxis(snake, currentPath.position);
                snake.transform.position = Vector3.MoveTowards(snake.transform.position, currentPath.position, animalMoveSpeed * Time.deltaTime);
                if (Vector3.Distance(snake.transform.position, currentPath.position) < 0.1f)
                {
                    indexPath++;
                }
            }
            else
            {
                snake.transform.Find("Model").transform.Find("Snake").GetComponent<Animator>().enabled = false;
                snake.transform.Find("Model").gameObject.SetActive(false);
                stage++;
                indexPath = 0;
            }
        }
        if (stage == 26)
        {
            DialogueFunc(mouseDM, "You see? I told you so.");
        }
        if (stage == 27)
        {
            DialogueFunc(graffeloDM, "Amazing!");
        }
        if (stage == 28)
        {
            mouseDM.CloseDIalogue();
            graffeloDM.CloseDIalogue();
            if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                stage++;
                storyManager.SetIsInDialogue(false);
                arrow2.SetActive(true);
            }
        }
        if (stage == 29)
        {
            if (!triggerAreaExit2.activeSelf)
            {
                triggerAreaExit2.SetActive(true);
            }
            if (triggerAreaExit2.GetComponent<TriggerArea>().IsMouseEnter())
            {
                //render next scene
                stage++;
                storyManager.SetIsInDialogue(true);
                //exit
            }
        }
        if (stage == 30)
        {
            // scene 3 appear
            scene_3.SetActive(true);
            if (scene_3.transform.position.y < scene3Position.transform.position.y)
            {
                scene_3.transform.position += new Vector3(0, 0.07f, 0) * Time.deltaTime;
            }
            else
            {
                stage++;
            }
        }
        if (stage == 31)
        {
            scene_2.SetActive(false);
            if (scene_3.transform.position != scene3Position.transform.position)
            {
                scene_3.transform.position = Vector3.MoveTowards(scene_3.transform.position, scene3Position.transform.position, 0.25f * Time.deltaTime);
                graffelo.transform.parent = scene_3.transform;
                mouse.transform.parent = scene_3.transform;
                mouseMovement.transform.parent = scene_3.transform;
            }
            else
            {
                stage++;
                storyManager.SetIsInDialogue(false);
            }
        }
        if (stage == 32)
        {
            if (triggerArea3.GetComponent<TriggerArea>().IsMouseEnter())
            {
                stage++;
            }
        }
        if (stage == 33)
        {
            owl.transform.Find("Model").gameObject.SetActive(true);
            owl.transform.Find("Model").transform.Find("owl").GetComponent<Animator>().enabled = true;
            stage++;
        }
        if (stage == 34)
        {
            if (indexPath != owlPath.Length)
            {
                currentPath = owlPath[indexPath];
                LookAtYaxis(owl, currentPath.position);

                owl.transform.position = Vector3.MoveTowards(owl.transform.position, currentPath.position, animalMoveSpeed * Time.deltaTime);
                if (owl.transform.position == currentPath.position)
                {
                    indexPath++;
                }
            }
            else
            {
                LookAtYaxis(owl, mouse.transform.position);
                owlDM.ShowCallSymbol(true);
                owl.transform.Find("Model").transform.Find("owl").GetComponent<Animator>().enabled = false;
                indexPath = 0;
                stage++;
            }
        }
        if (stage == 35)
        {
            // click on interact button when near fox
            if (Vector3.Distance(mouse.transform.position, owl.transform.position) < storyManager.GetInteractDistance())
            {
                storyManager.ShowInteraction(true, "You have been approched by owl");
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
        if (stage == 36)
        {
            DialogueFunc(graffeloDM, "I hear a hoot in the trees ahead.");
        }
        if (stage == 37)
        {
            DialogueFunc(mouseDM, "It's Owl.");
        }
        if (stage == 38)
        {
            DialogueFunc(mouseDM, "Why, Owl, hello!");
        }
        if (stage == 39)
        {
            DialogueFunc(owlDM, "Oh dear!");
        }
        if (stage == 40)
        {
            DialogueFunc(owlDM, "Goodbye, little mouse.");
        }
        if (stage == 41)
        {
            owlDM.CloseDIalogue();
            mouseDM.CloseDIalogue();
            graffeloDM.CloseDIalogue();
            if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                stage++;
                owl.transform.Find("Model").transform.Find("owl").GetComponent<Animator>().enabled = true;
            }
        }
        if (stage == 42)
        {
            if (indexPath != owlPath.Length)
            {
                currentPath = owlPath[(owlPath.Length - 1) - indexPath];
                LookAtYaxis(owl, currentPath.position);
                owl.transform.position = Vector3.MoveTowards(owl.transform.position, currentPath.position, animalMoveSpeed * Time.deltaTime);
                if (owl.transform.position == currentPath.position)
                {
                    indexPath++;
                }
            }
            else
            {
                owl.transform.Find("Model").transform.Find("owl").GetComponent<Animator>().enabled = false;
                owl.transform.Find("Model").gameObject.SetActive(false);
                stage++;
                indexPath = 0;
            }
        }
        if (stage == 43)
        {
            DialogueFunc(mouseDM, "You see? I told you so.");
        }
        if (stage == 44)
        {
            DialogueFunc(graffeloDM, "Astounding!");
        }
        if (stage == 45)
        {
            mouseDM.CloseDIalogue();
            graffeloDM.CloseDIalogue();
            if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                storyManager.SetIsInDialogue(false);
                arrow3.SetActive(true);
                stage++;
            }
        }
        if (stage == 46)
        {
            if (!triggerAreaExit3.activeSelf)
            {
                triggerAreaExit3.SetActive(true);
            }
            if (triggerAreaExit3.GetComponent<TriggerArea>().IsMouseEnter())
            {
                //render next scene
                stage++;
                storyManager.SetIsInDialogue(true);
                //exit
            }
        }
        if (stage == 47)
        {
            // scene 3 appear
            scene_4.SetActive(true);
            if (scene_4.transform.position.y < scene4Position.transform.position.y)
            {
                scene_4.transform.position += new Vector3(0, 0.07f, 0) * Time.deltaTime;
            }
            else
            {
                stage++;
            }
        }
        if (stage == 48)
        {
            scene_3.SetActive(false);
            if (scene_4.transform.position != scene4Position.transform.position)
            {
                scene_4.transform.position = Vector3.MoveTowards(scene_4.transform.position, scene4Position.transform.position, 0.25f * Time.deltaTime);
                graffelo.transform.parent = scene_4.transform;
                mouse.transform.parent = scene_4.transform;
                mouseMovement.transform.parent = scene_4.transform;
            }
            else
            {
                stage++;
                storyManager.SetIsInDialogue(false);
            }
        }
        if (stage == 49)
        {
            if (triggerArea4.GetComponent<TriggerArea>().IsMouseEnter())
            {
                stage++;
            }
        }
        if (stage == 50)
        {
            fox.transform.Find("Model").gameObject.SetActive(true);
            fox.transform.Find("Model").transform.Find("fox").GetComponent<Animator>().enabled = true;
            stage++;
        }
        if (stage == 51)
        {
            if (indexPath != foxPath.Length)
            {
                currentPath = foxPath[indexPath];
                LookAtYaxis(fox, currentPath.position);
                fox.transform.position = Vector3.MoveTowards(fox.transform.position, currentPath.position, animalMoveSpeed * Time.deltaTime);
                if (fox.transform.position == currentPath.position)
                {
                    indexPath++;
                }
            }
            else
            {
                LookAtYaxis(fox, mouse.transform.position);
                foxDM.ShowCallSymbol(true);
                fox.transform.Find("Model").transform.Find("fox").GetComponent<Animator>().enabled = false;
                indexPath = 0;
                stage++;
            }
        }
        if (stage == 52)
        {
            // click on interact button when near fox
            if (Vector3.Distance(mouse.transform.position, fox.transform.position) < storyManager.GetInteractDistance())
            {
                storyManager.ShowInteraction(true, "You have been approched by fox");
            }
            else
            {
                storyManager.ShowInteraction(false, "");
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
        if (stage == 53)
        {
            DialogueFunc(graffeloDM, "I can hear feet on the path ahead.");
        }
        if (stage == 54)
        {
            DialogueFunc(mouseDM, "It's Fox.");
        }
        if (stage == 55)
        {
            DialogueFunc(mouseDM, "Why, Fox, hello!");
        }
        if (stage == 56)
        {
            DialogueFunc(foxDM, "Oh help!");
        }
        if (stage == 57)
        {
            DialogueFunc(foxDM, "Goodbye, little mouse.");
        }
        if (stage == 58)
        {
            foxDM.CloseDIalogue();
            mouseDM.CloseDIalogue();
            graffeloDM.CloseDIalogue();
            if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                stage++;
                fox.transform.Find("Model").transform.Find("fox").GetComponent<Animator>().enabled = true;
            }
        }
        if (stage == 59)
        {
            if (indexPath != foxPath.Length)
            {
                currentPath = foxPath[(owlPath.Length - 1) - indexPath];
                LookAtYaxis(fox, currentPath.position);
                fox.transform.position = Vector3.MoveTowards(fox.transform.position, currentPath.position, animalMoveSpeed * Time.deltaTime);
                if (fox.transform.position == currentPath.position)
                {
                    indexPath++;

                }
            }
            else
            {
                fox.transform.Find("Model").transform.Find("fox").GetComponent<Animator>().enabled = false;
                fox.transform.Find("Model").gameObject.SetActive(false);
                stage++;
                indexPath = 0;
            }
        }
        if (stage == 60)
        {
            DialogueFunc(mouseDM, "Well, Gruffalo. You see? Everyone is afraid of me!");
        }
        if (stage == 61)
        {
            DialogueFunc(mouseDM, "But now my tummy's beginning to rumble.");
        }
        if (stage == 62)
        {
            DialogueFunc(mouseDM, "My favourite food is ?Gruffalo crumble!");
        }
        if (stage == 63)
        {
            DialogueFunc(graffeloDM, "Gruffalo crumble!");
        }



        if (stage == 64)
        {
            mouseDM.CloseDIalogue();
            graffeloDM.CloseDIalogue();
            if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                stage++;
            }
        }
        if (stage == 65)
        {
            LookAtYaxis(graffelo, graffeloRunAwayPos.transform.position);
            // graffelo follow the mouse
            if (graffeloRunAwayPos.transform.position != graffelo.transform.position)
            {
                graffelo.GetComponent<Animator>().enabled = true;
                graffelo.transform.position = Vector3.MoveTowards(graffelo.transform.position, graffeloRunAwayPos.transform.position, animalMoveSpeed *2* Time.deltaTime);
            }
            else
            {
                stage++;
                graffelo.GetComponent<Animator>().enabled = false;
                graffelo.transform.Find("Model").gameObject.SetActive(false);
            }
        }
        if (stage == 66)
        {
            DialogueFunc(mouseDM, "Silly Gruffly! Doesn't he know? My favourite food is nut.");
        }
        if (stage == 67)
        {
            mouseDM.CloseDIalogue();
            if ((Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))//PLACEHOLDER for touchscreen
            {
                stage++;
            }
        }
        if (stage == 68)
        {
            storyManager.ShowInteraction(true, "All was quiet in the deep dark wood.<br> The mouse found a nut and the nut was good.");
            GameObject.Find("Interact Button").GetComponent<TextMeshProUGUI>().text = "The End";
            if (storyManager.GetInteractIsClicked())
            {
                GameObject.Find("Interact Button").GetComponent<TextMeshProUGUI>().text = "Interact";
                storyManager.ShowInteraction(false, "");
                storyManager.SetIsInDialogue(false);
                storyManager.BackButtonConfirmation(true);
            }
        }

    }

    private void LookAtYaxis(GameObject self,Vector3 target)
    {
        Vector3 direction = Vector3.Normalize(target - self.transform.position);
        Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
        self.transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
