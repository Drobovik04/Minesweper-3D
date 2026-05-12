using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace Assets.Scripts.Core
{
    public class MenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private GameObject _setupPanel;

        [Header("UI Settings")]
        [SerializeField] private Toggle _vibrationToggle;

        [Header("UI Game Settings")]
        [SerializeField] private TMP_InputField _sizeInput;
        [SerializeField] private TMP_InputField _minesInput;
        [SerializeField] private TextMeshProUGUI _errorText;

        [Header("UI Record")]
        [SerializeField] private TextMeshProUGUI _recordText;
        private RecordService _recordService;

        private void Awake()
        {
            GameSettings.Load();

            _vibrationToggle.isOn = GameSettings.VibrationEnabled;
            _sizeInput.text = GameSettings.FieldSize.ToString();
            _minesInput.text = GameSettings.MineCount.ToString();
            _errorText.text = "";

#if UNITY_WEBGL && !UNITY_EDITOR
            _mainMenuPanel.transform.Find("Settings").gameObject.SetActive(false);
#else

#endif

            ShowPanel(_mainMenuPanel);

            UpdateRecordDisplay();
        }

        [Inject]
        public void Inject(RecordService recordService)
        {
            _recordService = recordService;
        }

        private void UpdateRecordDisplay()
        {
            if (_recordText != null && _recordService != null)
                _recordText.text = _recordService.GetRecordDisplay();
        }

        public void OpenSettings() => ShowPanel(_settingsPanel);
        public void OpenSetup() => ShowPanel(_setupPanel);
        public void BackToMain() => ShowPanel(_mainMenuPanel);

        public void OnVibrationToggleChanged(bool value)
        {
            GameSettings.VibrationEnabled = value;
            GameSettings.Save();
        }

        public void OnStartGame()
        {
            if (int.TryParse(_sizeInput.text, out int size) &&
                int.TryParse(_minesInput.text, out int mines))
            {
                if (ValidateSetup(size, mines))
                {
                    GameSettings.FieldSize = size;
                    GameSettings.MineCount = mines;
                    GameSettings.Save();

                    SceneManager.LoadScene("Game");
                }
                else
                {
                    _errorText.text = YG.YG2.lang == "en" ? "Size: 3-8 | Mines: 1-" + (size * size * size - 1) 
                        : "Размер: 3-8 | Кол-во мин: 1-" + (size * size * size - 1);
                }
            }
            else
            {
                _errorText.text = YG.YG2.lang == "en" ? "Enter correct numbers"
                    : "Введите корректное число";
            }
        }

        private bool ValidateSetup(int size, int mines)
        {
            int maxCells = size * size * size;
            return size >= 3 && size <= 8 &&
                   mines >= 1 && mines < maxCells;
        }

        private void ShowPanel(GameObject panel)
        {
            _mainMenuPanel.SetActive(false);
            _settingsPanel.SetActive(false);
            _setupPanel.SetActive(false);
            panel.SetActive(true);
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

