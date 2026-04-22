using UnityEngine;

namespace PuzzleSystem.Samples
{
    public class WeightObject : MonoBehaviour
    {
        private Rigidbody rb;
        [SerializeField]private Collider col;
        public float weight = 1f;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        
        public void PickUp(Transform holdPoint)
        {
            rb.isKinematic = true; 
            
            col.enabled = false;   
            
            transform.position = holdPoint.position;
            transform.SetParent(holdPoint);
        }

        public void Drop()
        {
            transform.SetParent(null);
            
          
            rb.isKinematic = false; 
            col.enabled = true;
        }
    }
}
