using UnityEngine;

[System.Serializable]
public class Responses
{
    [SerializeField] private string responseText;
    [SerializeField] public DialogueObject dialogueObject;
    
    public string ResponseText => responseText;

    public DialogueObject DialogueObject => dialogueObject;
}
