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
        Launcher
    }

    [Header("Weapon Info")]
    public string weaponName;
    public WeaponType weaponType;
    public Sprite weaponSprite;


    [Header("General Weapon Attributes")]
    public int damage;
    public int range = 10;
    [Range(0, 300)] public int fireRate; //shots per minute
    [Range(0f, 5f)] public float reloadTime; // time in sec
    [Range(1, 36)] public int clipSize;


    [Header("Crit")]
    [Range(0, 100)] public int critChance = 5; // Base is 5%
    public int critMultiplier = 2; // Base is 2x


    [Header("Projectile Attributes")]
    private GameObject projectileInstance;
    public GameObject projectile; // Whatever will be instantiated when firing
    public float projVelocity;


    [Header("Special Attributes")]
    public float explosionRadius;
    private Vector2 spray;
    [Range(0f, 0.5f)] public float sprayAmount;
    [Range(5, 20)] public int shottyProjCount;
    [Range(10, 50)] public float shottyScatterTightness;


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
        }


        return 0f;
    }
}
