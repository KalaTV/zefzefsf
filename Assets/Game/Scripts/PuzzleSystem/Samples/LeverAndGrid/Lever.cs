using UnityEngine;
using UnityEngine.Events;

namespace PuzzleSystem.samples
{
    public class Lever : MonoBehaviour
    {
        public bool IsActivated { get; private set; }
        
        [SerializeField] private UnityEvent OnActivated;
        [SerializeField] private GameObject visualOn;
        [SerializeField] private GameObject visualOff;
        
        public void ToggleLever()
        {
            if (IsActivated) return;

            IsActivated = true;
            
            if (visualOn) visualOn.SetActive(true);
            if (visualOff) visualOff.SetActive(false);
            
            OnActivated?.Invoke();
            Debug.Log($"{gameObject.name} activé !");
        }
        
        private void OnMouseDown() 
        {
            
            ToggleLever();
        }
    }
}