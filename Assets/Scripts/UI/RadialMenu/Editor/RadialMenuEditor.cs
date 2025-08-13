using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace FoodWars.RadialMenu.UI
{

    [CustomEditor(typeof(RadialMenu))]
    public class RadialMenuEditor : Editor
    {
        private RadialMenu _radialMenu;
        private int _numberOfButtons = 0;
        public override void OnInspectorGUI()
        {
            _radialMenu = target as RadialMenu;

            base.OnInspectorGUI();
            if (GUILayout.Button("Update Menu")) UpdateButtons();
            if (GUILayout.Button("Add Button")) AddButton();
            if (GUILayout.Button("Remove Button")) RemoveButton();
        }

        private void AddButton()
        {
            _radialMenu.AddElement();
            UpdateButtons();
        }

        private void RemoveButton()
        {
            _radialMenu.RemoveElement();
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            float rotation = 360 / _radialMenu.ButtonCount;

            var buttons = _radialMenu.GetListOfButtons();
            _numberOfButtons = buttons.Count;

            if (buttons == null) return;

            for (var i = 0; i < buttons.Count; i++)
            {
                buttons[i].SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, rotation * i));
            }
        }
    }
}
