using UnityEngine;
using System.Collections;

namespace Gisun
{
    public class EnemyEntity : CharacterEntity
    {

        protected override void Start()
        {

        }

        protected override void Die()
        {
            base.Die();

            Destroy(this.gameObject);
        }
    }
}
