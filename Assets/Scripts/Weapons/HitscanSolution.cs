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
        public HitscanSolution()
        {
        }

        public override void Fire()
        {
            base.Fire();
        }
    }
}
