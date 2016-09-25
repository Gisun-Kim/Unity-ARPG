using UnityEngine;
using System;
using System.Collections;

namespace Gisun
{
    public interface IDamageable
    {
        void TakeHit(int damage, Vector3 hitPoint, Vector3 hitDirection);
        void TakeDamage(int damage);
    }

    public class CharacterEntity : MonoBehaviour, IDamageable
    {
        public int maxHealth = 100;
        public int curruntHealth;

        protected bool isDead = false;

        // events
        public event Action OnDeath;

        public bool IsDead { get { return isDead; } }

        protected virtual void Start()
        {
            curruntHealth = maxHealth;
            isDead = false;
        }

        public virtual void TakeHit(int damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            TakeDamage(damage);
        }

        public virtual void TakeDamage(int damage)
        {
            curruntHealth = Mathf.Clamp(curruntHealth - damage, 0, maxHealth);
            if (curruntHealth <= 0 && !isDead)
            {
                this.Die();
            }
        }

        protected virtual void Die()
        {
            isDead = true;
            if (this.OnDeath != null)
            {
                this.OnDeath();
            }
        }
    }
}