using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Maybe transfomr into a Singleton
//For now this works
namespace DialogueSystem 
{
    public class DialogueEventManager : MonoBehaviour, ISerializationCallbackReceiver
    {

        [SerializeField]
        private List<GameObjectIdPar> SerializationParList;
        [SerializeField]
        private List<GameObjectIdPar> TestList;

        public void OnAfterDeserialize()
        {
            parList = SerializationParList;
            tempList = TestList;
        }

        public void OnBeforeSerialize()
        {
            SerializationParList = parList;
            TestList = tempList;
        }

        private void Start()
        {
        }

        private static List<GameObjectIdPar> parList;
        [SerializeField]
        private static List<GameObjectIdPar> tempList;

        [System.Serializable]
        private struct GameObjectIdPar
        {
            [SerializeField]
            string id;
            [SerializeField]
            GameObject gameObject;

            public GameObjectIdPar(string id, GameObject gameObject)
            {
                this.id = id;
                this.gameObject = gameObject;
            }

            public GameObject GameObject { get => gameObject; private set => gameObject = value; }
            public string Id { get => id; private set => id = value; }
        }

        public static string GetID(GameObject newGameObject)
        {
         
            if (GetGameObject(newGameObject) == null && 
                CheckTempList(newGameObject) == null)
            {
                string id = System.Guid.NewGuid().ToString();
                tempList.Add(new GameObjectIdPar(id, newGameObject));

                return id;        
            }

            return GetGameObjectID(newGameObject); 
        }



        public static void AddNewGameObject(GameObject newGameObject, DialogueScript script)
        {
            GameObject obj = GetGameObject(newGameObject);
            if (obj != null)
            {
                //DialogueUniqueId uniqueId = obj.GetComponent<DialogueUniqueId>();
                //uniqueId.UsedIn.Add(script);
            }
            else
            {
                //DialogueUniqueId uniqueId = newGameObject.AddComponent<DialogueUniqueId>();
                GameObjectIdPar par = GetTempPar(newGameObject);
                parList.Add(par);
            }
        }

        public static void RemoveGameObject(GameObject newGameObject, DialogueScript script)
        {
            GameObject obj = GetGameObject(newGameObject);
            if (obj == null) return;
            DialogueUniqueId uniqueId = obj.GetComponent<DialogueUniqueId>();
            uniqueId.UsedIn.Remove(script);
            if (uniqueId.TimesUsed == 0)
                Destroy(uniqueId); 
        }

        public static GameObject GetGameObject(string id)
        {
            foreach (GameObjectIdPar gp in parList)
            {
                if (gp.Id == id)
                    return gp.GameObject;
            }

            return null;
        }

        public static GameObject GetGameObject(GameObject gameObject)
        {
            foreach (GameObjectIdPar gp in parList)
            {
                if (gp.GameObject == gameObject)
                    return gp.GameObject;
            }

            return null;
        }

        private static GameObject CheckTempList(GameObject obj)
        {
            foreach (GameObjectIdPar gp in tempList)
            {
                if (gp.GameObject == obj)
                    return gp.GameObject;
            }

            return null;
        }
        
        private static string GetGameObjectID(GameObject gameObject)
        {
            return GetPar(gameObject).GetValueOrDefault().Id;
        }
        


        private static GameObjectIdPar? GetPar(GameObject gameObject)
        {
            foreach (GameObjectIdPar gp in parList)
            {
                if (gp.GameObject == gameObject)
                    return gp;
            }

            return null;
        }

        private static GameObjectIdPar GetTempPar(GameObject gameObject)
        {
            foreach (GameObjectIdPar gp in tempList)
            {
                if (gp.GameObject == gameObject)
                    return gp;
            }

            return default;
        }

    }
}