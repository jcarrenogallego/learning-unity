using Kogi.Scripts.Environment;
using UnityEngine;

namespace Kogi.Scripts.UI
{
    public sealed class CollectibleView : MonoBehaviour
    {
        private int count;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Create()
        {
            if (FindAnyObjectByType<CollectibleView>() == null)
            {
                new GameObject("CollectibleView").AddComponent<CollectibleView>();
            }
        }

        private void OnEnable()
        {
            count = CollectibleProgress.Count;
            CollectibleProgress.CountChanged += HandleCountChanged;
        }

        private void OnDisable()
        {
            CollectibleProgress.CountChanged -= HandleCountChanged;
        }

        private void HandleCountChanged(int newCount)
        {
            count = newCount;
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(24f, 70f, 220f, 32f), $"Reliquias: {count}");
        }
    }
}
