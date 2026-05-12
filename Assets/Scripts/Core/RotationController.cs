using Assets.Scripts.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

namespace Assets.Scripts.Core
{
    public class RotationController : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        [SerializeField] private float _speed = 0.2f;
        [SerializeField] private float _zoomSpeed = 0.2f;
        [SerializeField] private float _minZoom = 1f;
        [SerializeField] private float _maxZoom = 8f;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private PlayerInput _playerInput;
        private Camera _camera;

        private Vector2 _delta;
        private bool _isDragging;
        private bool _isPinching;
        private float _pinchDelta;
        private FieldView _fieldView;
        private FieldService _fieldService;
        private SliceController _sliceController;
        private RecordService _recordService;
        private Vector2 _cachedPointerPos;
        private float _lastSliceTime;
        private float _sliceCooldown = 0.25f;
        private float _gameStartTime;

        private InputActionMap _gameplayMap;
        private InputAction _rotateAction;
        private InputAction _clickAction;
        private InputAction _zoomAction;
        private InputAction _touchPressAction;

        public void Awake()
        {
            if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();

            _gameplayMap = _playerInput.actions.FindActionMap("Gameplay");

            _rotateAction = _gameplayMap.FindAction("Rotate");
            _clickAction = _gameplayMap.FindAction("Click");
            _zoomAction = _gameplayMap.FindAction("Zoom");
            _touchPressAction = _gameplayMap.FindAction("TouchPress");

            _gameStartTime = Time.time;
        }

        private void OnEnable()
        {
            GameEvents.OnGameFinished += DisableGameInput;
            GameEvents.OnGameReset += EnableGameInput;
            GameEvents.OnFullscreenAdActive += HandleFullscreenAdActive;
            GameEvents.OnInfoShow += () => DisableGameInput(GameResult.Win); // странно, но вроде ничего не сломает потенциально
            GameEvents.OnInfoHide += () => EnableGameInput();
            EnableGameInput();
        }

        private void OnDisable()
        {
            GameEvents.OnGameFinished -= DisableGameInput;
            GameEvents.OnGameReset -= EnableGameInput;
            GameEvents.OnFullscreenAdActive -= HandleFullscreenAdActive;
            GameEvents.OnInfoShow -= () => DisableGameInput(GameResult.Win);
            GameEvents.OnInfoHide -= () => EnableGameInput();
        }

        private void HandleFullscreenAdActive(bool active)
        {
            if (active) DisableGameInput(GameResult.Lose);
            else EnableGameInput();
        }

        private void DisableGameInput(GameResult _)
        {
            _rotateAction?.Disable();
            _clickAction?.Disable();
            _zoomAction?.Disable();
            _touchPressAction?.Disable();
        }

        private void EnableGameInput()
        {
            _rotateAction?.Enable();
            _clickAction?.Enable();
            _zoomAction?.Enable();
            _touchPressAction?.Enable();
        }

        [Inject]
        public void Inject(Camera camera, SliceController sliceController, FieldView fieldView, FieldService fieldService, RecordService recordService)
        {
            _camera = camera;
            _sliceController = sliceController;
            _fieldView = fieldView;
            _fieldService = fieldService;
            _recordService = recordService;
        }

        public void OnRotate(InputValue value) => _delta = value.Get<Vector2>();
        public void OnTouchPress(InputValue value) => _isDragging = value.isPressed;
        public void OnZoom(InputValue value)
        {
            var scroll = value.Get<float>();

            if (Mathf.Abs(scroll) > 0.01f)
            {
                Zoom(scroll);
            }
        }

        private void Update()
        {
#if UNITY_WEBGL
            //_isDragging = IsPointerPressed();
#endif

            if (Mouse.current != null)
                _cachedPointerPos = Mouse.current.position.ReadValue();
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                _cachedPointerPos = Touchscreen.current.primaryTouch.position.ReadValue();

            HandlePinch();
            HandleRotate();
        }

        private void HandleRotate()
        {
            if (!_isDragging) return;

            _root.Rotate(Vector3.up, -_delta.x * _speed, Space.World);
            _root.Rotate(Vector3.right, _delta.y * _speed, Space.World);
        }

        private void HandlePinch()
        {
            var touches = Touchscreen.current.touches;

            if (touches.Count < 2)
            {
                _isPinching = false;
                return;
            }

            var t1 = touches[0];
            var t2 = touches[1];

            if (!t1.press.isPressed || !t2.press.isPressed)
            {
                _isPinching = false;
                return;
            }

            float currentDistance = Vector2.Distance(t1.position.ReadValue(), t2.position.ReadValue());

            if (!_isPinching)
            {
                _pinchDelta = currentDistance;
                _isPinching = true;
                return;
            }

            float delta = currentDistance - _pinchDelta;

            Zoom(delta * 0.01f);

            _pinchDelta = currentDistance;
        }

        public void ZommClick(float value)
        {
            Zoom(value);
        }

