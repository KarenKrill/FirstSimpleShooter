using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public GameItem GameItem;
    [SerializeField]
    Image _icon;
    [HideInInspector]
    uint _lastStackCount = uint.MaxValue;
    TextMeshProUGUI _countText;
    [SerializeField]
    GameObject _countTextObject;
    Color _lastTextColor;
    bool _isEmptyWeaponColor = false;
    public Color TextColor;
    private void Start()
    {
        _lastTextColor = Color.white;
        _icon.sprite = GameItem.Icon;
        _countText = _countTextObject.GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        if (_countText != null)
        {
            if (GameItem != null)
            {
                if (GameItem.Stats.StackCount != _lastStackCount)
                {
                    if (_lastStackCount <= 1)
                    {
                        _countText.enabled = true;
                    }
                    _lastStackCount = GameItem.Stats.StackCount;
                    if (_lastStackCount <= 1)
                    {
                        _countText.enabled = false;
                    }
                    _countText.text = _lastStackCount.ToString();
                }
                if (GameItem is Weapon weapon)
                {
                    _countText.enabled = true;
                    _isEmptyWeaponColor = true;
                    if (weapon.WeaponStats.AmmoCount == 0)
                    {
                        _countText.color = Color.red;
                        _isEmptyWeaponColor = true;
                    }
                    else
                    {
                        _countText.color = TextColor;
                        _isEmptyWeaponColor = false;
                    }
                    _countText.text = $"{weapon.WeaponStats.AmmoCount}/{weapon.WeaponStats.MaxAmmoCount}";
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
