using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D))]
public sealed class KogiMovement : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float speed = 5f;

    private Rigidbody2D body;
    private Vector2 movementInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        body.linearVelocity = new Vector2(
            movementInput.x * speed,
            body.linearVelocity.y);
    } 
}
