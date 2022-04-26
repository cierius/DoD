using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossScript : MonoBehaviour, IDamageable, IEnemy
{
    private bool canFlee = false;

    [SerializeField] private IEnemy.State state = IEnemy.State.Idle;

    private float health = 2500f;
    private float moveSpeed = 15f;

    

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void ReceiveDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
            Death();
    }

    public void Death()
    {
        print("YOU HAVE DEFEATED THE BOSS!");
    }

    public void FindPath(Transform target)
    {
        
    }

    public void FollowPath()
    {
        
    }

    public void IdleState()
    {
        
    }

    public void MoveState()
    {
        
    }

    public void AttackState()
    {
        
    }

    public void FleeState()
    {
        
    }
}
