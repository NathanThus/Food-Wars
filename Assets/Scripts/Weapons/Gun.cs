using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodWars.Weapons
{
    public class Gun : MonoBehaviour
    {
        #region  Serialized Fields
        [SerializeField] private BulletType _type;
        [Header("Stats")]
        [SerializeReference] private BulletSolution _BulletStats;
        [SerializeField] private float _fireDelaySeconds;
        [Header("Magazine Stats")]

        [SerializeField] private int _magazineSize;
        [SerializeField] private int _currentRounds;

        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _muzzleFlash;

        [Header("Dependencies")]
        [SerializeField] private Transform _muzzle;
        #endregion

        public BulletType BulletType => _type;

        public void Fire()
        {
            if(_currentRounds <= 0) return;
            _currentRounds--;
            throw new NotImplementedException();
        }

        public void Reload()
        {

        }

        public void SwitchType() => _BulletStats = _type switch
        {
            BulletType.HitScan => new HitscanSolution(),
            BulletType.Projectile => new ProjectileSolution(),
            _ => throw new ArgumentOutOfRangeException(nameof(BulletType)),
        };

        #region EventHandlers



        #endregion
    }
}
