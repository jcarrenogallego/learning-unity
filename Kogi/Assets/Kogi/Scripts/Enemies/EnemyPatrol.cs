using UnityEngine;

namespace Kogi.Scripts.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class EnemyPatrol : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        private float speed = 2f;

        [SerializeField, Min(0f)]
        private float patrolDistance = 2f;

        private Rigidbody2D body;
        private SpriteRenderer characterRenderer;
        private float startingPositionX;
        private int direction = 1;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            characterRenderer = GetComponent<SpriteRenderer>();
            startingPositionX = body.position.x;
        }

        private void FixedUpdate()
        {
            float nextPositionX = body.position.x + direction * speed * Time.fixedDeltaTime;
            float leftLimit = startingPositionX - patrolDistance;
            float rightLimit = startingPositionX + patrolDistance;

            if (nextPositionX >= rightLimit)
            {
                nextPositionX = rightLimit;
                ChangeDirection(-1);
            }
            else if (nextPositionX <= leftLimit)
            {
                nextPositionX = leftLimit;
                ChangeDirection(1);
            }

            body.MovePosition(new Vector2(nextPositionX, body.position.y));
        }

        private void ChangeDirection(int newDirection)
        {
            direction = newDirection;
            characterRenderer.flipX = direction < 0;
        }
    }
}
