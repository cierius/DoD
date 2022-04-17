using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IProjectile
{
    private CharStats stats;

    private Vector2 startingPos;
    public float range = 20f;

    public int bulletDamage = 10;
    public int critChance = 5;
    public int critMultiplier;

    public GameObject floatingDamageText;

    private void Awake()
    {
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharStats>();

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>());
        transform.SetParent(null);
        transform.localScale = new Vector3(.125f, .125f, 1f);
        startingPos = transform.position;

        StartCoroutine(CheckRange());
    }


    IEnumerator CheckRange() // Every 0.1 sec will check to see if the bullet is out of range and thus will delete if so.
    {
        while (gameObject.transform != null)
        {
            if (Mathf.Abs(Vector2.Distance(startingPos, transform.position)) > range)
            {
                Destroy(gameObject);
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.1f);
        }
        
    }

    public void SetCritChance(int crit) => critChance = crit + stats.critChanceAdditive;

    public void SetRange(float _range) => range = _range;

    public void SetCritMultiplier(int multiplier) => critMultiplier = multiplier + stats.critMultiplierAdditive;

    public void SetDamage(int dmg) => bulletDamage = dmg + Mathf.RoundToInt(stats.damageAdditive);


    // Rotates the bullet to be looking at the point of fire
    public void SetLookDir(Vector2 d)
    {
        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dir.Normalize();

        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }


    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Unwalkable")
        {
            Destroy(this.gameObject);
        }
        else if(coll.gameObject.GetComponent<IDamageable>() != null)
        {
            // Vary the damage a little
            bulletDamage += Random.Range(-2, 2);

            // Crit chance
            bool hasCrit = false;
            if (Random.Range(0, 99) < critChance)
            {
                bulletDamage *= critMultiplier;
                hasCrit = true;
            }

            

            var floatingText = Instantiate(floatingDamageText);
            var randPos = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            floatingText.transform.position = coll.transform.position + randPos;
            floatingText.GetComponent<FloatDamageText>().SetText(bulletDamage, hasCrit);


            Destroy(this.gameObject);

            coll.gameObject.GetComponent<IDamageable>().ReceiveDamage(bulletDamage); // this is the damage dealing part
        }
    }
}
