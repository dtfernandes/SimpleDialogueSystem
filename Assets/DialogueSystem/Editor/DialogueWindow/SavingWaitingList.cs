using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Editor
{
    public class SavingWaitingList
    {
        private List<GameObject> waitListToAdd;
        private List<GameObject> waitListToDelete;

        public SavingWaitingList()
        {
            WaitListToAdd = new List<GameObject> { };
            WaitListToDelete = new List<GameObject> { };
        }

        public List<GameObject> WaitListToAdd { get => waitListToAdd; set => waitListToAdd = value; }
        public List<GameObject> WaitListToDelete { get => waitListToDelete; set => waitListToDelete = value; }

        internal void Save(DialogueScript script)
        {

            foreach (GameObject g in waitListToAdd)
            {           
                DialogueEventManager.AddNewGameObject(g, script);
            }

            foreach (GameObject g in waitListToDelete)
            {
                DialogueEventManager.RemoveGameObject(g, script);
            }

            waitListToAdd.Clear();
            waitListToDelete.Clear();

         

            DialogueEventManager.ClearTemp();            
        }
    }
}
