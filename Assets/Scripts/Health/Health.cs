using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace FoodWars.Health
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Health : MonoBehaviour
    {
        #region Events

        public event Action OnDeath;
        public event Action OnDamage;
        public event Action OnHeal;
        public event Action OnRegenerationStarted;
        public event Action OnRegenerationStopped;

        #endregion

        #region Serialized Fields

        [SerializeField] private float _maxHitPoints = 100;
        [SerializeField] private float _hitPoints = 100;

        [Header("Regeneration Settings")]
        [SerializeField] private bool _canRegenerate = false;
        [SerializeField] private float _regenerationPerSecond = 10;
        [SerializeField] private float _initialRegenerationDelaySeconds = 4;
        [SerializeField] private float _regenerationDelaySeconds = 2;

        #endregion

        #region Fields

        private bool _isDead = false;
        private bool _isRegenerating = false;
        private CancellationTokenSource _regenerationTokenSource;

        #endregion

        #region Properties
        public float HitPoints => _hitPoints;
        public float MaxHitPoints => _maxHitPoints;
        public bool IsRegenerating => _isRegenerating;
        public bool IsDead => _isDead;

        #endregion

        void Awake()
        {
            if (_hitPoints != _maxHitPoints) _hitPoints = _maxHitPoints;
            _regenerationTokenSource = new CancellationTokenSource();
        }

        void OnEnable()
        {
            _regenerationTokenSource ??= new CancellationTokenSource();
        }

        void OnDisable()
        {
            StopRegeneration();
        }

        void OnDestroy()
        {
            OnDeath = null;
            OnDamage = null;
            OnHeal = null;
            OnRegenerationStarted = null;
            OnRegenerationStopped = null;

            _regenerationTokenSource.Cancel();
            _regenerationTokenSource.Dispose();
        }

        #region Public

        public void DealDamage(float damage)
        {

            if (_isDead) return;
            if (damage < 0) throw new ArgumentOutOfRangeException(nameof(damage));

            StopRegeneration();

            _hitPoints -= damage;

            if (_hitPoints <= 0)
            {
                OnDeath?.Invoke();
                _isDead = true;
            }
            else
            {
                OnDamage?.Invoke();
            }

            if (_canRegenerate)
            {
                StartRegeneration();
            }
        }

        public void Heal(float hitPoints)
        {
            if (_isDead) return;

            if (hitPoints < 0) throw new ArgumentOutOfRangeException(nameof(hitPoints));

            if (hitPoints + _hitPoints > _maxHitPoints)
            {
                StopRegeneration();
                _hitPoints = _maxHitPoints;
            }
            else
            {
                _hitPoints += hitPoints;
            }

            OnHeal?.Invoke();
        }

        #endregion

        #region Private

#if UNITY_EDITOR || DEBUG

        public void EnableRegeneration() => _canRegenerate = true;
        public void DisableRegeneration() => _canRegenerate = false;
        public void SetMaxHealth(float maxHitPoints) => _maxHitPoints = maxHitPoints;
#endif

        private void StopRegeneration()
        {
            if (_isRegenerating)
            {
                _regenerationTokenSource?.Cancel();
                _regenerationTokenSource?.Dispose();
                _regenerationTokenSource = new CancellationTokenSource();

                _isRegenerating = false;
                OnRegenerationStopped?.Invoke();
            }
        }

        private async void StartRegeneration()
        {
            if (_isRegenerating || _isDead || _hitPoints >= _maxHitPoints) return;

            try
            {
                // Initial delay before regeneration starts
                await UniTask.Delay(TimeSpan.FromSeconds(_initialRegenerationDelaySeconds),
                                   cancellationToken: _regenerationTokenSource.Token);

                _isRegenerating = true;
                OnRegenerationStarted?.Invoke();

                // Regeneration loop
                while (_hitPoints < _maxHitPoints && !_isDead)
                {
                    Heal(_regenerationPerSecond);

                    if (_hitPoints < _maxHitPoints)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(_regenerationDelaySeconds),
                                           cancellationToken: _regenerationTokenSource.Token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when regeneration is cancelled
            }
            finally
            {
                if (_isRegenerating)
                {
                    _isRegenerating = false;
                    OnRegenerationStopped?.Invoke();
                }
            }
        }

        #endregion
    }
}
