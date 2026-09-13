using Kogi.Scripts.Player;
using UnityEngine;

namespace Kogi.Scripts.Enemies
{
    public sealed class EnemyContactDamage : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (!collision.gameObject.TryGetComponent(out KogiDamageReceiver receiver))
            {
                return;
            }

            receiver.ReceiveHit();
        }
    }
}
