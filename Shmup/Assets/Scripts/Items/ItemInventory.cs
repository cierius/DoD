using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public static ItemInventory inventory;

    private GameObject inventoryHUD;

    public List<GameObject> items;


    private void Awake()
    {
        inventory = this; // Singleton method 
        inventoryHUD = GameObject.Find("HUD/Inventory_HUD");
    }

    public void OnItemPickup(GameObject newItem) // Applies passives and (future use) shows popup displaying item & properties
    {
        if(newItem.GetComponent<ItemBase>().itemType == ItemBase.ItemType.Passive)
        {
            newItem.GetComponent<ItemBase>().ApplyPassives();
        }

        items.Add(newItem);

        newItem.transform.parent = inventoryHUD.transform;

        var index = 0f;
        foreach(GameObject item in items)
        {
            item.transform.position = new Vector3((inventoryHUD.transform.position.x - inventoryHUD.transform.localScale.x/2) + 0.25f + 0.4f*index, (inventoryHUD.transform.position.y + inventoryHUD.transform.localScale.y/2) - 0.75f, -5);
            index++;
        }
    }


    public void ItemProc() // Used with on-hit effects
    {
        foreach(var item in items)
        {
            if(item.GetComponent<ItemBase>().itemType == ItemBase.ItemType.WeaponEffect)
            {
                item.GetComponent<ItemBase>().Proc();
            }
        }
    }
}
