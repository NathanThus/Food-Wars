using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodWars.Weapons.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(Gun))]
    public class GunEditor : Editor
    {
        private BulletType _selectedBulletType;
        private Gun _targetGun;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _targetGun = target as Gun;

            if(_selectedBulletType != _targetGun.BulletType)
            {
                _selectedBulletType = _targetGun.BulletType;
                _targetGun.SwitchType();
            }

        }
    }
}
