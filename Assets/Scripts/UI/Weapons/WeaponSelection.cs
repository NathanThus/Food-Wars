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
        #region Fields

        private UserInputActions _userInput;
        private InputAction _openMenu;

        #endregion

        private void Awake()
        {
            _userInput = new UserInputActions();
            _openMenu = _userInput.Player.OpenMenu;
        }
        void OnEnable()
        {
            _openMenu.performed += HandleMenuOpen;
            _openMenu.canceled += HandleMenuClose;
            _openMenu.Enable();
        }


        void OnDisable()
        {
            _openMenu.performed -= HandleMenuOpen;
            _openMenu.canceled -= HandleMenuClose;
            _openMenu.Disable();
        }

        private void HandleMenuOpen(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        private void HandleMenuClose(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
            
    }
}
