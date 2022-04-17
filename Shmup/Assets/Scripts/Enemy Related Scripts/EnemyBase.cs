using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyBase
{
    public void Attack();
    public void Flee();
    public void Death();
    public void SearchForPlayer();
    public void FindPath();
    public void Idle();

}
