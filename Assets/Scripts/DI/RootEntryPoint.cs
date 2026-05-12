using Assets.Scripts.Ads;
using Assets.Scripts.Data;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace Assets.Scripts.DI
{
    public class RootEntryPoint : IStartable
    {
        private readonly DataControlService _dataControlService;
        private readonly AdsManager _adsManager;

        public RootEntryPoint(DataControlService dataControlService, AdsManager adsManager)
        {
            _dataControlService = dataControlService;
            _adsManager = adsManager;
        }

        public void Start()
        {
            Debug.Log("Bootstrap started");

            _dataControlService.Load();

            Debug.Log("Save loaded, switching scene");
#if UNITY_ANDROID
            _adsManager.Initialize();
#endif

            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }
}
