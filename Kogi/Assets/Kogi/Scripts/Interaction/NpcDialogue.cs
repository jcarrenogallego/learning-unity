using UnityEngine;

namespace Kogi.Scripts.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class NpcDialogue : MonoBehaviour, IInteractable
    {
        [SerializeField, TextArea]
        private string message = "Las ruinas recuerdan a quienes escuchan el viento.";

        public string Prompt => "Conversación iniciada";

        private bool isVisible;

        public void Interact()
        {
            isVisible = !isVisible;
        }

        private void OnGUI()
        {
            if (!isVisible)
            {
                return;
            }

            Rect panel = new Rect((Screen.width - 560f) * 0.5f, Screen.height - 170f, 560f, 120f);
            GUI.Box(panel, message);
            GUI.Label(new Rect(panel.x + 190f, panel.y + 80f, 220f, 25f), "Interactuar para cerrar");
        }
    }
}
