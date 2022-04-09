using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * When player collides with this GameObject the player will be given 1 of each ammo type.
 */

public class AmmoBoxScript : MonoBehaviour
{
    private CharStats stats;

    public int ammoToGive = 1;

    private void Awake()
    {
        stats = GameObject.FindWithTag("Player").GetComponent<CharStats>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.transform.tag == "Player")
        {
            GiveAmmo(ammoToGive);
        }
    }

    private void GiveAmmo(int amount)
    {
        for(int i = 0; i < stats.magsInInventory.Length; i++)
        {
            if (stats.magsInInventory[i] < stats.magCarryMax[i])
            {
                stats.magsInInventory[i] += amount;

                if(stats.magsInInventory[i] > stats.magCarryMax[i]) // Makes sure the player doesn't collect more than the max carry amount for ammo per weapon
                    stats.magsInInventory[i] = stats.magCarryMax[i];
            }
        }

        stats.UpdateAmmoHUD();

        Destroy(gameObject);
    }
}
