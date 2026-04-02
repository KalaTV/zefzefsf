using UnityEngine;

public class FloatingArrow : MonoBehaviour 
{
    void Update() {
        transform.localPosition = new Vector3(0, Mathf.Sin(Time.time * 3f) * 0.2f, 0);
    }
}