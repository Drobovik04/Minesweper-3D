using Assets.Scripts.Core;
using Assets.Scripts.Managers;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Assets.Scripts.View
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _subtitleText;
        [SerializeField] private Button _menuBtn;
        [SerializeField] private Button _restartBtn;
        [SerializeField] private Button _continueBtn;
        [SerializeField] private GameObject _panel;
        [SerializeField] private GameManager _gameManager;

        private Action _onContinueClicked;

        private void Awake()
        {
            _panel.SetActive(false);
            _continueBtn.gameObject.SetActive(false);
        }

        public void Show(GameResult result)
        {
            _panel.SetActive(true);

            if (result == GameResult.Win)
            {
                _titleText.text = YG2.lang == "en" ? "Win!" : "Победа!";
                _subtitleText.text = YG2.lang == "en" ? "All cells are open" : "Все клетки открыты";
            }
            else
            {
                _titleText.text = YG2.lang == "en" ? "Boom!" : "Взрыв!";
                _subtitleText.text = YG2.lang == "en" ? "You are stepped on a mine" : "Вы наступили на мину";
            }

            _menuBtn.gameObject.SetActive(true);
            _restartBtn.gameObject.SetActive(true);
            _continueBtn.gameObject.SetActive(false);

            EnableButtons();
        }

        private void EnableButtons()
        {
            _menuBtn.interactable = true;
            _restartBtn.interactable = true;
        }

        public void ShowWithContinueOption(string title, string subtitle)
        {
            _panel.SetActive(true);
            _titleText.text = title;
            _subtitleText.text = subtitle;

            // Показываем кнопку продолжения, скрываем обычные кнопки
            if (_continueBtn != null)
            {
                _continueBtn.gameObject.SetActive(true);
                //_menuBtn.gameObject.SetActive(false);
                //_restartBtn.gameObject.SetActive(false);
            }
        }

        public void Hide()
        {
            _panel.SetActive(false);
        }

        public void OnMenuClick() => _gameManager.ReturnToMenu();
        public void OnRestartClick() => _gameManager.RestartGame();
        public void OnShowAdsClick() => _gameManager.OnWatchAdClicked();
    }
}
