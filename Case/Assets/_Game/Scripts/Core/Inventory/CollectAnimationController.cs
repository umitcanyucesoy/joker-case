using System.Collections;
using Core.Data;
using Core.Enums;
using Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Inventory
{
    public class CollectAnimationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryPanel inventoryPanel;
        [SerializeField] private CollectAnimationData animationData;

        [Header("Popup Elements")]
        [SerializeField] private RectTransform popupContainer;
        [SerializeField] private Image popupIcon;
        [SerializeField] private TMP_Text popupCountText;
        [SerializeField] private CanvasGroup textCanvasGroup;

        private RectTransform _iconRect;
        private RectTransform _textRect;
        private Vector3 _iconStartLocalPos;
        private Vector3 _textStartLocalPos;

        private void Start()
        {
            _iconRect = popupIcon.rectTransform;
            _textRect = popupCountText.rectTransform;
            _iconStartLocalPos = _iconRect.localPosition;
            _textStartLocalPos = _textRect.localPosition;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<ItemCollectedEvent>(OnItemCollected);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ItemCollectedEvent>(OnItemCollected);
        }

        private void OnItemCollected(ItemCollectedEvent evt)
        {
            if (evt.TypeData == null || evt.Count <= 0) return;
            StartCoroutine(PlayCollectAnimation(evt.ItemType, evt.TypeData, evt.Count));
        }

        private IEnumerator PlayCollectAnimation(TileType itemType, TileTypeData typeData, int count)
        {
            popupIcon.sprite = typeData.icon;
            popupCountText.text = $"+{count}";
            
            _iconRect.sizeDelta = animationData.popupIconSize;
            popupCountText.fontSize = animationData.countFontSize;
            
            popupContainer.gameObject.SetActive(true);
            
            ResetElements();
            var screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
            popupContainer.position = screenCenter;
            popupContainer.localScale = Vector3.zero;

            var elapsed = 0f;
            while (elapsed < animationData.popupDuration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / animationData.popupDuration);
                var scale = animationData.popupScaleCurve.Evaluate(t);
                popupContainer.localScale = Vector3.one * scale;
                yield return null;
            }
            popupContainer.localScale = Vector3.one;

            yield return new WaitForSeconds(animationData.holdDuration);
            if (!inventoryPanel.TryGetSlotPosition(itemType, out var targetPosition))
            {
                popupContainer.gameObject.SetActive(false);
                PublishCompletion(itemType, count);
                yield break;
            }

            var iconStartPos = _iconRect.position;
            elapsed = 0f;

            while (elapsed < animationData.flyDuration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / animationData.flyDuration);

                var moveT = animationData.flyMoveCurve.Evaluate(t);
                _iconRect.position = Vector3.Lerp(iconStartPos, targetPosition, moveT);

                var scaleT = animationData.flyScaleCurve.Evaluate(t);
                _iconRect.localScale = Vector3.one * scaleT;

                var textScale = 1f + t * animationData.textScaleMultiplier;
                _textRect.localScale = Vector3.one * textScale;
                textCanvasGroup.alpha = 1f - t;

                yield return null;
            }

            popupContainer.gameObject.SetActive(false);
            ResetElements();

            PublishCompletion(itemType, count);
        }

        private void ResetElements()
        {
            popupContainer.localScale = Vector3.one;
            _iconRect.localScale = Vector3.one;
            _iconRect.localPosition = _iconStartLocalPos;
            _iconRect.sizeDelta = animationData.popupIconSize;
            _textRect.localScale = Vector3.one;
            _textRect.localPosition = _textStartLocalPos;
            textCanvasGroup.alpha = 1f;
        }

        private void PublishCompletion(TileType itemType, int count)
        {
            EventBus.Publish(new CollectAnimationCompletedEvent
            {
                ItemType = itemType,
                Count = count
            });
        }
    }
}


