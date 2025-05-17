using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodWars.Weapons
{
    [Serializable]
    public class BulletSolution
    {
        [SerializeField] private readonly string _name;
        [SerializeField] private float _damage;


        public float Damage => _damage;

        public virtual void Fire()
        {

        }
        
    }
}
