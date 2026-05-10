using Assets.Scripts.Ads;
using Assets.Scripts.Core;
using Assets.Scripts.Input;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Scripts.DI
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private FieldView _fieldView;
        [SerializeField] private SliceController _sliceController;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private FieldConfig _config;
        [SerializeField] private RotationController _rotationController;
        [SerializeField] private SliceUIController _sliceUIController;
        [SerializeField] private GameManager _gameManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<FieldService>(Lifetime.Singleton);
            builder.RegisterInstance(_sliceController);
            builder.RegisterInstance(_fieldView);
            builder.RegisterInstance(_mainCamera);
            builder.RegisterComponent(_rotationController);
            builder.RegisterInstance(_config);
            builder.RegisterInstance(_sliceUIController);
            builder.RegisterInstance(_gameManager);

            builder.RegisterEntryPoint<GameEntryPoint>();
        }
    }
}
