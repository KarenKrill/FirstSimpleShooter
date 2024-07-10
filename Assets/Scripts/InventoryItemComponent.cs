using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemComponent : MonoBehaviour
{
    private Assets.Scripts.Model.InventoryItems.InventoryItem _item;
    public Assets.Scripts.Model.InventoryItems.InventoryItem Item => _item;
    private Assets.Scripts.Model.InventorySlot _slot;
    public Assets.Scripts.Model.InventorySlot Slot => _slot;
    [SerializeField]
    Image _icon;
    [HideInInspector]
    int _lastStackCount = -1;
    TextMeshProUGUI _countText;
    [SerializeField]
    GameObject _countTextObject;
    Color _lastTextColor;
    bool _isEmptyWeaponColor = false;
    public Color TextColor;
    public void Init(Assets.Scripts.Model.InventoryItems.InventoryItem item, Assets.Scripts.Model.InventorySlot slot)
    {
        _item = item;
        _slot = slot;
        _lastTextColor = Color.white;
        _icon.sprite = _item.Icon;
        _countText = _countTextObject.GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        if (_countText != null)
        {
            if (_item != null && _slot != null)
            {
                if (_slot.StackCount != _lastStackCount)
                {
                    if (_lastStackCount <= 1)
                    {
                        _countText.enabled = true;
                    }
                    _lastStackCount = _slot.StackCount;
                    if (_lastStackCount <= 1)
                    {
                        _countText.enabled = false;
                    }
                    _countText.text = _lastStackCount.ToString();
                }
                if (_item is Assets.Scripts.Model.InventoryItems.Weapon weapon)
                {
                    _countText.enabled = true;
                    _isEmptyWeaponColor = true;
                    if (weapon.AmmoCount == 0)
                    {
                        _countText.color = Color.red;
                        _isEmptyWeaponColor = true;
                    }
                    else
                    {
                        _countText.color = TextColor;
                        _isEmptyWeaponColor = false;
                    }
                    _countText.text = $"{weapon.AmmoCount}/{weapon.MaxAmmoCount}";
                }
            }
            if (_countText.enabled && _lastTextColor != TextColor && !_isEmptyWeaponColor)
            {
                _lastTextColor = TextColor;
                _countText.color = TextColor;
            }
        }
    }
}
