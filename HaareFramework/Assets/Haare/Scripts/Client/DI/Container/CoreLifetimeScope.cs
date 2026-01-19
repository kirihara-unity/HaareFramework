using System;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using Haare.Client.Routine.Service.SceneService;
using Haare.Client.UI;
using Haare.Scripts.Client.Data;
using Haare.Util.Logger;
using UnityEngine;

namespace Haare.Client.Core.DI
{
    public class CoreLifetimeScope : LifetimeScope 
    {
        [SerializeField]
        private CoreUIManager _coreUIManagerPrefab;
        

        private bool isLocalMode = true;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<DataManager>(Lifetime.Singleton).As<DataManager>().AsSelf();
            builder.Register<SceneService>(Lifetime.Singleton).As<SceneService>().AsSelf();
            
            builder.RegisterComponentInNewPrefab(_coreUIManagerPrefab, Lifetime.Singleton)
                .DontDestroyOnLoad() // 씬 전환 시 파괴되지 않도록 설정
                .AsSelf();           // CoreUIManager 타입으로 등록
            
            
            builder.RegisterEntryPoint<GamePresenter>();
            LogHelper.Log(LogHelper.FRAMEWORK,"GameProcessorScope start");
        }
    }
}