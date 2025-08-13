using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodWars.Weapons
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Rigidbody _rigidBody;
        #endregion

        #region Fields
        private float _damage;
        #endregion

        #region Properties
        internal Rigidbody Rigidbody => _rigidBody;
        internal float Damage { set { _damage = value; } }

        #endregion

        void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Hit: " + collision.collider.name);
            Destroy(gameObject);
        }
    }
}
