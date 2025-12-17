using System;
using System.Threading;
using Cysharp.Threading.Tasks;

using R3;
using UnityEngine;


using Haare.Client.Core;
using Haare.Util.LogHelper;

namespace Haare.Client.Routine
{
    public class NativeRoutine : INativeRoutine , IDisposable
    {
        private Func<CancellationToken, UniTask> _oninitialize;

        public CancellationTokenSource _cts { get; private set; }
            = new CancellationTokenSource();
        public virtual bool isRegistered => true;
        public virtual bool isInSceneOnly { get; protected set; }
        public bool isInitialized { get; private set; }

        Func<CancellationToken, UniTask> IRoutine.Oninitialize => _oninitialize;

        public Func<UniTask> Oninitialize { get; protected set;} = async () => await UniTask.CompletedTask;
        public Func<UniTask> Onfinalize { get; protected set;} = async () => await UniTask.CompletedTask;
        public Subject<Unit> Onupdate { get; protected set; } = new Subject<Unit>();
        
        protected NativeRoutine() {
            Constructor(_cts.Token).Forget();
        }
        async UniTask Constructor(CancellationToken cts) {
            if ( !isRegistered )	{ return; }

            try
            {
                //LogHelper.LogTask(LogHelper.FRAMEWORK, "Custruct of Native Routine");
                await UniTask.Delay(1, cancellationToken: cts);
                await UniTask.WaitUntil(() => Processer.Instance.isInitialized, cancellationToken: cts);
                await Processer.Instance.Register(this,cts);
            }
            catch (OperationCanceledException)
            {
                LogHelper.LogTask(LogHelper.FRAMEWORK, $"{this.GetType()} initialization was canceled.");
            }
            catch (Exception ex)
            {
                LogHelper.Error(LogHelper.FRAMEWORK, $"An error occurred during {this.GetType()} initialization: {ex}");
            }
        }
        
        public virtual async UniTask Initialize(CancellationToken cts)
        {
            await Oninitialize();
            isInitialized = true;
        }

        
        public virtual void UpdateProcess() {

        }
        
        public void Dispose()
        {
            Finalize().Forget();
        }
        
        public virtual async UniTask Finalize()
        {
            await Onfinalize();
        }
    }
}