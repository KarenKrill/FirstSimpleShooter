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
    int _lastStackCount = -1;
    TextMeshProUGUI _countText;
    [SerializeField]
    GameObject _countTextObject;
    Color _lastTextColor;
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
                if (GameItem.StackCount != _lastStackCount)
                {
                    if (_lastStackCount <= 1)
                    {
                        _countText.enabled = true;
                    }
                    _lastStackCount = GameItem.StackCount;
                    if (_lastStackCount <= 1)
                    {
                        _countText.enabled = false;
                    }
                    _countText.text = _lastStackCount.ToString();
                }
                if (GameItem is Weapon weapon)
                {
                    _countText.enabled = true;
                    _countText.text = $"{weapon.AmmoCount}/{weapon.MaxAmmoCount}";
                }
            }
            if (_countText.enabled && _lastTextColor != TextColor)
            {
                _lastTextColor = TextColor;
                _countText.color = TextColor;
            }
        }
    }
}
