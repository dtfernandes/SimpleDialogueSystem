using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public class EntityInfo
    {
        [SerializeField]
        string entityName;
        [SerializeField]
        Sprite entitySprite;
        bool hidden;
   
        public string EntityName { get => entityName; set => entityName = value; }
        public Sprite EntitySprite { get => entitySprite; set => entitySprite = value; }
        public bool Hidden { get => hidden; set => hidden = value; }
                    
    }
}