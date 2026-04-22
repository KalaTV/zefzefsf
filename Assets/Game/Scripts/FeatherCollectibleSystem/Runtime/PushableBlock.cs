using UnityEngine;

namespace FeatherSystem.Runtime.Interactables
{
    public class PushableBlock : MonoBehaviour
    {
        private float lockedZ;

        void Awake()
        {
            lockedZ = transform.position.z;
        }

        public void MoveBlock(Vector3 movement)
        {
            Vector3 newPosition = transform.position + movement;
            
            newPosition.z = lockedZ;
            
            transform.position = newPosition;
        }
    }
}