        private void Zoom(float value)
        {
            float newSize = _camera.orthographicSize - value * _zoomSpeed;

            _camera.orthographicSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);
        }

        public void OnClick(InputValue value)
        {

            if (Time.time - _lastSliceTime < _sliceCooldown)
                return;

            Vector2 pointerPos = Vector2.zero;

            if (Pointer.current != null)
                pointerPos = Pointer.current.position.ReadValue();
            else if (Mouse.current != null)
                pointerPos = Mouse.current.position.ReadValue();
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                pointerPos = Touchscreen.current.primaryTouch.position.ReadValue();

            Debug.Log($"[OnClick] Position: {pointerPos}");

            if (pointerPos == Vector2.zero || float.IsNaN(pointerPos.x) || float.IsInfinity(pointerPos.x))
            {
                Debug.LogWarning("Position is invalid");
                return;
            }

            Ray ray = _camera.ScreenPointToRay(pointerPos);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

            if (Physics.Raycast(ray, out var hit, 100f, _layerMask))
            {
                Debug.Log($"[Raycast] Hit: {hit.collider.name} | Point: {hit.point}");
                var cell = hit.collider.GetComponent<CellView>();
                if (cell == null) return;

                if (cell.IsRevealed) return;

                if (!_sliceController.IsSliceActive)
                {
                    SelectSlice(hit);
                }
                else
                {
                    TryRevealCell(cell);
                }

                //_fieldService.Open(cell.Index.x, cell.Index.y, cell.Index.z);

                //SelectSlice(hit);
                _lastSliceTime = Time.time;
            }
            else
            {
                //Debug.Log("[Raycast] Miss, clear slice");
                //_sliceController.ClearSlice();
            }
        }

        private void SelectSlice(RaycastHit hit)
        {
            var cell = hit.collider.GetComponent<CellView>();

            if (cell == null) return;

            Vector3 camForward = _camera.transform.forward;
            float absX = Mathf.Abs(camForward.x);
            float absY = Mathf.Abs(camForward.y);
            float absZ = Mathf.Abs(camForward.z);

            Axis viewAxis;
            if (absX > absY && absX > absZ)
                viewAxis = Axis.X;
            else if (absY > absX && absY > absZ)
                viewAxis = Axis.Y;
            else
                viewAxis = Axis.Z;

            Vector3 normal = hit.normal;
            Axis sliceAxis;

            if (viewAxis == Axis.Z)
            {
                sliceAxis = Mathf.Abs(normal.x) > Mathf.Abs(normal.y) ? Axis.X : Axis.Y;
            }
            else if (viewAxis == Axis.X)
            {
                sliceAxis = Mathf.Abs(normal.y) > Mathf.Abs(normal.z) ? Axis.Y : Axis.Z;
            }
            else
            {
                sliceAxis = Mathf.Abs(normal.x) > Mathf.Abs(normal.z) ? Axis.X : Axis.Z;
            }

            int index = sliceAxis switch
            {
                Axis.X => cell.Index.x,
                Axis.Y => cell.Index.y,
                Axis.Z => cell.Index.z,
                _ => 0
            };

            _sliceController.SetSlice(sliceAxis, index);
        }

        private void TryRevealCell(CellView cell)
        {
            if (cell.IsRevealed) return;

            if (!IsCellOnActiveSlice(cell))
            {
                // _sliceController.SetSlice(_sliceController.CurrentAxis, cell.Index[...]);
                Debug.Log("[Reveal] Cell not on active slice, ignored");
                return;
            }

            var cellData = _fieldService.Get(cell.Index.x, cell.Index.y, cell.Index.z);

            if (cellData.IsMine)
            {
                //OnGameOver(cell, cellData);
                GameEvents.RaisePlayerHitMine(cell, cellData);
                return;
            }

            _fieldService.Open(cell.Index.x, cell.Index.y, cell.Index.z);

            SyncRevealedCells();

            _sliceController.MoveSlice(0); // чтобы перерисовать после открытия

            Debug.Log($"[Reveal] Cell {cell.Index} opened, adjacent mines: {cellData.AdjacentMines}");
        }

        private bool IsCellOnActiveSlice(CellView cell)
        {
            var (axis, index) = _sliceController.CurrentSlice;

            int cellValue = axis switch
            {
                Axis.X => cell.Index.x,
                Axis.Y => cell.Index.y,
                Axis.Z => cell.Index.z,
                _ => 0
            };

            return cellValue == index;
        }

        private void SyncRevealedCells()
        {
            if (_fieldView?.Views == null) return;

            var views = _fieldView.Views;
            int size = views.GetLength(0);
            int openedCount = 0;
            int totalCells = size * size * size;
            int totalMines = _fieldService.TotalMines;

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    for (int z = 0; z < size; z++)
                    {
                        var cellView = views[x, y, z];
                        var cellData = _fieldService.Get(x, y, z);

                        if (cellData.IsOpened && !cellView.IsRevealed)
                            cellView.Reveal(cellData);

                        if (cellData.IsOpened) openedCount++;
                    }

            if (openedCount == totalCells - totalMines)
            {
                int mines = GameSettings.MineCount;

                var clearTime = Time.time - _gameStartTime;

                bool isNewRecord = _recordService?.TryUpdateRecord(size, mines, clearTime) ?? false;

                GameEvents.RaiseGameFinished(GameResult.Win);
            }
        }

        private void OnGameOver(CellView explodedCell, CellData data)
        {
            _sliceController.ClearSlice();

            explodedCell.Reveal(data);

            var views = _fieldView.Views;
            int size = views.GetLength(0);

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    for (int z = 0; z < size; z++)
                    {
                        var cellData = _fieldService.Get(x, y, z);
                        if (cellData.IsMine)
                        {
                            var cellView = views[x, y, z];
                            if (cellView == explodedCell) continue;
                            if (!cellView.IsRevealed)
                                cellView.Reveal(cellData);
                        }
                    }

            Debug.Log("Game Over!");

            //if (GameSettings.VibrationEnabled)
            //    Handheld.Vibrate();

            GameEvents.RaiseGameFinished(GameResult.Lose);
        }

        private bool IsPointerPressed()
        {
            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
                return true;

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                return true;

            return false;
        }
    }
}
