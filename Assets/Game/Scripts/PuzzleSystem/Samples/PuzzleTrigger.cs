using UnityEngine;

namespace PuzzleSystem.samples
{
    public class PuzzleTrigger : MonoBehaviour 
    {
        [SerializeField] private PuzzleManager manager;
        private IPuzzle puzzle;

        private void Awake() 
        {
            puzzle = GetComponent<IPuzzle>();
        }

       
        public void Interact() 
        {
            Debug.Log("Interact");
            if (puzzle != null && !puzzle.isCompleted) 
            {
                manager.AddPuzzle(puzzle); 
            }
        }
    }
}