using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private Responses[] responses; 
    
    public string[] Dialogue => dialogue;

    public bool HasResponses => Responses != null && Responses.Length > 0; 
    
    public Responses[] Responses => responses;
}
