using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryPropertiesManager : MonoBehaviour
{
    public InventoryPanel InventoryPanel;
    public GameObject ClickCloseObject;
    public Image ItemImage;
    public TextMeshProUGUI ItemNameText;
    public Image ItemMainValueIcon;
    public TextMeshProUGUI ItemMainValueText;
    public Image ItemWeightValueIcon;
    public TextMeshProUGUI ItemWeightValueText;
    public TextMeshProUGUI ActionText;
    public Sprite DamageSprite;
    public Sprite HealSprite;
    public Sprite DefenceSprite;
    public Sprite WeightSprite;
    Assets.Scripts.Model.InventorySlot _selectedItemSlot;
    private void OnEnable()
    {
        _selectedItemSlot = InventoryPanel.SelectedGameItemSlot;
        if (ItemImage != null)
        {
            ItemImage.sprite = _selectedItemSlot.Item.Icon;
        }
        if (ItemNameText != null)
        {
            ItemNameText.text = _selectedItemSlot.Item.Name;
        }
        switch (_selectedItemSlot.Item)
        {
            case Assets.Scripts.Model.InventoryItems.Ammo ammo:
                if (ItemMainValueText != null)
                {
                    ItemMainValueText.text = ammo.Damage.ToString();
                }
                if (ItemMainValueIcon != null)
                {
                    ItemMainValueIcon.sprite = DamageSprite;
                }
                if (ActionText != null)
                {
                    ActionText.text = "Buy";
                }
                break;
            case Assets.Scripts.Model.InventoryItems.Armor armor:
                if (ItemMainValueText != null)
                {
                    ItemMainValueText.text = $"+{armor.Defence}";
                }
                if (ItemMainValueIcon != null)
                {
                    ItemMainValueIcon.sprite = DefenceSprite;
                }
                if (ActionText != null)
                {
                    ActionText.text = "Equip";
                }
                break;
            case Assets.Scripts.Model.InventoryItems.Potion potion:
                if (ItemMainValueText != null)
                {
                    ItemMainValueText.text = $"+{potion.RestoreValue}";
                }
                if (ItemMainValueIcon != null)
                {
                    ItemMainValueIcon.sprite = HealSprite;
                }
                if (ActionText != null)
                {
                    ActionText.text = "Drink";
                }
                break;
            case Assets.Scripts.Model.InventoryItems.Weapon weapon:
                if (ItemMainValueText != null)
                {
                    ItemMainValueText.text = weapon.Damage.ToString();
                }
                if (ItemMainValueIcon != null)
                {
                    ItemMainValueIcon.sprite = DamageSprite;
                }
                if (ActionText != null)
                {
                    ActionText.text = "Equip";
                }
                break;
            default:
                break;
        }
        if (ItemWeightValueIcon != null)
        {
            ItemWeightValueIcon.sprite = WeightSprite;
        }
        if (ItemWeightValueText != null)
        {
            ItemWeightValueText.text = $"{_selectedItemSlot.Item.Weight * _selectedItemSlot.StackCount}kg";
        }
    }
    private void OnDisable()
    {
        _selectedItemSlot = null;
    }
    public void OnUseButtonClick()
    {
        if (_selectedItemSlot != null)
        {
            switch (_selectedItemSlot.Item)
            {
                case Assets.Scripts.Model.InventoryItems.Ammo ammo:
                    _selectedItemSlot.StackCount = ammo.MaxStackCount;
                    break;
                case Assets.Scripts.Model.InventoryItems.Armor armor:
                    InventoryPanel.EquipArmor(armor);
                    break;
                case Assets.Scripts.Model.InventoryItems.Potion potion:
                    InventoryPanel.DrinkPotion(potion);
                    _selectedItemSlot.RemoveCount(1);
                    if (_selectedItemSlot.StackCount == 0)
                    {
                        _selectedItemSlot.Clear();
                    }
                    break;
                case Assets.Scripts.Model.InventoryItems.Weapon weapon:
                    InventoryPanel.EquipWeapon(weapon);
                    break;
                default:
                    break;
            }
        }
        gameObject.SetActive(false);
    }
    public void OnRemoveButtonClick()
    {
        InventoryPanel.RemItem(_selectedItemSlot.Item);
        gameObject.SetActive(false);
    }
}
