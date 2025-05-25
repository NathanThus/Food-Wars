using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] private float _reloadTimeSeconds;

        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _muzzleFlash;

        [Header("Dependencies")]
        [SerializeField] private Transform _muzzle;

        #endregion

        #region Fields

        private CancellationTokenSource _cancellationTokenSource;
        private UserInputActions _userActions;
        private InputAction _fireAction, _reloadAction;

        private bool _isBusy = false;

        #endregion

        #region Properties
        public BulletType BulletType => _type;

        #endregion

        #region Start

        public void Awake()
        {
            _cancellationTokenSource = new();
            _userActions = new();
            _fireAction = _userActions.Player.Fire;
            _reloadAction = _userActions.Player.Reload;
            _currentRounds = _magazineSize;
        }

        public void OnEnable()
        {
            _reloadAction.performed += HandleReload;
            _reloadAction.Enable();
            _fireAction.Enable();
        }

        void OnDisable()
        {
            _reloadAction.performed -= HandleReload;
            _reloadAction.Disable();
            _fireAction.performed -= HandleFireAsync;
            _fireAction.Disable();
        }

        void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        #endregion

        #region Public

        public void SwitchType() => _BulletStats = _type switch
        {
            BulletType.HitScan => new HitscanSolution(),
            BulletType.Projectile => new ProjectileSolution(),
            _ => throw new ArgumentOutOfRangeException(nameof(BulletType)),
        };

        #endregion

        #region EventHandlers

        private async void HandleFireAsync(InputAction.CallbackContext _)
        {
            if (_isBusy) return;
            _isBusy = true;

            while (_fireAction.IsPressed())
            {
                if (_currentRounds <= 0)
                {
                    _isBusy = false;
                    return;
                }

                _currentRounds--;
                _BulletStats.Fire();
                await UniTask.WaitForSeconds(_fireDelaySeconds, cancellationToken: _cancellationTokenSource.Token);
            }
            _isBusy = false;
        }

        private async void HandleReload(InputAction.CallbackContext _)
        {
            if (_isBusy) return;

            _isBusy = true;
            await UniTask.WaitForSeconds(_reloadTimeSeconds, cancellationToken: _cancellationTokenSource.Token);

            _currentRounds = _magazineSize;

            _isBusy = false;
        }

        #endregion
    }
}
