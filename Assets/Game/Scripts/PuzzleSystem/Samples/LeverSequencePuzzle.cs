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
        public bool isCompleted { get; }
        public void Enter()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void Refresh()
        {
            throw new System.NotImplementedException();
        }
    }
}