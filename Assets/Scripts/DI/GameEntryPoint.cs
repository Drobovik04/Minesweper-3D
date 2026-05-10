using Assets.Scripts.Ads;
using Assets.Scripts.Core;
using Assets.Scripts.Data;
using Assets.Scripts.Input;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;
using static UnityEngine.Rendering.STP;

namespace Assets.Scripts.DI
{
    public class GameEntryPoint : IStartable
    {
        private readonly FieldConfig _fieldConfig;
        private readonly FieldService _field;
        private readonly FieldView _view;
        private readonly SliceController _sliceController;
        private readonly SliceUIController _sliceUIController;
        private readonly GameManager _gameManager;
        private readonly AdsManager _adsManager;
        private readonly RecordService _recordService;

        public GameEntryPoint(FieldConfig fieldConfig, FieldService field, FieldView view,  SliceController sliceController, SliceUIController sliceUIController, GameManager gameManager, AdsManager adsManager, RecordService recordService)
        {
            _fieldConfig = fieldConfig;
            _field = field;
            _view = view;
            _sliceController = sliceController;
            _sliceUIController = sliceUIController;
            _gameManager = gameManager;
            _adsManager = adsManager;
            _recordService = recordService;
        }

        public void Start()
        {
            Debug.Log("Game started");

            Core.GameSettings.Load();

            int size = Core.GameSettings.FieldSize;
            int mines = Core.GameSettings.MineCount;

            if (Time.timeScale != 1f)
                Time.timeScale = 1f;

            //_field.Generate(_fieldConfig.size, _fieldConfig.mines);
            _field.Generate(size, mines);

            //_view.Build(_fieldConfig.size, _fieldConfig.gap);
            _view.Build(size, _fieldConfig.gap);
            _sliceController.Init(_view.Views, _field);
            _sliceUIController.Init(_sliceController);
            _gameManager.Init(_adsManager, _recordService);

            _view.AnimateSpawn();
        }

        public void GameOver()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
