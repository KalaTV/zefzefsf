using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleSystem.samples
{
    public class LeverSequencePuzzle : MonoBehaviour, IPuzzle
    {
        [Header("Puzzle Settings")]
        [SerializeField] private string _id = "LeversAndGrid_01";
        public string ID => _id;
        
        [SerializeField] private List<Lever> leverSequence; 
        [SerializeField] private Lever finalLever;
        private bool _isCompleted;
        public bool isCompleted => _isCompleted;
        
        private PuzzleManager manager;

        private void Start()
        {
            manager = FindFirstObjectByType<PuzzleManager>();
            if (manager != null)
            {
                manager.AddPuzzle(this);
            }
        }

        public void Enter()
        {
            Debug.Log($"Début du puzzle : {ID}");
            _isCompleted = false;
        }

        public void Exit()
        {
            Debug.Log($"Puzzle {ID} terminé avec succès !");
        }

        public void Refresh()
        {
            if (_isCompleted) return;
                     
            if (finalLever != null && finalLever.IsActivated) 
            {
                _isCompleted = true;
            }
        }
    }
}