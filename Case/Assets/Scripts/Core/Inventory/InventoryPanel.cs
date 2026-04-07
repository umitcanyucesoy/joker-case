using System.Collections.Generic;
using Core.Data;
using Core.Enums;
using Event;
using Service;
using UnityEngine;

namespace Core.Inventory
{
    public class InventoryPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform slotContainer;
        [SerializeField] private InventorySlot slotPrefab;
        [SerializeField] private TileTypeRegistry tileTypeRegistry;

        [Header("Initial Slots")]
        [SerializeField] private List<TileType> initialSlotTypes = new() 
        { 
            TileType.Strawberry, 
            TileType.Apple, 
            TileType.Pear 
        };

        private readonly Dictionary<TileType, InventorySlot> _slotMap = new();
        private IInventoryService _inventoryService;

        private void OnEnable()
        {
            EventBus.Subscribe<CollectAnimationCompletedEvent>(OnCollectAnimationCompleted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<CollectAnimationCompletedEvent>(OnCollectAnimationCompleted);
        }

        public void Initialize()
        {
            _inventoryService = ServiceLocator.Get<IInventoryService>();
            CreateInitialSlots();
        }

        private void CreateInitialSlots()
        {
            foreach (var type in initialSlotTypes)
            {
                if (type == TileType.None) continue;
                
                if (!tileTypeRegistry.TryGetType(type, out var typeData)) continue;

                var slot = Instantiate(slotPrefab, slotContainer);
                slot.Setup(type, typeData.icon, _inventoryService.GetCount(type));
                _slotMap[type] = slot;
            }
        }

        public bool TryGetSlotPosition(TileType type, out Vector3 worldPosition)
        {
            if (_slotMap.TryGetValue(type, out var slot))
            {
                worldPosition = slot.RectTransform.position;
                return true;
            }

            worldPosition = Vector3.zero;
            return false;
        }

        private void OnCollectAnimationCompleted(CollectAnimationCompletedEvent evt)
        {
            _inventoryService.AddItem(evt.ItemType, evt.Count);
            
            if (_slotMap.TryGetValue(evt.ItemType, out var slot))
            {
                slot.UpdateCount(_inventoryService.GetCount(evt.ItemType));
            }
        }
    }
}

