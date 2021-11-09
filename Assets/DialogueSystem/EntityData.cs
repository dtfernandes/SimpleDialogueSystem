using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DialogueSystem
{
    public class EntityData : ScriptableObject
    {
        public List<string> presetNames => CreateNamesList();

        private List<string> CreateNamesList()
        {
            List<string> returnList =
                data.Select(x => x.EntityName).ToList();
            returnList.Insert(0, "Default");

            return returnList;
        }


        [SerializeField]
        public List<EntityInfo> data;

        private EntityInfo defaultPreset;

        public void AddNewPreset()
        {
            data.Add(new EntityInfo());
        }

    }
}