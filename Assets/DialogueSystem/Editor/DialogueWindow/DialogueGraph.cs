using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DialogueSystem.Editor
{
    /// <summary>
    /// Class responsible for managing the editor window to display
    /// the Node based Dialogue System
    /// </summary>
    public class DialogueGraph : EditorWindow
    {
        /// <summary>
        /// GraphView component of this window
        /// </summary>
        private DialogueGraphView graphview;

        /// <summary>
        /// An instance of the class responsible for loading
        /// and saving the Dialogue
        /// </summary>
        private SaveLoadUtils svUtil;

        /// <summary>
        /// Method called when the program starts 
        /// </summary>
        private void Awake()
        {
            svUtil = new SaveLoadUtils();
        }

        /// <summary>
        /// Method called when this script is enabled
        /// </summary>
        private void OnEnable()
        {            
            CreateGraphView();
            CreateToolbar();
        }

        /// <summary>
        /// Method called when this script is disabled
        /// </summary>
        private void OnDisable()
        {
            rootVisualElement.Remove(graphview);
        }


        /// <summary>
        /// Method responsible for opening the Dialogue Window 
        /// where the node system is placed
        /// </summary>
        [MenuItem("Dialogue/Dialogue Graph")]
        public static void OpenDialogueGraphWindow()
        {
            DialogueGraph window = GetWindow<DialogueGraph>();
            window.titleContent = new GUIContent(text: "Dialogue Graph");

        }

        /// <summary>
        /// Method responsible for opening the Dialogue Window 
        /// where the node system is placed.
        /// This overload is called when the program tries to load 
        /// an already existing Dialogue
        /// </summary>
        /// <param name="ds">Dialogue to be loaded</param>
        public static void OpenDialogueGraphWindow(DialogueScript ds)
        {          
            DialogueGraph window = GetWindow<DialogueGraph>();
            window.Close();
            window = GetWindow<DialogueGraph>();

            window.titleContent = new GUIContent(text: ds.DialogueName);
            
             //window.graphview.remove
            SaveLoadUtils svUtil = new SaveLoadUtils();
            svUtil.LoadDialogues(window.graphview, ds);
            
        }

      
        /// <summary>
        /// Method responsible for creating the Toolbar on top of the window
        /// as well as asigning its components
        /// </summary>
        private void CreateToolbar()
        {
            //Create new instance of the Toolbar
            Toolbar toolbar = new Toolbar();

            //Create button to instanciate a new node
            Button nodeCreateButton = new Button(clickEvent: () =>
            {
                graphview.CreateDialogueNode();
            });

            //Create button to save the Dialogue
            Button saveButton = new Button(clickEvent: () =>
            {
                DialogueScript script = 
                    svUtil.SaveDialogues(graphview, graphview.DialogueName);

                if(script != null)
                    graphview.SaveWatingList.Save(script);
            });

            //Add text to buttons
            nodeCreateButton.text = "Create Node";
            saveButton.text = "Save Dialogue";

            //Add buttons to toolbar
            toolbar.Add(nodeCreateButton);
            toolbar.Add(saveButton);

            //Add toolbar to this graph
            rootVisualElement.Add(toolbar);
        }


        /// <summary>
        /// Creates and adds the GraphView component of this window 
        /// </summary>
        private void CreateGraphView()
        {
            graphview = new DialogueGraphView
            {
                name = "Dialogue Graph"
            };

            graphview.StretchToParentSize();
            rootVisualElement.Add(graphview);
        }

       
    }
}
