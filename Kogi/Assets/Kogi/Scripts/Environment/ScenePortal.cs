using System.Collections;
using Kogi.Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kogi.Scripts.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class ScenePortal : MonoBehaviour
    {
        [SerializeField]
        private string targetScene = "SantuarioPrueba";

        [SerializeField, Min(0f)]
        private float transitionDuration = 0.35f;

        private bool isLoading;
        private float overlayAlpha;

        public void Configure(string sceneName)
        {
            targetScene = sceneName;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isLoading || !other.TryGetComponent(out KogiLives _))
            {
                return;
            }

            StartCoroutine(LoadTargetScene());
        }

        private IEnumerator LoadTargetScene()
        {
            isLoading = true;
            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                overlayAlpha = transitionDuration <= 0f ? 1f : elapsed / transitionDuration;
                yield return null;
            }

            Time.timeScale = 1f;
            AudioListener.pause = false;
            SceneManager.LoadScene(targetScene);
        }

        private void OnGUI()
        {
            if (!isLoading)
            {
                return;
            }

            Color previous = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, Mathf.Clamp01(overlayAlpha));
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = previous;
        }
    }
}
