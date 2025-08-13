using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodWars.Weapons
{
    [Serializable]
    public class ProjectileSolution : BulletSolution
    {
        [SerializeField] private float _speed;
        [SerializeField] private Projectile _projectileObject;

        // May implement pooling in the future, but not now.
        public ProjectileSolution() { }

        public override void Fire()
        {
            var projectile = UnityEngine.Object.Instantiate(_projectileObject,
                                                _muzzleTransform.position,
                                                Quaternion.identity);
            projectile.Damage = _damage;
            projectile.Rigidbody.AddForce(Vector3.forward * _speed, ForceMode.Impulse);
        }

    }
}
