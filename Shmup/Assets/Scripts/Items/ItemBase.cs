using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    private CharStats stats;

    public enum ItemType
    {
        Passive,
        WeaponEffect,
        OnDamageTaken
    }

    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    private bool hasAppliedPassive = false; // Turned true once the passive has been applied to CharStats


    [Header("----- General Item Information -----")]
    public string itemName = "Item Name";
    public ItemType itemType = ItemType.Passive;
    public ItemRarity itemRarity;
    [Tooltip("Unique items cannot be stacked - Only show up in the item pool once.")]
    public bool unique;
    public Sprite itemSprite;
    [TextArea] public string itemDescription = "Item Description";


    [Header("----- Passive Properties -----")] // Anything passive -> stat boosts
    [Space(10)] // Space above
    [Tooltip("Flat health")]
    [Range(0, 100)] public int healthIncrease = 0;
     
    [Tooltip("Flat health regenerated per second")]
    [Range(0, 10)] public int regenAmount = 0;

    [Tooltip("Percentage damage reduced")]
    [Range(0, 25)] public float reduceDamagePercentage = 0f;

    [Tooltip("Percentage Damage Boost")]
    [Range(0, 100)] public int damageAdditive = 0;
    public int critChanceAdditive = 0;
    public int critDamageX = 0;
    public float fireRatePercentage = 0;

    [Tooltip("Speed increase percentage")]
    [Range(0, 25)] public float speedBoostPercentage = 0f;


    [Header("----- Weapon Effect Properties -----")] // Changes weapon effect / function
    [Space(10)] // Space above
    public float burnDOT = 0f; // Damage per second 
    public float slowPercentage = 0f; // Slow percent
    public float effectDuration = 0f;


    [Header("----- On Damage Taken Properties -----")] // Whenever player takes damage these will have an effect
    [Space(10)] // Space above

    [Tooltip("After damage, regenerate X% of max health")] 
    [Range(0, 25)] public float healthBack = 0f; 


    private void Awake()
    {
        stats = GameObject.FindWithTag("Player").GetComponent<CharStats>();
        GetComponentInChildren<TextMesh>().text = itemName;

        if(itemSprite != null) // Assign the sprite
            GetComponent<SpriteRenderer>().sprite = itemSprite;
    }


    public void ApplyPassives()
    {
        if (itemType == ItemType.Passive)
        {
            if (!hasAppliedPassive)
            {
                stats.healthAdditive += healthIncrease;
                stats.healthRegen += regenAmount;
                stats.damageReductionPercentage += reduceDamagePercentage;
                stats.damageAdditive += damageAdditive/100f; // Percentage
                stats.critMultiplierAdditive += critDamageX;
                stats.critChanceAdditive += critChanceAdditive;
                stats.fireRatePercentage += fireRatePercentage/100f;
                stats.speedAdditive += stats.speedBase * speedBoostPercentage;

                hasAppliedPassive = true;
            }
        }

        Debug.Log("Passive(s) for "+itemName+" applied!");
    }

    
    public void Proc()
    {
        print("Proc");
    }


    public void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.transform.tag == "Player")
        {
            ItemInventory.Instance.OnItemPickup(gameObject);
        }
    }
}
