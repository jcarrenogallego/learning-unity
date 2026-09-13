using UnityEngine;
using UnityEngine.InputSystem;

namespace Kogi.Scripts.UI
{
    public sealed class PauseController : MonoBehaviour
    {
        public static PauseController Instance { get; private set; }
        public bool IsPaused { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            if (Instance != null)
            {
                return;
            }

            GameObject controller = new GameObject("PauseController");
            DontDestroyOnLoad(controller);
            Instance = controller.AddComponent<PauseController>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            bool keyboardPressed = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
            bool gamepadPressed = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;

            if (keyboardPressed || gamepadPressed)
            {
                SetPaused(!IsPaused);
            }
        }

        public void SetPaused(bool paused)
        {
            if (GameFlowController.Instance != null && GameFlowController.Instance.IsFinished) return;
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
            AudioListener.pause = paused;
        }

        public void ClearPause()
        {
            IsPaused = false;
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }

            Time.timeScale = 1f;
            AudioListener.pause = false;
            Instance = null;
        }

        private void OnGUI()
        {
            if (!IsPaused)
            {
                return;
            }

            float width = 360f;
            float height = 180f;
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            GUI.Box(panel, string.Empty);
            GUI.Label(new Rect(panel.x + 125f, panel.y + 30f, 180f, 40f), "JUEGO EN PAUSA");

            if (GUI.Button(new Rect(panel.x + 80f, panel.y + 90f, 200f, 45f), "Continuar"))
            {
                SetPaused(false);
            }
        }
    }
}
