using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryPropertiesManager : MonoBehaviour
{
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
    GameItem _selectedItem;
    private void OnEnable()
    {
        _selectedItem = InventoryManager.Instance.SelectedGameItem;
        if (ItemImage != null)
        {
            ItemImage.sprite = _selectedItem.Icon;
        }
        if (ItemNameText != null)
        {
            ItemNameText.text = _selectedItem.Stats.Name;
        }
        switch (_selectedItem)
        {
            case Ammo ammo:
                if (ItemMainValueText != null)
                {
                    ItemMainValueText.text = ammo.AmmoStats.Damage.ToString();
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
            case Armor armor:
                if (ItemMainValueText != null)
                {
                    ItemMainValueText.text = $"+{armor.ArmorStats.Defence}";
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
            case Potion potion:
                if (ItemMainValueText != null)
                {
                    ItemMainValueText.text = $"+{potion.PotionStats.RestoreValue}";
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
            case Weapon weapon:
                if (ItemMainValueText != null)
                {
                    ItemMainValueText.text = weapon.WeaponStats.Damage.ToString();
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
            ItemWeightValueText.text = $"{_selectedItem.Stats.Weight * _selectedItem.Stats.StackCount}kg";
        }
    }
    private void OnDisable()
    {
        _selectedItem = null;
    }
    public void OnUseButtonClick()
    {
        if (_selectedItem != null)
        {
            switch (_selectedItem)
            {
                case Ammo ammo:
                    ammo.Stats.StackCount = ammo.Stats.MaxStackCount;
                    break;
                case Armor armor:
                    InventoryManager.Instance.EquipArmor(armor);
                    break;
                case Potion potion:
                    InventoryManager.Instance.DrinkPotion(potion);
                    break;
                case Weapon weapon:
                    InventoryManager.Instance.EquipWeapon(weapon);
                    break;
                default:
                    break;
            }
        }
        gameObject.SetActive(false);
    }
    public void OnRemoveButtonClick()
    {
        InventoryManager.Instance.RemItem(_selectedItem);
        gameObject.SetActive(false);
    }
}
