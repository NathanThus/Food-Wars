using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodWars.Health
{
    public class Health : MonoBehaviour
    {
        #region Events

        public event Action OnDeath;
        public event Action OnDamage;
        public event Action OnHeal;

        #endregion

        #region Serialized Fields

        [SerializeField] private float _maxHitPoints = 100;
        [SerializeField] private float _hitPoints = 100;

        #endregion

        #region Fields

        bool isDead = false;

        #endregion

        #region Properties

        public float HitPoints => _hitPoints;

        #endregion

        void Awake()
        {
            if (_hitPoints != _maxHitPoints) _hitPoints = _maxHitPoints;
        }

        void OnDestroy()
        {
            OnDeath = null;
            OnDamage = null;
            OnHeal = null;
        }

        #region Public

        public void DealDamage(float damage)
        {
            if (isDead) return;
            if (damage < 0) throw new ArgumentOutOfRangeException(nameof(damage));

            _hitPoints -= damage;

            if (_hitPoints <= 0)
            {
                OnDeath?.Invoke();
                isDead = true;
            }
            else
            {
                OnDamage?.Invoke();
            }
        }

        public void Heal(float hitPoints)
        {
            if (isDead) return;

            if (hitPoints < 0) throw new ArgumentOutOfRangeException(nameof(hitPoints));

            if (hitPoints + _hitPoints > _maxHitPoints)
            {
                _hitPoints = _maxHitPoints;
            }
            else
            {
                _hitPoints += hitPoints;
            }

            OnHeal?.Invoke();
        }

        #endregion
    }
}
