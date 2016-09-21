using UnityEngine;
using System.Collections;

namespace Gisun
{
    public class Weapon : MonoBehaviour
    {
        public int damage = 0;
        private Collider _collider;

        void Start()
        {
            _collider = GetComponent<Collider>();
        }

        void Update()
        {

        }

        void OnTriggerEnter(Collider other)
        {
            IDamageable damageableObject = other.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                damageableObject.TakeDamage(damage);
                Debug.Log("Take Damage");
            }
        }

        public void EnableCollider()
        {
            _collider.enabled = true;
        }

        public void DisableCollider()
        {
            _collider.enabled = false;
        }
    }
}
