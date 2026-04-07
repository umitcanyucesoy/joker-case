using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "CollectAnimationData", menuName = "GameData/CollectAnimationData", order = 0)]
    public class CollectAnimationData : ScriptableObject
    {
        [Header("Popup Settings")]
        public float popupDuration = 0.5f;
        public AnimationCurve popupScaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public float holdDuration = 0.3f;

        [Header("Fly To Inventory Settings")]
        public float flyDuration = 0.4f;
        public AnimationCurve flyMoveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public AnimationCurve flyScaleCurve = AnimationCurve.Linear(0f, 1f, 1f, 0.3f);

        [Header("Text Fade Settings")]
        [Tooltip("How much the text scales up during fade (0.5 = 50% bigger)")]
        public float textScaleMultiplier = 0.5f;

        [Header("Icon Size")]
        public Vector2 popupIconSize = new Vector2(100f, 100f);
        public float countFontSize = 48f;
    }
}

