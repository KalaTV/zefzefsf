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
    [SerializeField] private float raycastHeightOffset = 0.5f;

    private PlayerController    playerController;
    private CharacterController charController;
    private PushableBlock       currentBlock;
    private float               effortTimer = 0f;
    private bool                isGrabbing  = false;
    private bool                hasVibrated = false;
    private Vector3             grabAxis;

    // ------------------------------------------------------------------ //

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        charController   = GetComponent<CharacterController>();
    }

    // ------------------------------------------------------------------ //

    public void ToggleGrab(bool state)
    {
        if (state) TryGrab();
        else       Release();
    }

    // ------------------------------------------------------------------ //

    private void TryGrab()
    {
        Vector3 origin = transform.position + Vector3.up * raycastHeightOffset;

        // Sprite 2D flippé via localScale.x → direction du regard toujours sur X
        // On raycaste aussi sur Z pour permettre de pousser les débris de côté
        Vector3 facingDir = (transform.localScale.x > 0) ? Vector3.right : Vector3.left;

        Vector3[] directions = { facingDir, Vector3.forward, Vector3.back };

        foreach (var dir in directions)
        {
            Debug.DrawRay(origin, dir * data.interactionDistance, Color.red, 2f);

            if (Physics.Raycast(origin, dir, out RaycastHit hit, data.interactionDistance))
            {
                PushableBlock block = hit.collider.GetComponent<PushableBlock>();
                if (block != null)
                {
                    currentBlock = block;
                    grabAxis     = dir;
                    isGrabbing   = true;

                    if (playerController != null)
                        playerController.isMovementLocked = true;

                    return;
                }
            }
        }
    }

    // ------------------------------------------------------------------ //

    private void Release()
    {
        isGrabbing   = false;
        currentBlock = null;
        grabAxis     = Vector3.zero;
        effortTimer  = 0f;
        hasVibrated  = false;

        if (playerController != null)
            playerController.isMovementLocked = false;
    }

    // ------------------------------------------------------------------ //

    void Update()
    {
        if (!isGrabbing || currentBlock == null || joystick == null || data == null) return;

        Vector2 input = joystick.InputDirection;

        // Convertit l'input joystick en vecteur monde (joystick Y → monde Z)
        // puis projette sur l'axe de grab pour obtenir la magnitude de déplacement
        Vector3 inputWorld  = new Vector3(input.x, 0f, input.y);
        float moveAlongAxis = Vector3.Dot(inputWorld, grabAxis.normalized);

        if (Mathf.Abs(moveAlongAxis) > 0.1f)
        {
            effortTimer += Time.deltaTime;

            if (effortTimer >= data.startupDelay)
            {
                if (!hasVibrated)
                {
                    Handheld.Vibrate();
                    hasVibrated = true;
                }

                float speed     = (moveAlongAxis > 0) ? data.pushSpeed : data.pullSpeed;
                Vector3 moveVec = grabAxis * (moveAlongAxis * speed * Time.deltaTime);

                charController.Move(moveVec);
                currentBlock.MoveBlock(moveVec);
            }
        }
        else
        {
            effortTimer = 0f;
            hasVibrated = false;
        }
    }
}