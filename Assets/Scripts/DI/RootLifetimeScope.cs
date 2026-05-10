using Assets.Scripts.Ads;
using Assets.Scripts.Core;
using Assets.Scripts.Data;
using Assets.Scripts.DI;
using Assets.Scripts.Managers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Scripts.DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private AdsManager _adsManager;
        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SaveManager>(Lifetime.Singleton);
            builder.Register<RecordService>(Lifetime.Singleton);
            builder.Register<DataControlService>(Lifetime.Singleton);
            builder.RegisterComponent(_adsManager);

            builder.RegisterEntryPoint<RootEntryPoint>();
        }
    }
}
