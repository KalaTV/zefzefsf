using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace PuzzleSystem
{
    public class PuzzleManager : MonoBehaviour
    {

        public System.Action<IPuzzle> OnComplete;
        
        List<IPuzzle> puzzles = new();


        public void AddPuzzle(IPuzzle puzzle)
        {
            puzzles.Add(puzzle);
            puzzle.Enter();

        }

        private void Update()
        {
            using (ListPool<IPuzzle>.Get(out var list))
            {
                foreach (var puzzle in puzzles)
                {
                    puzzle.Refresh();
                    if (puzzle.isCompleted)
                    {
                        list.Add(puzzle);
                    }
                }

                foreach (var puzzle in list)
                {
                    puzzle.Exit();
                    puzzles.Remove(puzzle);
                    OnComplete?.Invoke(puzzle);
                }
            }
        }
    }

}