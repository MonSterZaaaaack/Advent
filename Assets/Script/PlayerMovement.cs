using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputAction moveAction;
    public float speed;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        PlayerMovementControl();
    }
    void PlayerMovementControl()
    {
        Vector2 moveDirection = moveAction.ReadValue<Vector2>().normalized;
        if (moveDirection != Vector2.zero)
        {
            Vector3 moveVelocity = new Vector3(moveDirection.x, 0, moveDirection.y);
            moveVelocity *= speed;
            rb.linearVelocity = moveVelocity;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}
