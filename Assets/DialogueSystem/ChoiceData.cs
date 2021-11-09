using System;

namespace DialogueSystem
{

    /// <summary>
    /// Class responsible for storing the choice text and
    /// its respective id
    /// </summary>
    [Serializable]
    public class ChoiceData
    {

        /// <summary>
        /// Text of a Choice Component
        /// </summary>
        [UnityEngine.SerializeField]
        private string choiceText;
        
        /// <summary>
        /// Unique Id that represents the specific Choice
        /// </summary>
        [UnityEngine.SerializeField]
        private string id;

        /// <summary>
        /// Property that defines the choice text
        /// </summary>
        public string ChoiceText => choiceText;

        /// <summary>
        /// Property that defines the unique id of the specific Choice
        /// </summary>
        public string ID => id;

        /// <summary>
        /// Constructor of this struct
        /// </summary>
        /// <param name="name">Choice text</param>
        /// <param name="id">Unique id of the Choice</param>
        public ChoiceData(string text, string id)
        {
            this.choiceText = text;
            this.id = id;
        }

        public void ChangeText(string text)
        {
            choiceText = text;
        }

        public void ChangeId(string id)
        {
            this.id = id;
        }

        public ChoiceData Clone()
        {
            return new ChoiceData(ChoiceText, ID);
        }
    }
}
