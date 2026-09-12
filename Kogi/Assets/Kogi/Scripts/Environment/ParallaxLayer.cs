using UnityEngine;

namespace Kogi.Scripts.Environment
{
    public sealed class ParallaxLayer : MonoBehaviour
    {
        [SerializeField]
        private Transform cameraTransform;

        [SerializeField, Range(0f, 1f)]
        private float horizontalInfluence;

        [SerializeField, Range(0f, 1f)]
        private float verticalInfluence;

        private Vector3 initialLayerPosition;
        private Vector3 initialCameraPosition;

        private void Awake()
        {
            initialLayerPosition = transform.position;
            initialCameraPosition = cameraTransform.position;
        }

        private void LateUpdate()
        {
            Vector3 cameraMovement = cameraTransform.position - initialCameraPosition;

            transform.position = initialLayerPosition + new Vector3(
                cameraMovement.x * horizontalInfluence,
                cameraMovement.y * verticalInfluence,
                0f);
        }
    }
}
