using Assets.Scripts.Ads;
using Assets.Scripts.Core;
using Assets.Scripts.Data;
using Assets.Scripts.View;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using YG;

namespace Assets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameOverUI _gameOverUI;
        [SerializeField] private float _gameStartTime;

        private AdsManager _adsManager;
        private RecordService _recordService;

        private bool _continueUsed;
        private CellView _pendingExplodedCell;
        private CellData _pendingCellData;

        public void Init(AdsManager adsManager, RecordService recordService)
        {
            _adsManager = adsManager;
            _recordService = recordService;
            _gameStartTime = Time.time;
        }

        private void OnEnable() 
        { 
            GameEvents.OnGameFinished += HandleGameFinished;
            //GameEvents.OnGameReset += OnGameReset;
            GameEvents.OnPlayerHitMine += HandlePlayerHitMine;
            YG2.onCloseRewardedAdv += OnRewardedClosed;
            YG2.onErrorRewardedAdv += OnRewardedError;
        }
        private void OnDisable() 
        {
            GameEvents.OnGameFinished -= HandleGameFinished;
            //GameEvents.OnGameReset -= OnGameReset;
            GameEvents.OnPlayerHitMine -= HandlePlayerHitMine;
            YG2.onCloseRewardedAdv -= OnRewardedClosed;
            YG2.onErrorRewardedAdv -= OnRewardedError;
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu");
        }
        public void RestartGame()
        {
            Time.timeScale = 1f;
            GameEvents.RaiseGameReset();
            SceneManager.LoadScene("Game");
        }
        private void HandlePlayerHitMine(CellView cell, CellData data)
        {
            if (!_continueUsed && _adsManager != null)
            {
                Time.timeScale = 0f;

                _pendingExplodedCell = cell;
                _pendingCellData = data;
                _gameOverUI.ShowWithContinueOption("Boom!", "You are stepped on a mine. Continue?");

#if UNITY_ANDROID && !UNITY_EDITOR
                if (Core.GameSettings.VibrationEnabled)
                    Handheld.Vibrate();
#endif
            }
            else
            {
                GameEvents.RaiseGameFinished(GameResult.Lose);
            }
        }
        public void OnWatchAdClicked()
        {
#if UNITY_WEBGL
            YG2.RewardedAdvShow("revive", OnRewardGranted);
#else
            _adsManager.ShowRewarded(OnRewardGranted, OnAdFailed);
#endif
        }

        private void OnRewardedClosed()
        {
            if (!_continueUsed)
            {
                OnAdFailed();
            }
        }

        private void OnRewardedError()
        {
            Debug.LogWarning($"[YG2] Error: failed to reward");
            OnAdFailed();
        }

        private void OnGameReset()
        {
            _continueUsed = false;
            _gameStartTime = Time.time;
        }
        private void OnRewardGranted()
        {
            Time.timeScale = 1f;
            _continueUsed = true;

            _pendingExplodedCell = null;
            _pendingCellData = null;

            Debug.Log("[Continue] Player survived!");

            _gameOverUI.Hide();
        }
        private void OnAdFailed()
        {
            Time.timeScale = 1f;
            _continueUsed = true;
            GameEvents.RaiseGameFinished(GameResult.Lose);
        }
        private void HandleGameFinished(GameResult result)
        {
            if (result == GameResult.Win)
            {
                var clearTime = Time.time - _gameStartTime;
                int size = Core.GameSettings.FieldSize;
                int mines = Core.GameSettings.MineCount;
                _recordService.TryUpdateRecord(size, mines, clearTime);
            }

            if (result == GameResult.Lose)
            {
                Debug.Log("Game Over!");

                //if (Core.GameSettings.VibrationEnabled)
                //    Handheld.Vibrate();
            }

            _gameOverUI.Show(result);
        }
    }
}

