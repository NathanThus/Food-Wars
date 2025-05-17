using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodWars.Weapons
{
    [Serializable]
    public class ProjectileSolution : BulletSolution
    {
        [SerializeField] private float _speed;
        [SerializeField] private GameObject _projectileObject;
        public ProjectileSolution()
        {
        }

    }
}
