using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FoodWars.UI.Weapons
{
    public class MagazineUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMesh;

        public void HandleWeaponFire(int magazine)
        {
            _textMesh.text = magazine.ToString();
        }
    }
}
