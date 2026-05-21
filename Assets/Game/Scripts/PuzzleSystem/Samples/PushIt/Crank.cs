using UnityEngine;

namespace PuzzleSystem
{
    public class Crank : MonoBehaviour
    {

        private Vector3 _originPos;
        private bool    _blocked  = true;
        private bool    _shaking  = false;

        public bool IsBlocked => _blocked;
        

        private void Awake() => _originPos = transform.localPosition;

        public void Unblock()
        {
            _blocked = false;
            _shaking = false;
            transform.localPosition = _originPos;
        }
    }
}