using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueSystem.Editor
{
    public class DialogueNodeInspector : ScriptableObject
    {
        [SerializeField]
        private bool isStart;

        [SerializeField] [HideInInspector]
        private string dialogueText;

        private DialogueNode node;

        [HideInInspector]
        public List<EventTriggerData> events;

        [SerializeField]
        private EntityInfo entityInfo;

        private string presetEntity;

        public void init(DialogueNode dn)
        {
            node = dn;
            isStart = dn.EntryPoint;
            dialogueText = dn.DialogText;
            events = node.Events;
            presetEntity = dn.entityName;
        }

        public void ChangeDialogue(string newDialogue, bool fromTxt = false)
        {
            Debug.Log(presetEntity); 
            if (!fromTxt)
            {
                node.textfield.value = newDialogue;
                node.DialogText = newDialogue;
                //node.entityInfo = 
            }

            dialogueText = newDialogue;
        }

        public void TestList()
        {
            node.Events = events;
            Debug.Log(node.Events[0].IndexPos);
        }



    }




}
