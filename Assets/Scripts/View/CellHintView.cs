using Assets.Scripts.Core;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.View
{
    public class CellHintView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _textMesh;
        [SerializeField] private Camera _targetCamera;

        private void Awake()
        {
            if (_targetCamera == null) _targetCamera = Camera.main;
        }

        public void SetColor(Color color)
        {
            if (_textMesh != null)
                _textMesh.color = color;
        }

        public void SetHint(int mineCount, bool isVisible)
        {
            if (_textMesh != null)
            {
                _textMesh.text = mineCount > 0 ? mineCount.ToString() : string.Empty;
                _textMesh.enabled = isVisible && mineCount > 0;
            }
        }

        private void LateUpdate()
        {
            if (!gameObject.activeSelf || _targetCamera == null || _textMesh == null || !_textMesh.enabled)
                return;

            Vector3 toCamera = _targetCamera.transform.position - transform.position;

            if (toCamera.sqrMagnitude < 0.001f) return;

            transform.rotation = Quaternion.LookRotation(-toCamera, _targetCamera.transform.up);
        }
    }
}
