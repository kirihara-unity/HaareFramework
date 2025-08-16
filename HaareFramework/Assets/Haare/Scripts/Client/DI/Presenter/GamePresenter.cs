﻿using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

using Haare.Client.Routine.Service.SceneService;
using Haare.Util.LogHelper;


namespace Haare.Client.Core.DI
{
    public class GamePresenter : IPostInitializable, IDisposable
    {
        [Inject]
        private SceneRoutine _SceneRoutine;
        
        public void Dispose()
        {
        }

        public void PostInitialize()
        {
            _SceneRoutine.CurrentPhase.AsObservable()
                .Skip(1)
                .Subscribe(_ =>
            {
                LogHelper.Log(LogHelper.FRAMEWORK,"Ended Loading"+_);
                if (_ != SceneLoadPhase.EndLoad)
                    return;
                Processer.Instance.CheckDeleteProcessesForScene().Forget();
                LogHelper.Log(LogHelper.FRAMEWORK,"Ended Clean Scene");
            });
            LogHelper.Log(LogHelper.FRAMEWORK,"GamePresenter PostInitialize");
            
            
        }
    }
}