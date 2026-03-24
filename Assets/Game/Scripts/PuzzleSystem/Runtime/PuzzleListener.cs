using System;
using UnityEngine;
using UnityEngine.Events;

namespace PuzzleSystem
{
    public class PuzzleListener : MonoBehaviour
    {
        [SerializeField] private PuzzleManager puzzleManager;
        [SerializeField] private string targetID;

        [SerializeField] private UnityEvent OnConditionMet;
        private IPuzzle puzzle;
        private void OnEnable()
        {
            if (puzzleManager != null) puzzleManager.OnComplete += HandlePuzzleFinished;
        }

        private void OnDisable()
        {
            if (puzzleManager != null) puzzleManager.OnComplete -= HandlePuzzleFinished;
        }

        private void HandlePuzzleFinished(IPuzzle puzzle)
        {
            if (puzzle.ID == targetID)
            {
                OnConditionMet?.Invoke();
            }
        }
    }
}