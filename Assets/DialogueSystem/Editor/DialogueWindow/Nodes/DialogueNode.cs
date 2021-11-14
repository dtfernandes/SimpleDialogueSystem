using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    /// <summary>
    /// Class responsible for storing the data of a DialogueNode
    /// </summary>
    public class DialogueNode : DefaultNode
    {

        public string PresetName { get; set; }

        public TextField textfield;
       
        public List<EventTriggerData> Events { get; set; }

        /// <summary>
        /// Text component of the Dialogue Node
        /// </summary>
        public string DialogText { get; set; }

        /// <summary>
        /// Property that defines if this node is connected to the Node "Start"
        /// </summary>
        public bool EntryPoint { get; set; }
       
        private SavingWaitingList saveWatingList;
        public SavingWaitingList SaveWatingList { get => saveWatingList; set => saveWatingList = value; }

        public DialogueNode(SavingWaitingList savingWaitingList, NodeData nd = null)
        {

            SaveWatingList = savingWaitingList;

            GUID = nd == null ? Guid.NewGuid().ToString() : nd.GUID;
            title = "Dialogue Node";
            DialogText = nd == null ? "" : nd.Dialogue;
            Events = nd == null ? new List<EventTriggerData> { } : nd.Events;

            if (nd != null)
                foreach (ChoiceData cd in nd.OutPorts)
                    InstatiateChoicePort(cd.ChoiceText, cd.ID);

            //Create and add button that creates choice components
            Button button = new Button(clickEvent: () =>
            {
                InstatiateChoicePort();
            });
            button.text = "Add Choice";
            titleContainer.Insert(1, button);

            Port port = InstatiateInputPort();
            inputContainer.Add(port);
            port.portName = "Input";
                                 
            //Create and Add a textField to input the dialogue
            TextField text = new TextField();
            textfield = text;

            text.RegisterCallback<ChangeEvent<string>>((ChangeEvent<string> evt) =>
            {
                DialogText = evt.newValue;
            });
            text.multiline = true;

            if (nd == null)
                text.value = $"\n\n\n";
            else
                text.value = $"{nd?.Dialogue}";

            mainContainer.Insert(1, text);
       
            #region Entity Preset PopUp
            //Load Entities and their presets 
            EntityData data = Resources.Load<EntityData>("EntityData");
            List<string> names = data.presetNames;
            
            //Get the selected preset
            int firstSelected = 0;
            if(nd != null) 
                firstSelected = names.IndexOf(nd.PresetName);

            try
            {
                PopupField<string> justfortest = new PopupField<string>(names, firstSelected);
            }
            catch 
            {
                firstSelected = 0;
                Debug.LogWarning("Dude... The preset list disapeared somehow again");
            }

            PopupField<string> presetPopUp = new PopupField<string>(names, firstSelected);

            PresetName = nd == null ? presetPopUp.value : nd.PresetName;

            presetPopUp.RegisterCallback<ChangeEvent<string>>((ChangeEvent<string> evt) =>
            {
                PresetName = presetPopUp.value;
                EnableInspectorDisplay();
            });
            extensionContainer.Add(presetPopUp);

            #endregion


            RegisterCallback<PointerDownEvent>((PointerDownEvent evt) =>
            {
                EnableInspectorDisplay();
            });

            RefreshExpandedState();
            //expanded = true;
        }

        private DialogueNodeInspector inspector;

        private void EnableInspectorDisplay()
        {
            inspector =
                  ScriptableObject.CreateInstance("DialogueNodeInspector")
                       as DialogueNodeInspector;
            inspector.init(this);
            Selection.activeObject = inspector;

            textfield.UnregisterCallback<ChangeEvent<string>>(ChangeText);          
            textfield.RegisterCallback<ChangeEvent<string>>(ChangeText);
        }

        private void ChangeText(ChangeEvent<string> evt)
        {
            inspector.ChangeDialogue(evt.newValue, true);
        }


        private void InstatiateChoicePort(string choice = "", string id = "")
        {
            ChoiceData cD = new ChoiceData(choice, id);
            OutPorts.Add(cD);
            int index = OutPorts.IndexOf(cD);


            Port port = InstatiateOutputPort();
            port.portName = "";

            //Create and Add a textField to input the dialogue
            TextField textNode = new TextField();
        
            textNode.value = choice;
            textNode.multiline = true;

            textNode.RegisterCallback<ChangeEvent<string>>((ChangeEvent<string> evt) =>
            {
                if (port.connected)
                {
                    OutPorts[index].ChangeText(evt.newValue);
                }
            });


            //The Event responsible for managing the connetions of two ports
            port.RegisterCallback<MouseUpEvent>((MouseUpEvent evt) =>
            {             
                if (port.connected)
                {
                    foreach (Edge e in port.connections)
                    {
                        OutPorts[index].ChangeId((e.input.node as DefaultNode).GUID);
                        
                        e.RegisterCallback<DetachFromPanelEvent>
                        ((DetachFromPanelEvent evnt) =>
                        {
                            OutPorts[index].ChangeId("");
                        });
                    }
                }
            });

            //Create and Add Button to delete a Choice Port
            Button deleteButton = new Button();
       
            deleteButton.text = "X";
            
            VisualElement choiceContainer = new VisualElement();
            choiceContainer.Add(deleteButton);
            choiceContainer.Add(textNode);
            choiceContainer.Add(port);
            choiceContainer.styleSheets.Add(
                Resources.Load<StyleSheet>("ChoiceComponent_Style"));

            deleteButton.RegisterCallback<ClickEvent>((ClickEvent evt) =>
            {           
                foreach (Edge e in port.connections)
                {
                    e.input.Disconnect(e);

                    e.parent.Remove(e);
                }
                outputContainer.Remove(choiceContainer);
                OutPorts.Remove(cD);
                
            });


            outputContainer.Add(choiceContainer);

            RefreshExpandedState();
        }
    
        public void SetInInitialPosition(GraphView graph)
        {
            float width = graph.parent.contentRect.width;
            float height = graph.parent.contentRect.height;

            float x = -graph.viewTransform.position.x +
                width * 0.1f;

            float y = -graph.viewTransform.position.y +
                height * 0.1f;
                                 
            Vector2 position = new Vector2(x, y);
            Vector2 size = GetPosition().size;

            //Accout for node size
            //This is not implemented
            Vector2 pos = new Vector2(
                position.x - size.x / 2,
                position.y - size.y / 2
               );

            Rect rect = new Rect(position, size);
            SetPosition(rect);
        }

        public void SetInInitialPosition(Rect position)
        {
            SetPosition(position);
        }
   
        public void ConnectPorts(NodeData nd, DialogueGraphView graph)
        {
            //foreach(ChoiceComponent)
        }
    
    }
}
