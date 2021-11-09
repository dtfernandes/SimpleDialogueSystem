using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueUniqueId : MonoBehaviour
{
    [SerializeField]
    private string uniqueID;
    public string UniqueID { get => uniqueID; set => uniqueID = value; }
    public int TimesUsed { get => timesUsed; set => timesUsed = value; }

    [SerializeField]
    private int timesUsed;

}
