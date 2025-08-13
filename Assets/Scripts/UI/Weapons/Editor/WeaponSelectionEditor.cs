using UnityEngine;
using UnityEditor;
using FoodWars.UI.Weapons;
using System;
using FoodWars.Weapons;
using System.Collections.Generic;

[CustomEditor(typeof(WeaponSelection))]
public class WeaponSelectionEditor : Editor
{
    WeaponSelection _weaponSelection;
    public override void OnInspectorGUI()
    {
        _weaponSelection = target as WeaponSelection;

        base.DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Update Weapon List"))
        {
            EditorUtility.SetDirty(_weaponSelection);
            UpdateWeaponList();
            AssetDatabase.SaveAssets();

        }
    }

    private void UpdateWeaponList()
    {
        Gun[] children = _weaponSelection.gameObject.GetComponentsInChildren<Gun>();

        if (children == null) throw new ArgumentNullException(nameof(children));
        if (children.Length == 0) throw new ArgumentOutOfRangeException(nameof(children));

        _weaponSelection.ClearWeaponList();
        _weaponSelection.AddWeapons(children);
    }
}