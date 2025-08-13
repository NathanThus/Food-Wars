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
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private ParticleSystem _tracer;

        public HitscanSolution() { }

        public override void Fire()
        {
            if (!Physics.Raycast(new Ray(_muzzleTransform.position, _muzzleTransform.forward), out RaycastHit hit, _range))
                return;
            _tracer?.Play();
            Debug.Log("Hit!");
        }
    }
}
