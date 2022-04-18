using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    public void SetDamage(float damage);

    public void SetRange(float range);

    public void SetCritChance(int critChance);

    public void SetCritMultiplier(int multiplier);

    public void SetLookDir(Vector2 dir);

    public void OnTriggerEnter2D(Collider2D coll);
}
