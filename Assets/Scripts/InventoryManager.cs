using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static InventoryManager Instance;
    public GameObject SlotsParent;
    public GameObject ItemsParent;
    public GameObject HotBarSlotsParent;
    public GameObject InventoryPropertiesParentPanel;
    public GameObject InventoryPropertiesPanel;
    //[HideInInspector]
    public Weapon EquippedWeapon;
    //[HideInInspector]
    public Dictionary<ArmorType, Armor> EquippedArmor = new();
    [HideInInspector]
    public GameItem SelectedGameItem;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            var clickedObj = eventData.pointerCurrentRaycast.gameObject;
            var inventorySlot = clickedObj?.GetComponent<InventorySlot>();
            if (inventorySlot != null && inventorySlot.Item != null)
            {
                _startDragItemPos = inventorySlot.Item.transform.position;
                _dragItemSlot = inventorySlot;
                Debug.Log($"PointerDown on {inventorySlot}");
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
                SelectedGameItem = null;
            }
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_dragItemSlot != null)
            {
                var clickedObj = eventData.pointerCurrentRaycast.gameObject;
                var destInventorySlot = clickedObj?.GetComponent<InventorySlot>();
                if (destInventorySlot != null && _dragItemSlot != destInventorySlot)
                {
                    if (destInventorySlot.Item != null)
                    {
                        var destInventoryItemParent = destInventorySlot.Item;
                        var destGameItem = destInventoryItemParent.GetComponent<InventoryItem>()?.GameItem;
                        var sourceGameItem = _dragItemSlot.Item.GetComponent<InventoryItem>()?.GameItem;
                        if (destGameItem.Name == sourceGameItem.Name && destGameItem.StackCount < destGameItem.MaxStackCount)
                        {
                            uint availableDestItemsCount = destGameItem.MaxStackCount - destGameItem.StackCount;
                            uint moveItemsCount = sourceGameItem.StackCount < availableDestItemsCount ? sourceGameItem.StackCount : availableDestItemsCount;
                            destGameItem.StackCount += moveItemsCount;
                            sourceGameItem.StackCount -= moveItemsCount;
                            if (sourceGameItem.StackCount == 0)
                            {
                                Destroy(_dragItemSlot.Item);
                            }
                            else
                            {
                                _dragItemSlot.PutItem(_dragItemSlot.Item);
                            }
                        }
                        else
                        {
                            if (destGameItem is Weapon weapon && sourceGameItem is Ammo ammo && ammo.WeaponName == weapon.Name && weapon.AmmoCount < weapon.MaxAmmoCount)
                            {
                                var missingAmmoCount = weapon.MaxAmmoCount - weapon.AmmoCount;
                                uint ammoCount = ammo.StackCount > missingAmmoCount ? missingAmmoCount : ammo.StackCount;
                                weapon.AmmoCount += ammoCount;
                                if (ammoCount < ammo.StackCount)
                                {
                                    ammo.StackCount -= ammoCount;
                                    _dragItemSlot.PutItem(_dragItemSlot.Item);
                                }
                                else
                                {
                                    Destroy(_dragItemSlot.Item);
                                }
                            }
                            else
                            {
                                // swap:
                                destInventorySlot.PutItem(_dragItemSlot.Item);
                                _dragItemSlot.PutItem(destInventoryItemParent);
                            }
                        }
                    }
                    else
                    {
                        destInventorySlot.PutItem(_dragItemSlot.Item);
                        _dragItemSlot.Item = null;
                    }
                    _dragItemSlot = null;
                    Debug.Log($"PointerUp on {destInventorySlot}");
                }
            }
            if (_dragItemSlot != null)
            {
                _dragItemSlot.Item.transform.position = _startDragItemPos;
                _dragItemSlot = null;
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            var clickedObj = eventData.pointerCurrentRaycast.gameObject;
            var inventorySlot = clickedObj?.GetComponent<InventorySlot>();
            if (inventorySlot != null)
            {
                var gameItem = inventorySlot.Item?.GetComponent<InventoryItem>()?.GameItem;
                if (gameItem != null && InventoryPropertiesParentPanel != null)
                {
                    SelectedGameItem = gameItem;
                    InventoryPropertiesParentPanel.SetActive(true);
                }
            }
        }
    }

    private InventorySlot _dragItemSlot;
    private Vector3 _startDragItemPos;
    private Camera _canvasCamera;
    public GameObject InventoryItemPrefub;
    private List<(string, GameItem)> _items;

    void Awake()
    {
        Instance = this;
        var canvas = GetComponent<Canvas>();
        if (canvas == null || canvas.worldCamera == null)
        {
            _canvasCamera = Camera.main;
        }
        else _canvasCamera = canvas.worldCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if (_dragItemSlot != null)
        {
            var pos = _canvasCamera.ScreenToWorldPoint(Input.mousePosition);
            _dragItemSlot.Item.transform.position = new Vector3(pos.x, pos.y, _dragItemSlot.Item.transform.position.z);
        }
    }
    public void AddItem(GameItem gameItem)
    {
        foreach (Transform childTransform in SlotsParent.transform)
        {
            var slot = childTransform.gameObject.GetComponent<InventorySlot>();
            if (slot != null && slot.Item == null)
            {
                var inventoryItem = Instantiate(InventoryItemPrefub, ItemsParent.transform);
                var ii = inventoryItem.GetComponent<InventoryItem>();
                if (ii != null)
                {
                    ii.GameItem = gameItem;
                    ii.TextColor = Color.black;
                    slot.PutItem(inventoryItem);
                    //_items.Add((gameItem.Name, gameItem));
                } // else exception
                break;
            }
        }
    }
    public void RemItem(GameItem gameItem)
    {
        foreach (Transform itemTransform in ItemsParent.transform)
        {
            var item = itemTransform?.gameObject?.GetComponent<InventoryItem>()?.GameItem;
            if (item != null && item == gameItem)
            {
                Destroy(itemTransform.gameObject);
            }
        }
    }
    public void ClearItems()
    {
        foreach (Transform itemTransform in ItemsParent.transform)
        {
            var item = itemTransform?.gameObject?.GetComponent<InventoryItem>();
            if (item != null)
            {
                Destroy(itemTransform.gameObject);
            }
        }
        foreach (Transform childTransform in SlotsParent.transform)
        {
            var slot = childTransform?.gameObject?.GetComponent<InventorySlot>();
            if (slot != null)
            {
                slot.Item = null;
            }
        }
    }
    public void EquipWeapon(Weapon weapon)
    {
        EquippedWeapon = weapon;
    }
    public void EquipArmor(Armor armor)
    {
        EquippedArmor[armor.Type] = armor;
    }
    public void DrinkPotion(Potion potion)
    {
        GameManager.Instance.Heal(potion.RestoreValue);
        RemItem(potion);
    }
}
