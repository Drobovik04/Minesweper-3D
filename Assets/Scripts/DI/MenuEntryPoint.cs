using Assets.Scripts.Core;
using Assets.Scripts.Data;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;
using YG;

namespace Assets.Scripts.DI
{
    public class MenuEntryPoint : IStartable
    {
        private readonly DataControlService _dataControlService;
        private readonly RecordService _recordService;

        public MenuEntryPoint(RecordService recordService, DataControlService dataControlService)
        {
            _recordService = recordService;
            _dataControlService = dataControlService;
        }

        public void Start()
        {
            Debug.Log("Menu started");
            var settings = _dataControlService.Current.GameSettings;

            YG2.GameReadyAPI();
        }
    }
}
