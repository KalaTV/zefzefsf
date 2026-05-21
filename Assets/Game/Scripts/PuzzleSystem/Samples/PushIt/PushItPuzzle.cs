using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleSystem
{
    public class PushItPuzzle : MonoBehaviour, IPuzzle
    {

        [Header("Puzzle System")]
        [SerializeField] private PuzzleManager puzzleManager;
        [SerializeField] private string puzzleID = "push_it_01";

        [Header("Débris (dans l'ordre des manivelles)")]
        [SerializeField] private List<Debris> debrisList = new();

        [Header("Manivelles (même ordre que les débris)")]
        [SerializeField] private List<Crank> crankList = new();

        [Header("Visage rotatif")]
        [SerializeField] private FaceRotation faceRotation;
        

        public string ID          => puzzleID;
        public bool   isCompleted => _solved;

        private bool _solved       = false;
        private int  _clearedCount = 0;
        
        private void Start()
        {
            puzzleManager = FindFirstObjectByType<PuzzleManager>();
            if (puzzleManager != null)
            {
                puzzleManager.AddPuzzle(this);
            }
        }
        public void Enter()
        {
            _solved       = false;
            _clearedCount = 0;

            foreach (var debris in debrisList)
            {
                debris.OnCleared += HandleDebrisCleared;
            }

            foreach (var crank in crankList)
               
            

            Debug.Log($"[PushItPuzzle] Enter — {debrisList.Count} débris à pousser.");
        }
        
        public void Refresh()
        {
        }
        
        public void Exit()
        {
            foreach (var debris in debrisList)
                if (debris != null) debris.OnCleared -= HandleDebrisCleared;

            Debug.Log("[PushItPuzzle] Exit — puzzle terminé.");
        }

        private void HandleDebrisCleared(Debris debris)
        {
            int idx = debrisList.IndexOf(debris);
            
            if (idx >= 0 && idx < crankList.Count)
                crankList[idx].Unblock();

            _clearedCount++;
            Debug.Log($"[PushItPuzzle] Débris {idx} poussé ({_clearedCount}/{debrisList.Count}).");

            if (_clearedCount >= debrisList.Count)
                StartCoroutine(ResolvePuzzle());
        }

        private IEnumerator ResolvePuzzle()
        {

            if (faceRotation != null)
                yield return StartCoroutine(faceRotation.PlayOpenAnimation());
            
            _solved = true;
        }
        public void RegisterToPuzzleManager()
        {
            if (puzzleManager != null)
                puzzleManager.AddPuzzle(this);
        }
    }
}