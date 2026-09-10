using Kogi.Scripts.Player;
using TMPro;
using UnityEngine;

namespace Kogi.Scripts.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class LivesView : MonoBehaviour
    {
        [SerializeField]
        private KogiLives lives;

        private TMP_Text label;

        private void Awake()
        {
            label = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            lives.LivesChanged += Refresh;
        }

        private void Start()
        {
            Refresh(lives.CurrentLives);
        }

        private void OnDisable()
        {
            lives.LivesChanged -= Refresh;
        }

        private void Refresh(int currentLives)
        {
            label.text = $"Vidas: {currentLives}";
        }
    }
}
