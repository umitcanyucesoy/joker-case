using Core.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Inventory
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text countText;

        public TileType AssignedType { get; private set; }
        public RectTransform RectTransform => (RectTransform)transform;

        public void Setup(TileType type, Sprite icon, int initialCount = 0)
        {
            AssignedType = type;
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
            UpdateCount(initialCount);
        }

        public void UpdateCount(int count)
        {
            countText.text = count > 0 ? count.ToString() : "0";
        }

        public void Clear()
        {
            AssignedType = TileType.None;
            iconImage.sprite = null;
            iconImage.enabled = false;
            countText.text = "0";
        }
    }
}

