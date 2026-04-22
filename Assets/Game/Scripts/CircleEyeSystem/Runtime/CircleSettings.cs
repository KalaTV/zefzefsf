using UnityEngine;

[CreateAssetMenu(fileName = "CircleSettings", menuName = "Detection/CircleSettings")]
public class CircleSettings : ScriptableObject
{
    public float moveSpeed = 3f;
    public float wanderRadius = 10f;
    public float detectionRadius = 3f;
    public LayerMask playerLayer;
}
