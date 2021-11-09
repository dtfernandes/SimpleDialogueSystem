using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;

namespace DialogueSystem.Editor
{
    [CustomEditor(typeof(DialogueNodeInspector))]
    public class DialogueNodeInspectorEditor : UnityEditor.Editor
    {
        SerializedProperty dialogueText;
        SerializedProperty events;

        int funcSelectPopup;

        void OnEnable()
        {
            dialogueText = serializedObject.FindProperty("dialogueText");
            events = serializedObject.FindProperty("events");
        }

        public override void OnInspectorGUI()
        {

            DialogueNodeInspector nodeInsp = target as DialogueNodeInspector;

            serializedObject.Update();

            base.OnInspectorGUI();

            string dialTxtTemp = dialogueText.stringValue; 
            EditorGUILayout.PropertyField(dialogueText, new GUIContent("Dialogue Text"));
            if (dialTxtTemp != dialogueText.stringValue)
                nodeInsp.ChangeDialogue(dialogueText.stringValue);

            GUILayout.BeginHorizontal();
          
            if(GUILayout.Button("Add Event"))
            {
                events.InsertArrayElementAtIndex(events.arraySize);
                SerializedProperty ev = events.GetArrayElementAtIndex(events.arraySize - 1);
                SerializedProperty gameObj =
                   ev.FindPropertyRelative("gameObj");
                SerializedProperty functionName =
                   ev.FindPropertyRelative("functionName");
                SerializedProperty index =
                   ev.FindPropertyRelative("indexPos");
                SerializedProperty showText =
                   ev.FindPropertyRelative("showText");

                gameObj.objectReferenceValue = null;
                functionName.stringValue = "";
                index.intValue = 0;
                showText.boolValue = false;


            }
           
            if (GUILayout.Button("Clear"))
            {
                
                events.ClearArray();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUI.indentLevel = 2;
      
            //Draw Event Trigger Data
            for (int i = 0; i < events.arraySize; i++)
            {

                #region Assign Serialized Properties
                SerializedProperty gameObj =
                   events.GetArrayElementAtIndex(i).FindPropertyRelative("gameObj");
                SerializedProperty functionName =
                   events.GetArrayElementAtIndex(i).FindPropertyRelative("functionName");
                SerializedProperty index =
                   events.GetArrayElementAtIndex(i).FindPropertyRelative("indexPos");
                SerializedProperty showText =
                   events.GetArrayElementAtIndex(i).FindPropertyRelative("showText");
                SerializedProperty showSelf =
                   events.GetArrayElementAtIndex(i).FindPropertyRelative("showSelf");
                SerializedProperty uniqueID =
                   events.GetArrayElementAtIndex(i).FindPropertyRelative("uniqueID");
                SerializedProperty listOfType =
                   events.GetArrayElementAtIndex(i).FindPropertyRelative("listOfType");
                SerializedProperty selectedComponent =
                   events.GetArrayElementAtIndex(i).FindPropertyRelative("selectedComponent");
                SerializedProperty seletedTypeIndex =
                   events.GetArrayElementAtIndex(i).FindPropertyRelative("seletedTypeIndex");
                SerializedProperty seletedMethodIndex =
                  events.GetArrayElementAtIndex(i).FindPropertyRelative("seletedMethodIndex");
                SerializedProperty listOfAssemblies =
                  events.GetArrayElementAtIndex(i).FindPropertyRelative("listOfAssemblies");
                #endregion

                GUILayout.Space(5);
                if (EditorGUILayout.DropdownButton(new GUIContent($"Event { i }"), FocusType.Passive))
                {
                    showSelf.boolValue = !showSelf.boolValue;
                }

                if (showSelf.boolValue)
                {
                    GUILayout.Space(10);

                    #region Show Text Toggle
                    EditorGUILayout.PropertyField(showText);
                    if (showText.boolValue)
                        DrawTriggerLabel(index.intValue, dialogueText.stringValue);
                    #endregion

                    #region GameObject Selection
                    GameObject gameObjTemp = gameObj.objectReferenceValue as GameObject;
                    EditorGUILayout.PropertyField(gameObj);

                    //On Change Game Object
                    if (gameObjTemp != gameObj.objectReferenceValue)
                    {

                        GameObject newGameObj = gameObj?.objectReferenceValue as GameObject;

                        DialogueUniqueId idOld = gameObjTemp?.GetComponent<DialogueUniqueId>();
                        DialogueUniqueId idNew =
                            (newGameObj)?.GetComponent<DialogueUniqueId>();

                        //Deselect Old Object
                        if (idOld)
                        {
                            if (idOld.TimesUsed == 1)
                                DestroyImmediate(idOld);
                            else
                                idOld.TimesUsed--;
                        }

                        //New Object Selected
                        if (newGameObj != null)
                        {
                            #region Create List of Types and Assemblies
                            Component[] components = newGameObj.GetComponents(typeof(Component));
                            List<System.Type> listType = new List<Type> { };
                            foreach (Component component in components)
                            {
                                listType.Add(component.GetType());                                                            
                            }

                            listOfType.ClearArray();

                            for (int o = 0; o < listType.Count; o++)
                            {
                                listOfType.InsertArrayElementAtIndex(listOfType.arraySize);
                                listOfAssemblies.InsertArrayElementAtIndex(listOfAssemblies.arraySize);

                                listOfType.GetArrayElementAtIndex(o).stringValue = listType[o].ToString();
                                listOfAssemblies.GetArrayElementAtIndex(o).stringValue =
                                    Assembly.GetAssembly(listType[o]).ToString();
                            }
                            #endregion 

                            if (idNew)
                            {
                                idNew.TimesUsed++;
                                uniqueID.stringValue = idNew.UniqueID;
                            }
                            else
                            {
                                DialogueUniqueId newId = newGameObj.AddComponent<DialogueUniqueId>();
                                newId.TimesUsed++;
                                newId.UniqueID = Guid.NewGuid().ToString();
                                uniqueID.stringValue = newId.UniqueID;
                            }
                        }
                        else
                        {
                            uniqueID.stringValue = "";
                        }
                    }
                    #endregion

                    #region Component Type PopUp
                    if (listOfType.arraySize > 0)
                    {
                        string[] typeNameList = new string[listOfType.arraySize];
                        for (int o = 0; o < listOfType.arraySize; o++)
                        {
                            typeNameList[o] = listOfType.GetArrayElementAtIndex(o).stringValue;
                        }

                        seletedTypeIndex.intValue =
                        EditorGUILayout.Popup(new GUIContent("Object Components"), seletedTypeIndex.intValue, typeNameList);                      
                    }
                    #endregion

                    #region Method PopUp
                    if (listOfType.arraySize > 0)
                    {
                        string assemblyName = listOfAssemblies.GetArrayElementAtIndex(seletedTypeIndex.intValue).stringValue;
                        string type = listOfType.GetArrayElementAtIndex(seletedTypeIndex.intValue).stringValue;
                        string qualifiedName = Assembly.CreateQualifiedName(assemblyName, type);
                        System.Type typeSelected = Type.GetType(qualifiedName);

                        MethodInfo[] methodInfo = typeSelected.GetMethods();
                        string[] methodNames = methodInfo.Select(x => x.Name).ToArray();
                        seletedMethodIndex.intValue =
                            EditorGUILayout.Popup(new GUIContent("Methods"), seletedMethodIndex.intValue, methodNames);
                        functionName.stringValue = methodNames[seletedMethodIndex.intValue];
                    }
                    #endregion

                    #region Unique Id
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    string idField = uniqueID.stringValue != "" ? uniqueID.stringValue : "no id";
                    GUILayout.Label(idField);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    #endregion

                    EditorGUILayout.PropertyField(functionName);

                    #region Selected Letter Index Trigger
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(28);
                    GUILayout.Label("Index");
                    index.intValue = (int)EditorGUILayout.Slider(index.intValue, 0, dialogueText.stringValue.Length - 1);
                    GUILayout.EndHorizontal();
                    #endregion

                    GUILayout.Space(10);

                 
                }
                GUILayout.Space(10);
            }

            EditorGUI.indentLevel = 1;

            if (GUILayout.Button("Test Function"))
            {
                

            }

            


            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTriggerLabel(int intValue, string text)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            
            for (int i = 0; i < text.Length; i++)
            {
                if (i % 25 == 0)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
   
                if (intValue == i)
                {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.red;
                    style.fontSize = 20;
                    GUILayout.Label(text[i].ToString(), style);
                }
                else
                {
                    GUILayout.Label(text[i].ToString());
                }

             
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        
    }
}
