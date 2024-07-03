using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public GameObject Item;
    public void PutItem(GameObject item)
    {
        Item = item;
        Item.transform.position = transform.position;
    }
    public void Awake()
    {
        if(Item != null)
        {
            Item.transform.position = transform.position;
        }
    }
}
