using UnityEngine;

[CreateAssetMenu(fileName = "NewPushPullData", menuName = "Feather System/Push Pull Data")]
public class PushPullData : ScriptableObject
{
    [Header("Speed (m/s)")]
    public float pushSpeed = 2.2f;
    public float pullSpeed = 1.2f;
        
    [Header("Game Feel")]
    public float startupDelay = 0.5f;
    public float interactionDistance = 1.5f;
}