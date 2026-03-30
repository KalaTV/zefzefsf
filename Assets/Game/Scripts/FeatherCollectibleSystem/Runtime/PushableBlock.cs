using UnityEngine;

namespace FeatherSystem.Runtime.Interactables
{
    public class PushableBlock : MonoBehaviour
    {
        public void MoveBlock(Vector3 movement)
        {
            transform.position += movement;
        }
    }
}