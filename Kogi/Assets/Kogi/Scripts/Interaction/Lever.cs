using UnityEngine;

namespace Kogi.Scripts.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Lever : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private DoorController controlledDoor;

        public string Prompt => "Palanca accionada";

        public void Configure(DoorController door)
        {
            controlledDoor = door;
        }

        public void Interact()
        {
            controlledDoor?.Toggle();
            transform.localRotation = Quaternion.Euler(0f, 0f, controlledDoor != null && controlledDoor.IsOpen ? -30f : 30f);
        }
    }
}
