using UnityEngine;

public class InventorySlotComponent : MonoBehaviour
{
    public GameObject Item;
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
