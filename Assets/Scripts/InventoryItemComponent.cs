using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model.InventoryItems;
using Assets.Scripts.Model;

public class InventoryItemComponent : MonoBehaviour
{
    private InventoryItem _item;
    private InventorySlot _slot;
    private Color _defaultTextColor = Color.black;
    private Color _fullStackTextColor = Color.black;
    private Color _emptyWeaponMagTextColor = Color.black;
    private Color _fullWeaponMagTextColor = Color.black;

    [SerializeField]
    private Image _icon;
    [SerializeField]
    private Toggle _equipToggle;
    [SerializeField]
    private TextMeshProUGUI _countText;

    public InventoryItem Item => _item;
    public InventorySlot Slot => _slot;

    private void UpdateStackCountText(int stackCount)
    {
        if (_countText != null)
        {
            _countText.color = (stackCount < _item.MaxStackCount) ? _defaultTextColor : _fullStackTextColor;
            _countText.enabled = stackCount > 1;
            _countText.text = stackCount.ToString();
        }
    }
    private void UpdateWeaponAmmoCountText(int ammoCount, int maxAmmoCount)
    {
        if (_countText != null)
        {
            _countText.enabled = true;
            _countText.color = (ammoCount == 0) ? _emptyWeaponMagTextColor : (ammoCount == maxAmmoCount) ? _fullWeaponMagTextColor : _defaultTextColor;
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

    public void Init(InventoryItem item, InventorySlot slot, Color defaultTextColor, Color fullStackTextColor, Color emptyWeaponMagTextColor, Color fullWeaponMagTextColor)
    {
        _item = item;
        _slot = slot;
        _defaultTextColor = defaultTextColor;
        _fullStackTextColor = fullStackTextColor;
        _emptyWeaponMagTextColor = emptyWeaponMagTextColor;
        _fullWeaponMagTextColor = fullWeaponMagTextColor;
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
