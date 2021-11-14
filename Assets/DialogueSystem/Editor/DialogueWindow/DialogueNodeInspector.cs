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

        //[SerializeField]
        private EntityInfo entityInfo;
        
        private string presetEntityName;

        private SavingWaitingList saveWaitingList;
        public SavingWaitingList SaveWaitingList { get => saveWaitingList; set => saveWaitingList = value; }

        public void init(DialogueNode dn)
        {
            node = dn;
            isStart = dn.EntryPoint;
            dialogueText = dn.DialogText;
            events = node.Events;
            presetEntityName = dn.PresetName;
            SaveWaitingList = dn.SaveWatingList;

            EntityData data = Resources.Load<EntityData>("EntityData");
            entityInfo = data[presetEntityName];

        }

        public void ChangeDialogue(string newDialogue, bool fromTxt = false)
        {

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
