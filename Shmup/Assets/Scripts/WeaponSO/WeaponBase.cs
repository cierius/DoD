using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is for building a base weapon class
 * 
 * If modular, this script could be very handy in possibly allowing the turrets to use different weapons as well
 *
 */


[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon Scriptable Object", order = 1)]
public class WeaponBase : ScriptableObject
{
    public enum WeaponType
    {
        Singleshot,
        Multishot,
        Launcher,
        Melee
    }

    [Header("--- Weapon Info ---")]
    [Space(10)]
    public string weaponName;
    public WeaponType weaponType;
    public Sprite weaponSprite;


    [Header("--- General Weapon Attributes ---")]
    [Space(10)]
    [Range(1, 100)] public float damage = 1;
    [Range(1, 20)] public int range;
    [Range(0, 300)] public int fireRate; //shots per minute
    [Range(0f, 5f)] public float reloadTime; // time in sec
    [Range(0, 36)] public int clipSize;
    [Range(0, 999)] public int startingAmmo;
    [Range(0, 999)] public int maxAmmo;
    [Range(0, 100)] public int critChance = 5; // Base is 5%
    public int critMultiplier = 2; // Base is 2x


    [Header("--- Projectile Attributes ---")]
    [Space(10)]
    private GameObject projectileInstance;
    public GameObject projectile; // Whatever will be instantiated when firing
    public float projVelocity;


    [Header("--- Multi-projectile ---")]
    [Space(10)]
    [Range(0f, 0.5f)] public float sprayAmount;
    [Range(5, 20)] public int shottyProjCount;
    [Range(10, 50)] public float shottyScatterTightness;
    private Vector2 spray;

    [Header("--- Melee Attributes ---")]
    [Space(10)]
    [Range(0, 360)] public int attackRadius = 45; // Degrees

    [Header("--- Special Attributes ---")]
    [Space(10)]
    [Range(0, 5)] public float explosionRadius;
    
    


    public float Fire(Vector2 dir, Transform trans)
    {
        switch(weaponType)
        {
            case WeaponType.Singleshot:
                spray = new Vector2(Random.Range(-sprayAmount, sprayAmount), Random.Range(-sprayAmount, sprayAmount));
                projectileInstance = Instantiate(projectile, trans);

                projectileInstance.GetComponent<IProjectile>().SetDamage(damage);
                projectileInstance.GetComponent<IProjectile>().SetRange(range);
                projectileInstance.GetComponent<IProjectile>().SetCritChance(critChance);
                projectileInstance.GetComponent<IProjectile>().SetCritMultiplier(critMultiplier);
                projectileInstance.GetComponent<IProjectile>().SetLookDir(dir);

                projectileInstance.GetComponent<Rigidbody2D>().AddRelativeForce((dir + spray) * projVelocity, ForceMode2D.Impulse);
                break;

            case WeaponType.Multishot:
                for(int i = -shottyProjCount/2; i < shottyProjCount/2; i++)
                {
                    spray = new Vector2(Random.Range(-sprayAmount, sprayAmount), Random.Range(-sprayAmount, sprayAmount));
                    projectileInstance = Instantiate(projectile, trans);

                    projectileInstance.GetComponent<IProjectile>().SetDamage(damage);
                    projectileInstance.GetComponent<IProjectile>().SetRange(range);
                    projectileInstance.GetComponent<IProjectile>().SetCritChance(critChance);
                    projectileInstance.GetComponent<IProjectile>().SetCritMultiplier(critMultiplier);
                    projectileInstance.GetComponent<IProjectile>().SetLookDir(dir);

                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    {
                        projectileInstance.GetComponent<Rigidbody2D>().AddRelativeForce((new Vector2(dir.x, dir.y + i / shottyScatterTightness) + spray) * projVelocity, ForceMode2D.Impulse);
                    }
                    else
                    {
                        projectileInstance.GetComponent<Rigidbody2D>().AddRelativeForce((new Vector2(dir.x + i / shottyScatterTightness, dir.y) + spray) * projVelocity, ForceMode2D.Impulse);
                    }
                    
                }
                break;

            case WeaponType.Launcher:
                projectileInstance = Instantiate(projectile, trans);
                projectileInstance.GetComponent<IProjectile>().SetDamage(damage);
                projectileInstance.GetComponent<IProjectile>().SetRange(range);
                projectileInstance.GetComponent<IProjectile>().SetCritChance(critChance);
                projectileInstance.GetComponent<IProjectile>().SetCritMultiplier(critMultiplier);
                projectileInstance.GetComponent<IProjectile>().SetLookDir(dir);
                projectileInstance.GetComponent<Rocket>().SetExplosionRadius(explosionRadius);
                projectileInstance.GetComponent<Rigidbody2D>().AddRelativeForce((dir + spray) * projVelocity, ForceMode2D.Impulse);
                break;

            case WeaponType.Melee:
                break;
        }


        return 0f;
    }

    private List<Collider2D> MeleeAttack(int radius, Vector2 dir, Transform trans)
    {
        List<Collider2D> coll = new List<Collider2D>();

        for(int i = -radius/2; i < radius/2; i++) // Casts a ray every degree of the attack
        {
            var ray = Physics2D.Linecast(trans.position, dir);
            if(ray.collider != null)
                coll.Add(ray.collider);
        }

        return coll;
    }
}
