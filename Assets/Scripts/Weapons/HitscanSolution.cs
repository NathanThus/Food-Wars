using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodWars.Weapons
{
    [Serializable]
    public class HitscanSolution : BulletSolution
    {
        [SerializeField] private float _range;
        [SerializeField] private Transform _thisObj;
        [SerializeField] private LayerMask _layerMask;
        public HitscanSolution()
        {
        }

        public override void Fire()
        {
        }
    }
}
