using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DialogueSystem;
using TMPro;
using System.Linq;
using System.Collections.Generic;

//Class responsible for the handling of the Dialogue Display
public class DialogueDisplayHandler : MonoBehaviour
{
    /// <summary>
    /// Key used to pass from a line of dialogue to the next
    /// </summary>
    [SerializeField]
    private KeyCode passDialogueKey = default;

    /// <summary>
    /// Text component responsible for displaying the Dialogue text
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI dialogueDisplayTarget = default;


    /// <summary>
    /// Current Dialogue script beeing displayed 
    /// </summary>
    [SerializeField]
    private DialogueScript currentScript = default;


    /// <summary>
    /// Time between each char of the Dialogue
    /// </summary>
    [SerializeField]
    private float displaySpeed = default;


    /// <summary>
    /// GameObject that defines the container of the ChoiceButtons
    /// </summary>
    [SerializeField]
    private GameObject buttonLayout = default;

    /// <summary>
    /// GameObject that defines the container of the Name
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI nameContainer = default;

    /// <summary>
    /// Prefab of the ChoiceButton
    /// </summary>
    [SerializeField]
    private GameObject buttonPREFAB = default;


    /// <summary>
    /// Variable that defines the data of one line of dialogue
    /// </summary>
    private NodeData dialogueLine;


    /// <summary>
    /// ID component of the current line of dialogue
    /// </summary>
    private string currentGUID;


    /// <summary>
    /// Text component of the current line of dialogue
    /// </summary>
    private string dialogueText;


    /// <summary>
    /// Auxiliar vairable that represents the time between each char 
    /// in one line of dialogue
    /// </summary>
    private WaitForSeconds effectSpeed;


    /// <summary>
    /// Variable that defines if tha Dialogue has ended
    /// </summary>
    private bool ended;


    /// <summary>
    /// Variable that defines if the current line of dialogue is 
    /// currenly beeing displayed
    /// </summary>
    private bool inDialogue;


    [SerializeField]
    private bool playOnLoad = default;

    
    public System.Action onEndDialogue;
  
    private void Start()
    {
        if (playOnLoad)
            StartDialolgue(currentScript);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(passDialogueKey))
        {

            if (!inDialogue) return;

            if (ended)
            {
                if (dialogueLine.OutPorts.Count > 0)
                {
                    if (dialogueLine.OutPorts[0].ChoiceText != "") return;
                }
                
                NextLine(0);
            }
            else
            {              
                dialogueDisplayTarget.text += dialogueText;
                dialogueText = "";
                buttonLayout.SetActive(true);
                ended = true;
                StopCoroutine("TypeWriterEffect");
            }          
        }
    }


    /// <summary>
    /// Method responsible for switching to the passed DialogueScript
    /// </summary>
    /// <param name="script">Dialogue Script to inicialize</param>
    public void StartDialolgue(DialogueScript script)
    {
        currentScript = script;
        PrepareNewDialogue();
    }


    /// <summary>
    /// Method responsible for seting up the Dialogue
    /// </summary>
    public void PrepareNewDialogue()
    {
        //Display Specificationss
        effectSpeed = new WaitForSeconds(displaySpeed);

        //Initialize First Line
        dialogueLine = currentScript[0];
        dialogueText = currentScript[0].Dialogue;
        currentGUID = currentScript[0].GUID;

        //Handle button Layout
        InstatiateChoices();
        DisplayLine();
    }


    /// <summary>
    /// Method responsible for selecting and instantiating the respective 
    /// Choice Buttons of the current Dialogue 
    /// </summary>
    private void InstatiateChoices()
    {

        foreach (Transform g in buttonLayout.transform)
        {
            Destroy(g.gameObject);
        }

        int choiceNumb = dialogueLine.OutPorts.Count;
        if (choiceNumb == 0) return;

        for (int i = 0; i < choiceNumb; i++)
        {
            GameObject temp = Instantiate(buttonPREFAB, transform.position,
                Quaternion.identity, buttonLayout.transform);


            ChoiceSelector cs = temp.GetComponent<ChoiceSelector>();
            cs.ChangeChoiceText(dialogueLine.OutPorts[i].ChoiceText);
            cs.ChoiceNumb = i;
            cs.NextLine = NextLine;
        }

        buttonLayout.SetActive(false);

    }


    /// <summary>
    /// Method responsible for deciding initializing next line of the current 
    /// DialogueScript
    /// </summary>
    /// <param name="choice">The selected choice of the current line</param>
    public void NextLine(int choice)
    {

        dialogueLine =
               currentScript.GetNextNode(dialogueLine, choice);

        if (dialogueLine == null)
        {
            EndDialogue();
            return;
        }

        dialogueText = dialogueLine.Dialogue;

        InstatiateChoices();
        DisplayLine();
    }


    /// <summary>
    /// Method responsible for ending the current DialogueScript
    /// </summary>
    private void EndDialogue()
    {
        inDialogue = false;
        dialogueDisplayTarget.text = "";
        StopCoroutine("TypeWriterEffect");
    }


    /// <summary>
    /// Method that starts the next line in the Dialogue
    /// </summary>
    private void DisplayLine()
    {
        StopCoroutine("TypeWriterEffect");
        StartCoroutine("TypeWriterEffect");
    }


    /// <summary>
    /// IEnumerator that creates a TypeWriteEffect
    /// </summary>
    /// <returns></returns>
    IEnumerator TypeWriterEffect()
    {
        inDialogue = true;
        ended = false;
        dialogueDisplayTarget.text = "";
        int index = 0;
        while (dialogueText.Length > 0)
        {
            yield return effectSpeed;
            
            //Feio
            index++;
            foreach (EventTriggerData data in dialogueLine.Events)
            {                
                if (data.IndexPos == index)
                {
                    DialogueUniqueId[] test = GameObject.FindObjectsOfType<DialogueUniqueId>();
                    GameObject currentObject = null;
                  
                    foreach(DialogueUniqueId d in test)
                    {
                        if(d.UniqueID == data.UniqueID)
                        {
                            currentObject = d.gameObject;
                                                       
                            MonoBehaviour obj = currentObject.GetComponent(data.SelectedComponent) as MonoBehaviour;
                            obj.Invoke(data.FunctionName,0);
                            break;
                        }
                        
                    }
                
                }
            }

            char nextChar = dialogueText[0];
            dialogueDisplayTarget.text += nextChar;
            dialogueText = dialogueText.Substring(1);
        }
        ended = true;
        buttonLayout.SetActive(true);
    }


}
