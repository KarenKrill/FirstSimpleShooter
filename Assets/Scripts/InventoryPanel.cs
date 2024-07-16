using Assets.Scripts.Model;
using Assets.Scripts.Model.InventoryItems;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    [Serializable]
    public class InventoryPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private Camera _renderingCamera;
        [SerializeField]
        private GameObject _inventorySlotPrefab;
        [SerializeField]
        private GameObject _inventoryItemPrefab;
        [SerializeField]
        private GameObject _slotsParent;
        [SerializeField]
        private GameObject _itemsParent;
        [SerializeField, Min(5)]
        private float _slotSideSize = 5;
        [SerializeField, Min(0)]
        private float _horizontalMargin, _verticalMargin;
        [SerializeField, Min(1)]
        private int _columnsCount = 1;
        private Model.Inventory _inventory;
        public Model.Inventory Inventory
        {
            get => _inventory;
            set
            {
                //if (_inventory != value)
                {
                    _inventory = value;
                    RefreshInventory();
                }
            }
        }
        private bool _isInventoryMonitorStarted = false;
        private Vector3 CalcSlotPosition(int slotNumber)
        {
            Vector3 v = Vector3.zero;
            var slotsRectTransform = _slotsParent.GetComponent<RectTransform>();
            v.x = _horizontalMargin + (_horizontalMargin + _slotSideSize) * (slotNumber % _columnsCount) + slotsRectTransform.rect.position.x;
            v.y = -_verticalMargin - (_verticalMargin + _slotSideSize) * (slotNumber / _columnsCount) - slotsRectTransform.rect.position.y;
            return v;
        }
        public void RefreshInventory()
        {
            foreach (Transform slotTransform in _slotsParent.transform)
            {
                Destroy(slotTransform.gameObject);
            }
            foreach (Transform slotTransform in _itemsParent.transform)
            {
                Destroy(slotTransform.gameObject);
            }
            int slotNumber = 0;
            foreach (var slot in Inventory.ItemsSlots)
            {
                var newSlotObject = Instantiate(_inventorySlotPrefab, Vector3.zero, Quaternion.identity, _slotsParent.transform);
                var slotRectTransform = newSlotObject.GetComponent<RectTransform>();
                slotRectTransform.localPosition = CalcSlotPosition(slotNumber++);
                slotRectTransform.sizeDelta = new(_slotSideSize, _slotSideSize);
                var slotComponent = newSlotObject.GetComponent<InventorySlotComponent>();
                slotComponent.Init(slot);
                if (slot.Item != null)
                {
                    var newItemObject = Instantiate(_inventoryItemPrefab, _itemsParent.transform);
                    slotComponent.PutItem(newItemObject);
                    var itemRectTransform = newItemObject.GetComponent<RectTransform>();
                    itemRectTransform.localPosition = slotRectTransform.localPosition;
                    itemRectTransform.sizeDelta = slotRectTransform.sizeDelta;
                    var itemComponent = slotComponent.Item.GetComponent<InventoryItemComponent>();
                    itemComponent.Init(slot.Item, slot, Color.black, Color.green, Color.red, Color.green);
                }
            }
        }
        private void OnInventoryItemAdded(InventoryItem item)
        {
            RefreshInventory();
        }
        private void OnInventoryItemCountChanged(InventoryItem item)
        {
            RefreshInventory();
        }
        private void OnInventoryItemRemoved(InventoryItem obj)
        {
            RefreshInventory();
        }
        private void OnInventorySlotsCleared()
        {
            RefreshInventory();
        }
        private void OnInventoryItemsCleared()
        {
            RefreshInventory();
        }
        public void TryStartInventoryMonitorIfStopped()
        {
            if (!_isInventoryMonitorStarted && Inventory != null)
            {
                Inventory.ItemAdded += OnInventoryItemAdded;
                Inventory.ItemCountChanged += OnInventoryItemCountChanged;
                Inventory.ItemRemoved += OnInventoryItemRemoved;
                Inventory.ItemsCleared += OnInventoryItemsCleared;
                Inventory.SlotsCleared += OnInventorySlotsCleared;
                _isInventoryMonitorStarted = true;
                RefreshInventory();
            }
        }
        public void TryStopInventoryMonitorIfStarted()
        {
            if (_isInventoryMonitorStarted)
            {
                if (Inventory != null)
                {
                    Inventory.ItemAdded -= OnInventoryItemAdded;
                    Inventory.ItemCountChanged -= OnInventoryItemCountChanged;
                    Inventory.ItemRemoved -= OnInventoryItemRemoved;
                    Inventory.ItemsCleared -= OnInventoryItemsCleared;
                    Inventory.SlotsCleared -= OnInventorySlotsCleared;
                }
                _isInventoryMonitorStarted = false;
            }
        }
        public void Awake()
        {
            if (_renderingCamera == null)
            {
                var canvas = GetComponent<Canvas>();
                if (canvas == null || canvas.worldCamera == null)
                {
                    _renderingCamera = Camera.main;
                }
                else _renderingCamera = canvas.worldCamera;
            }
        }

        public void OnEnable() => TryStartInventoryMonitorIfStopped();
        public void OnDisable() => TryStopInventoryMonitorIfStarted();
        private void Update()
        {
            TryStartInventoryMonitorIfStopped();
            if (_dragItemSlot != null)
            {
                var pos = _renderingCamera.ScreenToWorldPoint(Input.mousePosition);
                _dragItemSlot.Item.transform.position = new Vector3(pos.x, pos.y, _dragItemSlot.Item.transform.position.z);
            }
        }

        // InventoryManagerEx
        private InventorySlotComponent _dragItemSlot;
        private Vector3 _startDragItemPos;
        [HideInInspector]
        public InventorySlot SelectedGameItemSlot;
        public GameObject InventoryPropertiesParentPanel;
        public GameObject InventoryPropertiesPanel;
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var clickedObj = eventData.pointerCurrentRaycast.gameObject;
                if (clickedObj != null)
                {
                    var inventorySlot = clickedObj.GetComponent<InventorySlotComponent>();
                    if (inventorySlot != null && inventorySlot.Item != null)
                    {
                        _startDragItemPos = inventorySlot.Item.transform.position;
                        _dragItemSlot = inventorySlot;
                    }
                }
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (InventoryPropertiesParentPanel.activeInHierarchy)
            {
                if (InventoryPropertiesPanel != null && eventData.pointerCurrentRaycast.gameObject != InventoryPropertiesPanel)
                {
                    InventoryPropertiesParentPanel.SetActive(false);
                    SelectedGameItemSlot = null;
                }
                return;
            }
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (_dragItemSlot != null)
                {
                    var clickedObj = eventData.pointerCurrentRaycast .gameObject;
                    if (clickedObj != null)
                    {
                        var destInventorySlot = clickedObj.GetComponent<InventorySlotComponent>();
                        if (destInventorySlot != null && _dragItemSlot != destInventorySlot)
                        {
                            _dragItemSlot.Slot.MoveTo(destInventorySlot.Slot);
                            _dragItemSlot = null;
                            RefreshInventory();
                        }
                    }
                    if (_dragItemSlot != null)
                    {
                        _dragItemSlot.Item.transform.position = _startDragItemPos;
                        _dragItemSlot = null;
                    }
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                var clickedObj = eventData.pointerCurrentRaycast.gameObject;
                var inventorySlot = clickedObj?.GetComponent<InventorySlotComponent>();
                if (inventorySlot != null && inventorySlot.Item != null)
                {
                    var inventoryItemConponent = inventorySlot.Item.GetComponent<InventoryItemComponent>();
                    if (inventoryItemConponent != null)
                    {
                        var gameSlot = inventoryItemConponent.Slot;
                        if (gameSlot != null && InventoryPropertiesParentPanel != null)
                        {
                            SelectedGameItemSlot = gameSlot;
                            InventoryPropertiesParentPanel.SetActive(true);
                        }
                    }
                }
            }
        }

        public void AddItem(InventoryItem gameItem, InventorySlot gameSlot)
        {
            foreach (Transform childTransform in _slotsParent.transform)
            {
                var slot = childTransform.gameObject.GetComponent<InventorySlotComponent>();
                if (slot != null && slot.Item == null)
                {
                    slot.Init(gameSlot);
                    var inventoryItem = Instantiate(_inventoryItemPrefab, _itemsParent.transform);
                    var ii = inventoryItem.GetComponent<InventoryItemComponent>();
                    if (ii != null)
                    {
                        ii.Init(gameItem, gameSlot, Color.black, Color.green, Color.red, Color.green);
                        slot.PutItem(inventoryItem);
                    }
                    break;
                }
            }
        }
        public void RemItem(InventorySlot gameItemSlot)
        {
            var gameItem = gameItemSlot.Item;
            if (gameItem is Weapon weapon && GameDataManager.Instance.GameData.Player.EquippedWeapon == weapon)
            {
                GameDataManager.Instance.GameData.Player.EquippedWeapon = null;
            }
            else if (gameItem is Armor armor && GameDataManager.Instance.GameData.Player.EquippedArmors.TryGetValue(armor.Type, out var equippedArmor) && equippedArmor == armor)
            {
                GameDataManager.Instance.GameData.Player.EquippedArmors.Remove(armor.Type);
            }
            foreach (Transform itemTransform in _itemsParent.transform)
            {
                if (itemTransform.gameObject != null)
                {
                    var itemComponent = itemTransform.gameObject.GetComponent<InventoryItemComponent>();
                    if (itemComponent != null)
                    {
                        var item = itemComponent.Item;
                        if (item != null && item == gameItem && itemComponent.Slot.StackCount == gameItemSlot.StackCount)
                        {
                            Destroy(itemTransform.gameObject);
                        }
                    }
                }
            }
            GameDataManager.Instance.GameData.Player.InventoryConfig.RemoveItem(gameItemSlot);
        }
        public void ClearItems()
        {
            foreach (Transform itemTransform in _itemsParent.transform)
            {
                var item = itemTransform?.gameObject?.GetComponent<InventoryItemComponent>();
                if (item != null)
                {
                    Destroy(itemTransform.gameObject);
                }
            }
            foreach (Transform childTransform in _slotsParent.transform)
            {
                var slot = childTransform?.gameObject?.GetComponent<InventorySlotComponent>();
                if (slot != null)
                {
                    slot.Item = null;
                }
            }
        }
        public List<InventoryItem> ListItems()
        {
            List<InventoryItem> gameItems = new();
            foreach (Transform childTransform in _slotsParent.transform)
            {
                var slot = childTransform.gameObject.GetComponent<InventorySlotComponent>();
                if (slot != null && slot.Item != null)
                {
                    var ii = slot.Item.GetComponent<InventoryItemComponent>();
                    if (ii != null && ii.Item != null)
                    {
                        gameItems.Add(ii.Item);
                    }
                }
            }
            return gameItems;
        }
        public void EquipWeapon(Weapon weapon)
        {
            if (GameDataManager.Instance.GameData.Player.EquippedWeapon != null)
            {
                GameDataManager.Instance.GameData.Player.EquippedWeapon.IsEquipped = false;
            }
            GameDataManager.Instance.GameData.Player.EquippedWeapon = weapon;
            weapon.IsEquipped = true;
        }
        public void EquipArmor(Armor armor)
        {
            if (GameDataManager.Instance.GameData.Player.EquippedArmors.TryGetValue(armor.Type, out var prevArmor))
            {
                prevArmor.IsEquipped = false;
            }
            GameDataManager.Instance.GameData.Player.EquippedArmors[armor.Type] = armor;
            armor.IsEquipped = true;
        }
        public void DrinkPotion(Potion potion)
        {
            GameManager.Instance.Heal(potion.RestoreValue);
        }
    }
}
