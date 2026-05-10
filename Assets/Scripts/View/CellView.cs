using Assets.Scripts.Core;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.View
{
    public class CellView : MonoBehaviour
    {
        private Vector3 _basePosition;
        private Vector3 _targetOffset;
        private Tween _offsetTween;

        private Renderer _renderer;
        private Collider _collider;

        [SerializeField] private CellHintView _hintView;

        [SerializeField] private GameObject _mineVisual;
        [SerializeField] private Color _revealedColor = new(0.75f, 0.75f, 0.75f);
        [SerializeField] private Color _neigbourColor = new(0.65f, 0.65f, 1f);
        [SerializeField] private Color _dimmedColor = new(0.3f, 0.3f, 0.35f, 1f);
        [SerializeField] private Color _revealedActiveColor = new(0.92f, 0.92f, 0.95f);
        [SerializeField] private Color _revealedNeighborColor = new(0.78f, 0.78f, 0.85f);
        [SerializeField] private Color _revealedDimmedColor = new(0.65f, 0.65f, 0.7f);

        private Color _originalColor;
        private bool _isRevealed;

        public Vector3Int Index { get; private set; }
        public bool IsRevealed => _isRevealed;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _collider = GetComponent<Collider>();
            _originalColor = _renderer.sharedMaterial.color;

            if (_mineVisual != null) _mineVisual.SetActive(false);
            if (_hintView != null) _hintView.gameObject.SetActive(false);
        }

        public void SetPosition(float x, float y, float z)
        {
            _basePosition = new Vector3(x, y, z);
            transform.localPosition = _basePosition;
        }

        public void SetIndexPosition(int x, int y, int z)
        {
            Index = new Vector3Int(x, y, z);
        }
        public void Reveal(CellData data)
        {
            if (_isRevealed) return;
            _isRevealed = true;

            var matColor = _revealedColor;
            matColor.a = 1f;
            _renderer.material.color = matColor;

            if (data.IsMine)
            {
                if (_mineVisual != null)
                {
                    _mineVisual.SetActive(true);
                    var col = _renderer.material.color;
                    col.a = 0f;
                    _renderer.material.color = col;

                    var targetScale = _mineVisual.transform.localScale;
                    _mineVisual.transform.localScale = Vector3.zero;
                    _mineVisual.transform.DOScale(targetScale, 0.35f).SetEase(Ease.OutBack);
                }
                if (_hintView != null) _hintView.gameObject.SetActive(false);
            }
            else
            {
                if (data.AdjacentMines > 0)
                {
                    if (_hintView != null)
                    {
                        _hintView.gameObject.SetActive(true);
                        _hintView.SetHint(data.AdjacentMines, true);
                        _hintView.SetColor(GetNumberColor(data.AdjacentMines));
                    }
                }
                if (_mineVisual != null) _mineVisual.SetActive(false);
            }

            if (_collider != null) _collider.enabled = false;
        }

        public void PlaySpawn()
        {
            var targetScale = transform.localScale;
            transform.localScale = Vector3.zero;
            transform.DOScale(targetScale, 0.5f).SetEase(Ease.OutBack);
        }

        public void SetVisualOffset(Vector3 offset, float duration = 0.35f)
        {
            if (_targetOffset == offset) return;
            _targetOffset = offset;

            Vector3 targetPos = _basePosition + _targetOffset;

            if (_offsetTween != null && _offsetTween.IsActive())
                _offsetTween.Kill();

            _offsetTween = transform.DOLocalMove(targetPos, duration)
                                    .SetEase(Ease.OutCubic)
                                    .OnComplete(() => _offsetTween = null);
        }

        public void SetDimmed(bool dimmed)
        {
            if (_isRevealed) return;

            if (dimmed)
            {
                _renderer.material.color = _dimmedColor;
            }
            else
            {
                _renderer.material.color = _isRevealed ? _revealedColor : _originalColor;
            }


            //var color = _renderer.material.color;
            //color.a = dimmed ? 0.4f : 1f;

            //_renderer.material.color = color;
            //_renderer.material.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            //_collider.enabled = !dimmed;
        }

        public void SetSliceState(int distance)
        {
            if (_isRevealed)
            {
                Color targetColor = distance switch
                {
                    0 => _revealedActiveColor,
                    1 => _revealedNeighborColor,
                    _ => _revealedDimmedColor
                };

                _renderer.material.color = targetColor;
                return;
            }

            Color unrevealedColor = distance switch
            {
                0 => _revealedColor,
                1 => _neigbourColor,
                _ => _dimmedColor
            };

            _renderer.material.color = unrevealedColor;

        }

        public void ClearSliceState()
        {
            if (_isRevealed)
            {
                _renderer.material.color = _revealedColor;
            }
            else
            {
                _renderer.material.color = _originalColor;
            }
        }

        public void SetHint(int mineCount, bool show)
        {
            if (_hintView != null)
                _hintView.SetHint(mineCount, show);
        }

        private Color GetNumberColor(int number)
        {
            return number switch
            {
                1 => new Color(0f, 0.2f, 1f),
                2 => new Color(0f, 0.8f, 0.2f),
                3 => new Color(1f, 0.2f, 0.2f),
                4 => new Color(0.2f, 0.2f, 0.8f),
                5 => new Color(0.8f, 0.2f, 0.2f),
                6 => new Color(0.2f, 0.8f, 0.8f),
                7 => new Color(0.1f, 0.1f, 0.1f),
                8 => new Color(0.5f, 0.5f, 0.5f),

                9 => new Color(0.6f, 0.2f, 0.8f),
                10 => new Color(1f, 0.6f, 0.2f),
                11 => new Color(1f, 0.2f, 0.8f),
                12 => new Color(0.2f, 0.6f, 0.2f),
                13 => new Color(0.8f, 0.8f, 0.2f),
                14 => new Color(0.2f, 0.4f, 0.6f),
                15 => new Color(0.8f, 0.4f, 0.2f),

                16 => new Color(1f, 0f, 1f),
                17 => new Color(0f, 1f, 1f),
                18 => new Color(1f, 1f, 0f),
                19 => new Color(0.5f, 0f, 0.5f),
                20 => new Color(1f, 0.5f, 0f),

                21 => new Color(1f, 0f, 0f),
                22 => new Color(0.8f, 0f, 0f),
                23 => new Color(0.6f, 0f, 0f),
                24 => new Color(1f, 0.3f, 0f),
                25 => new Color(0.9f, 0.9f, 0f),
                26 => new Color(1f, 0f, 1f),

                _ => Color.white
            };
        }
    }
}
