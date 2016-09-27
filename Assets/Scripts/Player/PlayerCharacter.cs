using UnityEngine;
using System;
using System.Collections;

public class PlayerCharacter : LivingEntity, IDamageable
{


    protected override void Start()
    {
        base.Start();
    }

    public override void TakeHit(int damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);

    }


    protected override void Die()
    {
        base.Die();
    }
}