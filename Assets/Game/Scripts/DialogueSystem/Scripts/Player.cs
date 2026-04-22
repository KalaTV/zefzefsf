using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    
    [SerializeField] private float moveSpeed = 10f;
    
    [SerializeField] private InputActionAsset input;
    
    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }
    
    private Rigidbody2D rb;
    private Vector2 inputDirection;
    
    
    private void  Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void HandleMovement()
    {
        inputDirection = input.FindAction("Move").ReadValue<Vector2>();
    }

    private void ApplyMovement()
    {
        Vector2 force = inputDirection * moveSpeed;
        rb.MovePosition(rb.position + force * Time.deltaTime);
    }

    private void Update()
    {
        if (dialogueUI.IsOpen) return;
        
        HandleMovement();
        
        if (Keyboard.current.eKey.wasPressedThisFrame)
        { 
            Interactable?.Interact(this);
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }
}
