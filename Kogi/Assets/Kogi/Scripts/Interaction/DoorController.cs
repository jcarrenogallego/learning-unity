using UnityEngine;

namespace Kogi.Scripts.Interaction
{
    public sealed class DoorController : MonoBehaviour
    {
        [SerializeField]
        private Vector3 openOffset = new Vector3(0f, 3f, 0f);

        [SerializeField, Min(0.1f)]
        private float speed = 3f;

        public bool IsOpen { get; private set; }

        private Vector3 closedPosition;

        private void Awake()
        {
            closedPosition = transform.position;
        }

        private void Update()
        {
            Vector3 target = IsOpen ? closedPosition + openOffset : closedPosition;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
        }
    }
}
