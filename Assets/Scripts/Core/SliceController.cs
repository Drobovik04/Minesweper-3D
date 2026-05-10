using Assets.Scripts.View;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using VContainer;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Scripts.Core
{
    public class SliceController : MonoBehaviour
    {
        [SerializeField] private float _offset = 0.1f;
        [SerializeField] private float _animationDuration = 0.35f;

        private Axis _axis;
        private int _index;
        private bool _active;

        private CellView[,,] _views;
        private FieldService _fieldService;
        private int _size;

        public bool IsSliceActive => _active;
        public (Axis axis, int index) CurrentSlice => (_axis, _index);
        public int Size => _size;

        public void Init(CellView[,,] views, FieldService fieldService)
        {
            _views = views;
            _size = views.GetLength(0);
            _fieldService = fieldService;
        }

        public void SetSlice(Axis axis, int index)
        {
            _axis = axis;
            _index = index;
            _active = true;

            HandleSliceChange();
        }

        public void MoveSlice(int direction)
        {
            if (!_active) return;

            int newIndex = _index + direction;
            if (newIndex < 0 || newIndex >= _size) return;

            _index = newIndex;
            HandleSliceChange();
        }

        public void ClearSlice()
        {
            if (_active == false)
                return;

            _active = false;

            HandleSliceChange();
        }

        private void HandleSliceChange()
        {
            if (_views == null) return;

            Vector3 axisVec = GetAxisVector();

            for (int x = 0; x < _size; x++)
                for (int y = 0; y < _size; y++)
                    for (int z = 0; z < _size; z++)
                    {
                        var cell = _views[x, y, z];
                        Vector3 targetOffset = Vector3.zero;

                        int value = GetAxisValue(x, y, z);
                        int distance = Mathf.Abs(value - _index);

                        bool isRelevant = false;

                        if (_active)
                        {
                            isRelevant = distance <= 1;

                            if (value < _index)
                                targetOffset = -axisVec * _offset;

                            else if (value > _index)
                                targetOffset = axisVec * _offset;

                        }
                        else
                        {
                            isRelevant = true;
                        }

                        //cell.SetDimmed(!isRelevant);
                        if (_active)
                            cell.SetSliceState(distance);
                        else
                            cell.ClearSliceState();
                        
                        var cellData = _fieldService.Get(x, y, z);

                        bool showNumbersHere = !_active || isRelevant;

                        if (cell.IsRevealed)
                        {
                            if (showNumbersHere && !cellData.IsMine && cellData.AdjacentMines > 0)
                                cell.SetHint(cellData.AdjacentMines, true);
                            else
                                cell.SetHint(0, false);
                        }
                        else
                        {
                            if (_active && showNumbersHere)
                            {
                                cell.SetHint(cellData.AdjacentMines, true);
                            }
                            else
                            {
                                cell.SetHint(0, false);
                            }
                        }

                        cell.SetVisualOffset(targetOffset, _animationDuration);

                    }
        }

        private int GetAxisValue(int x, int y, int z)
        {
            return _axis switch
            {
                Axis.X => x,
                Axis.Y => y,
                Axis.Z => z,
                _ => 0
            };
        }

        private Vector3 GetAxisVector()
        {
            return _axis switch
            {
                Axis.X => Vector3.right,
                Axis.Y => Vector3.up,
                Axis.Z => Vector3.forward,
                _ => Vector3.zero
            };
        }
    }
}

