using UnityEngine;

namespace PuzzleSystem.Samples
{
    public class PlayerLiftingController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private Transform holdPoint;
        [SerializeField] private float maxReachDistance = 4f;

        private WeightObject carriedObject;

        // Cette fonction sera appelée par l'EventTrigger sur les objets
        public void OnObjectClicked(GameObject clickedObject)
        {
            float distance = Vector3.Distance(transform.position, clickedObject.transform.position);
        
            // Sécurité : on ne peut pas interagir si on est trop loin
            if (distance > maxReachDistance)
            {
                Debug.Log("Trop loin pour interagir !");
                return;
            }

            // CAS 1 : On porte déjà un objet et on clique sur une ScalePlate
            ScalePlate plate = clickedObject.GetComponent<ScalePlate>();
            if (carriedObject != null && plate != null)
            {
                DropObjectOnPlate(plate);
                return;
            }

            // CAS 2 : On ne porte rien et on clique sur un objet ramassable
            WeightObject liftable = clickedObject.GetComponent<WeightObject>();
            if (carriedObject == null && liftable != null)
            {
                PickUpObject(liftable);
                return;
            }
        }

        private void PickUpObject(WeightObject obj)
        {
            carriedObject = obj;
            carriedObject.PickUp(holdPoint);
        }

        private void DropObjectOnPlate(ScalePlate plate)
        {
            carriedObject.Drop();
        
            // Positionnement au-dessus du plateau
            Vector3 dropPos = plate.transform.position;
            dropPos.y += 1.2f; 
            carriedObject.transform.position = dropPos;

            carriedObject = null;
        }
    }
}
