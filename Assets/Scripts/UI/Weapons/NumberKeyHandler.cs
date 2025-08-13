using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodWars.UI.Weapons
{
    using FoodWars.Weapons;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class NumberKeyHandler : MonoBehaviour
    {
        [SerializeField] private InputActionReference _numberKeyAction;
        [SerializeField] private WeaponSelection _weaponSelector;

        private void OnEnable()
        {
            _numberKeyAction.action.performed += OnNumberKeyPressed;
        }

        private void OnDisable()
        {
            _numberKeyAction.action.performed -= OnNumberKeyPressed;
        }

        private void OnNumberKeyPressed(InputAction.CallbackContext context)
        {
            GunType type = GetNumberFromControl(context.control);
            if (type != GunType.Invalid)
            {
                _weaponSelector.SwitchWeapon(type);
                Debug.Log($"Number pressed: {type}");
            }
        }

        private GunType GetNumberFromControl(InputControl control)
        {
            string controlName = control.name;

            return controlName switch
            {
                "1" or "numpad1" => GunType.CroissantRevolver,
                "2" or "numpad2" => GunType.HotDogRailGun,
                "3" or "numpad3" => GunType.CroissantRevolver,
                "4" or "numpad4" => GunType.CroissantRevolver,
                "5" or "numpad5" => GunType.CroissantRevolver,
                "6" or "numpad6" => GunType.CroissantRevolver,
                _ => GunType.Invalid // Invalid key
            };
        }
    }
}
