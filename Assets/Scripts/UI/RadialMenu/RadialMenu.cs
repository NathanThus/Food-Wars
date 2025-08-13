using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FoodWars.RadialMenu
{
    public class RadialMenu : MonoBehaviour
    {
        [SerializeField] private List<Canvas> _buttons;
        [SerializeField] private Canvas _prefab;

        public int ButtonCount { get { return _buttons.Count; } }
        void Start()
        {
            // foreach (var button in _buttons)
            // {
            //     button.alphaHitTestMinimumThreshold = AlphaHitTestMinimumThreshold;
            // }
        }

        public List<RectTransform> GetListOfButtons()
        {
            if (_buttons.Count == 0) return null;
            var list = new List<RectTransform>();

            foreach (var button in _buttons)
            {
                list.Add(button.GetComponent<RectTransform>());
            }

            return list;
        }

        public void AddElement()
        {
            var newElement = Instantiate(_prefab, transform);
            _buttons.Add(newElement);
        }

        public void RemoveElement()
        {
            var obj = _buttons[^1];
            _buttons.Remove(obj);
            DestroyImmediate(obj.gameObject);
        }
    }
}
