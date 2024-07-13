using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model.InventoryItems;
using Assets.Scripts.Model;

public class InventoryItemComponent : MonoBehaviour
{
    private InventoryItem _item;
    private InventorySlot _slot;
    private bool _isEmptyWeaponColor = false;
    private Color _textColor = Color.black;

    [SerializeField]
    private Image _icon;
    [SerializeField]
    private Toggle _equipToggle;
    [SerializeField]
    private TextMeshProUGUI _countText;

    public InventoryItem Item => _item;
    public InventorySlot Slot => _slot;
    public Color TextColor
    {
        get => _textColor;
        set
        {
            _textColor = value;
            if (_countText != null && !_isEmptyWeaponColor)
            {
                _countText.color = value;
            }
        }
    }

    private void UpdateStackCountText(int stackCount)
    {
        if (_countText != null)
        {
            _countText.color = _textColor;
            _countText.enabled = stackCount > 1;
            _countText.text = stackCount.ToString();
        }
    }
    private void UpdateWeaponAmmoCountText(int ammoCount, int maxAmmoCount)
    {
        if (_countText != null)
        {
            _countText.enabled = true;
            if (ammoCount == 0)
            {
                _countText.color = Color.red;
                _isEmptyWeaponColor = true;
            }
            else
            {
                _countText.color = _textColor;
                _isEmptyWeaponColor = false;
            }
            _countText.text = $"{ammoCount}/{maxAmmoCount}";
        }
    }
    private void UpdateEquipState(bool state)
    {
        if (_equipToggle != null)
        {
            _equipToggle.isOn = state;
        }
    }
    private void OnStackCountChanged(InventorySlot slot) => UpdateStackCountText(slot.StackCount);
    private void OnWeaponAmmoCountChanged(Weapon weapon) => UpdateWeaponAmmoCountText(weapon.AmmoCount, weapon.MaxAmmoCount);
    private void OnEquipStateChanged(bool state) => UpdateEquipState(state);
    private void SubscribeOnEvents()
    {
        bool isWeapon = false;
        if (_item != null)
        {
            _item.IsEquippedChanged += OnEquipStateChanged;
            if (_item is Weapon weapon)
            {
                isWeapon = true;
                weapon.AmmoCountChanged += OnWeaponAmmoCountChanged;
            }
        }
        if (_slot != null && !isWeapon)
        {
            _slot.StackCountChanged += OnStackCountChanged;
        }
    }
    private void UnsubscribeFromEvents()
    {
        bool isWeapon = false;
        if (_item != null)
        {
            _item.IsEquippedChanged -= OnEquipStateChanged;
            if (_item is Weapon weapon)
            {
                isWeapon = true;
                weapon.AmmoCountChanged -= OnWeaponAmmoCountChanged;
            }
        }
        if (_slot != null && !isWeapon)
        {
            _slot.StackCountChanged -= OnStackCountChanged;
        }
    }
    private void OnEnable() => SubscribeOnEvents();
    private void OnDisable() => UnsubscribeFromEvents();

    public void Init(InventoryItem item, InventorySlot slot)
    {
        _item = item;
        _slot = slot;
        bool isWeapon = false;
        if (_item != null)
        {
            _icon.sprite = _item.Icon;
            UpdateEquipState(_item.IsEquipped);
            if (_item is Weapon weapon)
            {
                isWeapon = true;
                UpdateWeaponAmmoCountText(weapon.AmmoCount, weapon.MaxAmmoCount);
            }
        }
        if (slot != null && !isWeapon)
        {
            UpdateStackCountText(_slot.StackCount);
        }
        SubscribeOnEvents();
    }
}
