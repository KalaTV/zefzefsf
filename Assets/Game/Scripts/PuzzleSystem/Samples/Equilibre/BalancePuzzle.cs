using UnityEngine;

namespace PuzzleSystem.Samples
{
    public class BalancePuzzle : MonoBehaviour, IPuzzle
    {
        [Header("Puzzle Settings")]
        [SerializeField] private string _id = "Balance_01";
        public string ID => _id;

        [Header("Balance References")]
        [SerializeField] private ScalePlate featherPlate; 
        [SerializeField] private ScalePlate rockPlate;    
        
        [Tooltip("Marge d'erreur tolérée pour l'équilibre parfait")]
        [SerializeField] private float tolerance = 0.1f; 

        [Header("Feedback Visuel")]
        [SerializeField] private Transform balanceBeam; 
        [SerializeField] private float maxTiltAngle = 30f; 
        [SerializeField] private float tiltSpeed = 5f;

        private bool _isCompleted;
        public bool isCompleted => _isCompleted;

        private PuzzleManager manager;

        private void Start()
        {
            manager = FindFirstObjectByType<PuzzleManager>();
            if (manager != null) manager.AddPuzzle(this);
        }

        public void Enter()
        {
            _isCompleted = false;
        }

        public void Refresh()
        {
            if (_isCompleted) return;
            
            float difference = rockPlate.CurrentTotalWeight - featherPlate.CurrentTotalWeight;
            
            UpdateVisualFeedback(difference);
            
            if (Mathf.Abs(difference) <= tolerance)
            {
                if (featherPlate.CurrentTotalWeight > 0)
                {
                    _isCompleted = true;
                }
            }
        }

        private void UpdateVisualFeedback(float weightDifference)
        {
            if (balanceBeam == null) return;
            
            float targetAngle = Mathf.Clamp(weightDifference * 5f, -maxTiltAngle, maxTiltAngle);
            
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            balanceBeam.rotation = Quaternion.Lerp(balanceBeam.rotation, targetRotation, Time.deltaTime * tiltSpeed);
        }

        public void Exit()
        {
            Debug.Log($"Puzzle {ID} (L'Équilibre) résolu !");
            
            if (balanceBeam != null) 
                balanceBeam.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}