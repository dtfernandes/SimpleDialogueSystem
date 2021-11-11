using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DialogueSystem;

public class DialogueNameDisplay : MonoBehaviour
{
    [SerializeField]
    private DialogueDisplayHandler ddHandler;

    private TextMeshProUGUI textComponent;

    private void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        ddHandler.onStartLine += (NodeData data) =>
        {
            if (data.PresetName != "Default")
                textComponent.text = data.PresetName;
            else
                textComponent.text = "";
        };
    }

}
