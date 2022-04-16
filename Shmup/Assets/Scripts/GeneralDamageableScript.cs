using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralDamageableScript : MonoBehaviour, IDamageable
{
    [Range(0, 1000)] public int health = 100;

    public void Death()
    {
        if (health <= 0)
            Destroy(gameObject, 1f);
    }

    public void ReceiveDamage(int amount)
    {
        health -= amount;
        Death(); // Checks to see if the unit has "died"
    }
}
