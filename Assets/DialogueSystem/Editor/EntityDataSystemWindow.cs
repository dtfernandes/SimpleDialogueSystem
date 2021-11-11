using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace DialogueSystem.Editor
{
    public class EntityDataSystemWindow : EditorWindow 
    {

        EntityData data;
        SerializedObject serObj;
        // The variable to control where the scrollview 
        //'looks' into its child elements.
        Vector2 scrollPosition;
        
        /// <summary>
        /// Method responsible for opening the EntityDataSystem Window 
        /// </summary>
        [MenuItem("Dialogue/Entity Data")]
        public static void OpenEntityDataWindow()
        {
            EntityDataSystemWindow window = GetWindow<EntityDataSystemWindow>();
            window.titleContent = new GUIContent(text: "Entity Data");
            window.data = GetEntityData();
            DisplayData(window.data);
        }


        private void OnGUI()
        {

            
            if (GUILayout.Button("Add New Preset"))
            {
                data.AddNewPreset();
            }

            GUILayout.Space(20);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < data.data.Count; i++)
            {
                EntityInfo info = data.data[i];

                DrawUILine(new Color(0.1f, 0.1f, 0.1f, 1), 2);

                
                if(GUILayout.Button($"Preset {i}", 
                   new GUIStyle(GUI.skin.label)))
                {
                    data.data[i].Hidden = !data.data[i].Hidden;
                }
                //GUILayout.Box(EditorGUIUtility.IconContent("PlayButton"));

                if (data.data[i].Hidden)
                {
                    GUILayout.BeginHorizontal();

                    info.EntityName = EditorGUILayout.TextField(info.EntityName,
                        GUILayout.Width(Math.Min(300, position.width - 120)));
                    GUILayout.Space(10);

                    GUILayout.FlexibleSpace();

                    info.EntitySprite =
                    (Sprite)EditorGUILayout.ObjectField(obj: info.EntitySprite,
                    objType: typeof(Sprite), false,
                    GUILayout.Height(100), GUILayout.Width(100));

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(5);
            GUILayout.EndScrollView();
        }

        private static void DisplayData(EntityData data)
        {
            
        }

        private static EntityData GetEntityData()
        {
           
            //AssetDatabase.DeleteAsset("Assets/DialogueSystem/Editor/Resources/EntityData.asset");           
            EntityData[] foundObjects = Resources.LoadAll<EntityData>("");

            bool dataExists = foundObjects.Length > 0;
            
            if (!dataExists)
            {
                Debug.Log("Curious");
                EntityData asset = ScriptableObject.CreateInstance<EntityData>();
                AssetDatabase.CreateAsset(asset,
                    "Assets/DialogueSystem/Editor/Resources/EntityData.asset");
            }
            
            return Resources.Load<EntityData>("EntityData");
        }

        //alexanderamey
        //https://forum.unity.com/threads/horizontal-line-in-editor-window.520812/
        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

    }

}
