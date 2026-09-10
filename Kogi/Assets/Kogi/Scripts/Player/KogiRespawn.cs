using UnityEngine;

namespace Kogi.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class KogiRespawn : MonoBehaviour
    {
        private Rigidbody2D body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void RespawnAt(Vector2 position)
        {
            body.position = position;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }
    }
}
