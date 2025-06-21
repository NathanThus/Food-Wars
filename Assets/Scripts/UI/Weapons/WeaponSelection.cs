using System;
using System.Collections;
using System.Collections.Generic;
using Foodwars.UserInputActions;
using FoodWars.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodWars.UI.Weapons
{
    public class WeaponSelection : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private MagazineUI _magazineUI;
        [SerializeField] private Gun _currentGun;
        [SerializeField] private List<Gun> _guns;

        #endregion

        #region Fields

        private UserInputActions _userInput;
        private InputAction _openMenu;


        #endregion

        #region Start

        private void Awake()
        {
            _userInput = new UserInputActions();
            _openMenu = _userInput.Player.OpenMenu;
        }

        private void Start()
        {
            _currentGun.OnMagazineChange += _magazineUI.HandleWeaponFire;
        }

        private void OnEnable()
        {
            _openMenu.performed += HandleMenuOpen;
            _openMenu.canceled += HandleMenuClose;
            _openMenu.Enable();
        }


        private void OnDisable()
        {
            _openMenu.performed -= HandleMenuOpen;
            _openMenu.canceled -= HandleMenuClose;
            _openMenu.Disable();
            _currentGun.OnMagazineChange -= _magazineUI.HandleWeaponFire;
        }

        #endregion

        #region Event Handlers

        private void HandleMenuOpen(InputAction.CallbackContext _)
        {
            throw new NotImplementedException();
        }

        private void HandleMenuClose(InputAction.CallbackContext _)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public

        public void SwitchWeapon(GunType next)
        {
            Gun selectedGun = _guns.Find(gun => gun.GunType == next);

            if (selectedGun == null) throw new ArgumentOutOfRangeException(nameof(GunType));

            _currentGun.OnMagazineChange -= _magazineUI.HandleWeaponFire;
            _currentGun.enabled = false;

            selectedGun = _currentGun;

            _currentGun.OnMagazineChange += _magazineUI.HandleWeaponFire;
            _currentGun.enabled = true;
        }

        #endregion

        #region Editor Methods

        /// <summary>
        /// Editor Exclusive. Do not call.
        /// </summary>
        public void ClearWeaponList()
        {
            _guns.Clear();
        }

        public void AddWeapons(Gun[] guns)
        {
            if (guns == null) throw new ArgumentNullException(nameof(guns));
            if (guns.Length == 0) throw new ArgumentOutOfRangeException(nameof(guns));

            foreach (var gun in guns)
            {
                _guns.Add(gun);
            }

            _currentGun = _guns[0];
        }

        #endregion
    }
}
