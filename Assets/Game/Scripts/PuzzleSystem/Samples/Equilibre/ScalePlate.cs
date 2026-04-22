using UnityEngine;
using System.Collections.Generic;

namespace PuzzleSystem.Samples
{
    public class ScalePlate : MonoBehaviour
    {
        private float currentTotalWeight = 0f;
        public float CurrentTotalWeight => currentTotalWeight;
        
        private HashSet<WeightObject> objects = new HashSet<WeightObject>();

        private void OnTriggerEnter(Collider other)
        {
            WeightObject weightObject = other.GetComponent<WeightObject>();
            if (weightObject != null && !objects.Contains(weightObject))
            {
                objects.Add(weightObject);
                CalculateWeight();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            WeightObject weightObject = other.GetComponent<WeightObject>();
            if (weightObject != null && objects.Contains(weightObject))
            {
                objects.Remove(weightObject);
                CalculateWeight();
            }
        }

        private void CalculateWeight()
        {
            currentTotalWeight = 0f;
            foreach (var obj in objects)
            {
                if (obj != null) currentTotalWeight += obj.weight;
            }
            
            Debug.Log($"{gameObject.name} pèse maintenant : {currentTotalWeight}");
        }
    }
}