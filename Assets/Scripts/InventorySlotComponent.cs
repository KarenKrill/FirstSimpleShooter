using Assets.Scripts.Model;
using UnityEngine;

public class InventorySlotComponent : MonoBehaviour
{
    public GameObject Item;
    private InventorySlot _slot;
    public InventorySlot Slot => _slot;
    public void Init(InventorySlot slot) => _slot = slot;
    public void PutItem(GameObject item)
    {
        Item = item;
        Item.transform.position = transform.position;
        Item.transform.localScale = transform.localScale;
    }
    public void Awake()
    {
        if(Item != null)
        {
            Item.transform.position = transform.position;
        }
    }
}
