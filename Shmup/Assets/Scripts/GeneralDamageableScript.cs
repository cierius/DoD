using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralDamageableScript : MonoBehaviour, IDamageable
{
    [Range(0, 10000)] public float health = 100;

    public void Death()
    {
        if (health <= 0)
            Destroy(gameObject, 0.5f);
    }

    public void ReceiveDamage(float amount)
    {
        health -= amount;
        Death(); // Checks to see if the unit has "died"
    }
}
