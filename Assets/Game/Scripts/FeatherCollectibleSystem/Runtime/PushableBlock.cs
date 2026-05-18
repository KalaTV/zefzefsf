using UnityEngine;

namespace FeatherSystem.Runtime
{
    public class PushableBlock : MonoBehaviour
    {
        private float lockedZ;
        

        public void MoveBlock(Vector3 movement)
        {
            Vector3 newPosition = transform.position + movement;
            
            transform.position = newPosition;
        }
    }
}