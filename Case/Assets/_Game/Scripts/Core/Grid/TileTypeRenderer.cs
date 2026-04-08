using Core.Data;
using TMPro;
using UnityEngine;

namespace Core.Grid
{
    public class TileTypeRenderer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer iconRenderer;
        [SerializeField] private TextMeshPro countText;

        [Header("Settings")]
        [SerializeField] private Vector3 iconOffset = new(0f, 0.1f, 0f);

        public void Setup(TileTypeData typeData, int count)
        {
            if (!typeData)
            {
                Hide();
                return;
            }

            gameObject.SetActive(true);

            iconRenderer.sprite = typeData.icon;
            iconRenderer.transform.localPosition = iconOffset;

            if (count > 0)
            {
                countText.gameObject.SetActive(true);
                countText.text = count.ToString();
            }
            else
                countText.gameObject.SetActive(false);
        }

        public void UpdateCount(int newCount)
        {
            if (newCount <= 0)
            {
                Hide();
                return;
            }

            countText.text = newCount.ToString();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            iconRenderer.sprite = null;
            countText.text = string.Empty;
        }
    }
}