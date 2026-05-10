using Assets.Scripts.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Scripts.DI
{
    public class MenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private MenuController _menuController;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_menuController);

            builder.RegisterEntryPoint<MenuEntryPoint>();
        }
    }
}

