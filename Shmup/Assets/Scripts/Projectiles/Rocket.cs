using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour, IProjectile
{
    private CharStats stats;

    public bool showExplosionRadius = false;

    private Vector2 startingPos;
    private float range;

    private float damage;
    private int critChance;
    private int critMultiplier;
    private float explosionRadius; //2.5 is default to fit with the particle system, need to figure that system out

    public GameObject floatingDamageText;
    private ParticleSystem explosionParticles;

    private void Awake()
    {
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharStats>();

        explosionParticles = GetComponentInChildren<ParticleSystem>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>());
        transform.SetParent(null);
        transform.localScale = new Vector3(.25f, .25f, 1f);

        startingPos = transform.position;

        StartCoroutine(CheckRange());
    }


    IEnumerator CheckRange() // Every 0.1 sec will check to see if the bullet is out of range and thus will delete if so.
    {
        while (gameObject != null)
        {
            if (Mathf.Abs(Vector2.Distance(startingPos, transform.position)) > range)
            {
                Explode();
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.1f);
        }
    }


    public void SetRange(float _range) => range = _range;

    public void SetCritChance(int crit) => critChance = crit + stats.critChanceAdditive;

    public void SetCritMultiplier(int multiplier) => critMultiplier = multiplier + stats.critMultiplierAdditive;

    public void SetDamage(float dmg) => damage = (dmg + (stats.damageAdditive * dmg)) * stats.damageMultiplier;

    public void SetExplosionRadius(float radius) => explosionRadius = radius;

    public void SetLookDir(Vector2 d)
    {
        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dir.Normalize();

        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    private void OnDrawGizmos()
    {
        if(showExplosionRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }


    public void Explode()
    {
        // Particle stuff
        explosionParticles.transform.parent = null;
        explosionParticles.transform.localScale = new Vector3(1, 1, 1);
        explosionParticles.Play();


        //Explosion radius checking for all enemies and applying damage
        RaycastHit2D[] explosionCircleCast = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector2.zero);
        if(explosionCircleCast != null)
        {
            // Vary the damage a little
            damage += Random.Range(-damage*0.1f, damage*0.1f);

            // Crit chance for the whole explosion
            bool hasCrit = false;
            if (Random.Range(0, 99) < critChance)
            {
                damage *= critMultiplier;
                hasCrit = true;
            }

            foreach (RaycastHit2D ray in explosionCircleCast)
            {
                if (ray.collider.GetComponent<IDamageable>() != null)
                {
                    ray.collider.GetComponent<IDamageable>().ReceiveDamage(damage);

                    var floatingText = Instantiate(floatingDamageText);
                    var randPos = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                    floatingText.transform.position = ray.collider.transform.position + randPos;
                    floatingText.GetComponent<FloatDamageText>().SetText(damage, hasCrit);
                }
            }
        }
        

        Destroy(this.gameObject);
    }

    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll != null)
        {
            Explode();
        }
    }
}
