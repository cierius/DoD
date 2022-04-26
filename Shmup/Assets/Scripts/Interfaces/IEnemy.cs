using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public enum State
    {
        Idle,
        FindingPath,
        Moving,
        Attacking,
        Fleeing
    }

    public void FindPath(Transform target); // For use with finding the next path
    public void FollowPath(); // Follows the path
    public void IdleState(); // Changes state to idle and starts idle behaviors
    public void MoveState(); // While moving this state is going to be called
    public void AttackState(); // Once in range this state will happen and the enemy will attack
    public void FleeState(); // If the enemy has the ability to flee then this state will happen causing the enemy to run away from the player
}
