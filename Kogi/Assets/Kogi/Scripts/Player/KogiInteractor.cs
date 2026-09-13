using Kogi.Scripts.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using Kogi.Scripts.UI;

namespace Kogi.Scripts.Player
{
    public sealed class KogiInteractor : MonoBehaviour
    {
        [SerializeField]
        private Transform interactionOrigin;

        [SerializeField, Min(0.1f)]
        private float interactionRadius = 1.2f;

        public void Configure(Transform origin, float radius)
        {
            interactionOrigin = origin;
            interactionRadius = radius;
        }

        private void OnInteract(InputValue value)
        {
            if (!value.isPressed || Time.timeScale == 0f ||
                (GameFlowController.Instance != null && GameFlowController.Instance.IsFinished)) return;
            Vector2 origin = interactionOrigin != null ? interactionOrigin.position : transform.position;
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, interactionRadius);

            foreach (Collider2D hit in hits)
            {
                IInteractable interactable = hit.GetComponentInParent<IInteractable>();

                if (interactable == null)
                {
                    continue;
                }

                interactable.Interact();
                Debug.Log(interactable.Prompt);
                return;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = interactionOrigin != null ? interactionOrigin.position : transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin, interactionRadius);
        }
    }
}
