using UnityEngine;

namespace Kogi.Scripts.Player.Visual
{
    [CreateAssetMenu(menuName = "Kogi/Visual/Rig Parts", fileName = "KogiRigParts")]
    public sealed class KogiRigParts : ScriptableObject
    {
        public Sprite head;
        public Sprite torso;
        public Sprite upperArmBack;
        public Sprite lowerArmBack;
        public Sprite upperArmFront;
        public Sprite lowerArmFront;
        public Sprite upperLegBack;
        public Sprite lowerLegBack;
        public Sprite upperLegFront;
        public Sprite lowerLegFront;
        public Sprite scarf;
    }
}
