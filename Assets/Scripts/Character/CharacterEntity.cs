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
        [SerializeField]
        private int _startHP = 100;
        protected int _curruntHP;
        protected bool _isDead = false;

        // events
        public event Action OnDeath;

        public int HP { get { return _curruntHP; } }
        public bool IsDead { get { return _isDead; } }

        protected virtual void Start()
        {
            _curruntHP = _startHP;
            _isDead = false;
        }

        public virtual void TakeHit(int damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            TakeDamage(damage);
        }

        public virtual void TakeDamage(int damage)
        {
            _curruntHP = Mathf.Clamp(_curruntHP - damage, 0, _startHP);
            if (_curruntHP <= 0 && !_isDead)
            {
                this.Die();
            }
        }

        protected virtual void Die()
        {
            _isDead = true;
            if (this.OnDeath != null)
            {
                this.OnDeath();
            }
        }
    }
}