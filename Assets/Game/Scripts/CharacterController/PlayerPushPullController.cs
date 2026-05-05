using UnityEngine;
using Character.Runtime;
using PinePie.SimpleJoystick;
using FeatherSystem.Runtime;

public class PlayerPushPullController : MonoBehaviour
{
    [Header("Data & References")]
    [SerializeField] private PushPullData data; 
    [SerializeField] private JoystickController joystick;
    
    [Header("Raycast Settings")]
    [SerializeField] private float raycastHeightOffset = 1.0f; 

    private PlayerController playerController;
    private CharacterController charController;
    private PushableBlock currentBlock;
    private float effortTimer = 0f;
    private bool isGrabbing = false;
    private bool hasVibrated = false;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        charController = GetComponent<CharacterController>();
    }
    
    public void ToggleGrab(bool state)
    {
        if (state) TryGrab();
        else Release();
    }

    private void TryGrab()
    {
        Vector3 rayDirection = (transform.localScale.x > 0) ? Vector3.right : Vector3.left;
        
        Vector3 origin = transform.position; 
        
        origin.y += 0.5f; 

        Debug.DrawRay(origin, rayDirection * data.interactionDistance, Color.red, 2f);

        if (Physics.Raycast(origin, rayDirection, out RaycastHit hit, data.interactionDistance))
        {
            PushableBlock block = hit.collider.GetComponent<PushableBlock>();
            if (block != null)
            {
                currentBlock = block;
                isGrabbing = true;
                if (playerController != null) playerController.isMovementLocked = true;
            }
        }
    }

    private void Release()
    {
        isGrabbing = false;
        currentBlock = null;
        
        if (playerController != null) playerController.isMovementLocked = false;
        effortTimer = 0f;
    }

    void Update()
    {
        if (!isGrabbing || currentBlock == null || joystick == null || data == null) return;
    
        // 1. Utilise l'axe X pour la gauche/droite (le 'y' c'est pour haut/bas)
        float moveX = joystick.InputDirection.x; 

        if (Mathf.Abs(moveX) > 0.1f)
        {
            effortTimer += Time.deltaTime;
        
            if (effortTimer >= data.startupDelay)
            {
                if (!hasVibrated)
                {
                    Handheld.Vibrate();
                    hasVibrated = true;
                }
                
                float speed = (moveX > 0) ? data.pushSpeed : data.pullSpeed;
                
                Vector3 moveVector = Vector3.right * (moveX * speed);
                
                charController.Move(moveVector * Time.deltaTime);
                currentBlock.MoveBlock(moveVector * Time.deltaTime);
                
            }
        }
        else
        {
            effortTimer = 0f;
            hasVibrated = false;
        }
    }
}