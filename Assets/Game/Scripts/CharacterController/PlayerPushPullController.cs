using UnityEngine;
using Character.Runtime;
using PinePie.SimpleJoystick;
using FeatherSystem.Runtime.Interactables;

public class PlayerPushPullController : MonoBehaviour
{
    [Header("Settings")]
    public float pushSpeed = 2.2f;
    public float pullSpeed = 1.2f;
    public float interactionDistance = 1.5f;
    public float startupDelay = 0.5f;

    [SerializeField] private JoystickController joystick;
    
    private PlayerController playerController;
    private CharacterController charController;
    private PushableBlock currentBlock;
    private float effortTimer = 0f;
    private bool isGrabbing = false;

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
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance))
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
        if (!isGrabbing || currentBlock == null || joystick == null) return;
        
        float moveX = joystick.InputDirection.x;

        if (Mathf.Abs(moveX) > 0.1f)
        {
            effortTimer += Time.deltaTime;
            
            if (effortTimer >= startupDelay)
            {
                float dot = Vector3.Dot(transform.forward, new Vector3(moveX, 0, 0));
                float speed = (dot > 0) ? pushSpeed : pullSpeed;

                Vector3 moveVector = new Vector3(moveX * speed, 0, 0);
                
                charController.Move(moveVector * Time.deltaTime);
                currentBlock.MoveBlock(moveVector * Time.deltaTime);

                Handheld.Vibrate();
            }
        }
        else
        {
            effortTimer = 0f;
        }
    }
}