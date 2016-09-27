using UnityEngine;
using System;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    public int curruntHealth;

    // events
    public event Action OnDeath;

    public bool IsDead { private set; get; }

    protected virtual void Start()
    {
        curruntHealth = maxHealth;
        IsDead = false;
    }

    public virtual void TakeHit(int damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(int damage)
    {
        curruntHealth = Mathf.Clamp(curruntHealth - damage, 0, maxHealth);
        if (curruntHealth <= 0 && !IsDead)
        {
            this.Die();
        }
    }

    protected virtual void Die()
    {
        IsDead = true;
        if (this.OnDeath != null)
        {
            this.OnDeath();
        }
    }
}