using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets.Scripts.Input
{
    public class SliceUIController : MonoBehaviour
    {
        [SerializeField] private Button _btnPrev;
        [SerializeField] private Button _btnNext;
        [SerializeField] private Button _btnCancel;
        [SerializeField] private GameObject _controlsPanel;

        private SliceController _sliceController;

        [Inject]
        public void Init(SliceController sliceController)
        {
            _sliceController = sliceController;
        }

        private void Awake()
        {
            _btnPrev.onClick.AddListener(() => _sliceController.MoveSlice(-1));
            _btnNext.onClick.AddListener(() => _sliceController.MoveSlice(1));
            _btnCancel.onClick.AddListener(() => _sliceController.ClearSlice());
        }

        private void Update()
        {
            bool isActive = _sliceController.IsSliceActive;

            if (_controlsPanel.activeSelf != isActive)
                _controlsPanel.SetActive(isActive);

            if (isActive)
            {
                var (_, index) = _sliceController.CurrentSlice;
                int size = _sliceController.Size;

                _btnPrev.interactable = index > 0;
                _btnNext.interactable = index < size - 1;
            }
        }
    }
}
