using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodWars.Weapons
{
    [Serializable]
    public class BulletSolution
    {
        [SerializeField] protected readonly string _name;
        [SerializeField] protected float _damage;
        [SerializeField] protected Transform _muzzleTransform;

        public float Damage => _damage;

        public virtual void Fire()
        {

        }
        
    }
}
