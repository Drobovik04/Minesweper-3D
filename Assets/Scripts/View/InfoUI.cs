using Assets.Scripts.Core;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.View
{
    public class InfoUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;

        private void Awake()
        {
            _panel.SetActive(false);
        }

        public void Show()
        {
            _panel.SetActive(true);
            GameEvents.RaiseInfoShow();
        }

        public void Hide()
        {
            _panel.SetActive(false);
            GameEvents.RaiseInfoHide();
        }

    }
}
