using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerRune : MonoBehaviour, IDamageable
{
    [Range(10, 1000)] [SerializeField] private float health = 50; // Current health of rune
    //[Range(0, 10)] [SerializeField] private int healthRegen = 1; // Regen per second


    void Update()
    {
        if(health <= 0)
            Death();

        // Doesn't work currently
        //if (health < maxHealth)
            //health += Mathf.RoundToInt(healthRegen * Time.deltaTime);
            //health = Mathf.Clamp(health, 0, maxHealth);
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    public void ReceiveDamage(float amount)
    {
        health -= amount;
    }
}
