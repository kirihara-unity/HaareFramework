using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Haare.Client.Core;
using Haare.Util.LogHelper;

namespace Haare.Client.Routine
{
    /// <summary>
    ///  Gameobject에 할당이 가능한 Routine
    /// 일반적인 MonoBehaviour 와 동일한 역할입니다.
    /// </summary>
    public class MonoRoutine : MonoBehaviour ,IRoutine
    {
        public CancellationTokenSource _cts { get; private set; }
        
        public virtual bool isRegistered => true;
        public virtual bool isInSceneOnly { get; protected set; } = true;
        public bool isInitialized { get; private set; }
        public Func<CancellationToken,UniTask> Oninitialize { get; protected set; } = async (cts) => await UniTask.CompletedTask;
        public Func<UniTask> Onfinalize { get; protected set;} = async () => await UniTask.CompletedTask;
        public Subject<Unit> Onupdate { get; protected set; } = new Subject<Unit>();
        public Subject<Unit> OnLateupdate { get; protected set; } = new Subject<Unit>();
        public Subject<Unit> OnFixedupdate { get; protected set; } = new Subject<Unit>();
        
        public CompositeDisposable disposables = new CompositeDisposable();
        private bool _isFinalized = false; 

        private void Awake()
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            
            if (!isRegistered) { return; } 
            //LogHelper.Log(LogHelper.DEMO,$"이거불러지는중?{this.GetType()}");
            // 비동기 초기화 로직을 안전하게 호출
            InitializeAsync(_cts.Token).Forget();
        }
        protected async UniTask InitializeAsync(CancellationToken cts)
        {
            try
            {
                Constructor();

                await UniTask.WaitUntil(
                    () => Processer.Instance.isInitialized
                    , cancellationToken : cts);

                await Processer.Instance.Register(this,cts);

                disposables.Add(Processer.Instance.PROCESSING.Subscribe(_ =>
                {
                    if (_)
                    {
                        OnRestartProcess();
                    }
                    else
                    {
                        OnStopProcess();
                    }
                }));


                disposables.Add(Processer.Instance.Onupdate.Subscribe(_ => { UpdateProcess(); }));
                disposables.Add(Processer.Instance.OnLateupdate.Subscribe(_ => { LateUpdateProcess(); }));
                disposables.Add(Processer.Instance.OnFixedupdate.Subscribe(_ => { FixedUpdateProcess(); }));
            }
            catch (OperationCanceledException exception)
            {
                LogHelper.Log(LogHelper.CANCELLED, 
                    $"{this.GetType()} initialization was canceled. by {exception}");
            }
            catch (Exception ex)
            {
                LogHelper.Error(LogHelper.FRAMEWORK, $"An error occurred during {this.GetType()} initialization: {ex}");
            }
        }
        
        
        protected virtual void Constructor() {
            
        }
        public virtual async UniTask Initialize(CancellationToken cts)
        {
            await Oninitialize(cts);
            isInitialized = true;
        }
        protected virtual void OnStopProcess()
        {
            
        }
        protected virtual void OnRestartProcess()
        {
            
        }
        protected virtual void UpdateProcess() {
            Onupdate.OnNext( Unit.Default );
        }
        protected virtual void LateUpdateProcess() {
            OnLateupdate.OnNext( Unit.Default );
        }  
        protected virtual void FixedUpdateProcess() {
            OnFixedupdate.OnNext( Unit.Default );
        }
        
        public virtual async UniTask Finalize()
        {
            if (_isFinalized) return;
            _isFinalized = true;
            disposables.Clear();
            await Onfinalize();
            //LogHelper.LogTask(LogHelper.FRAMEWORK,this.GetType()+"MonoRoutine Disposed");
        }
        private void OnApplicationQuit()
        {
            disposables.Clear();
        }
        private void OnDestroy()
        {
            if (_isFinalized) return;
            
            if (Processer.isCreated)
            {
                Processer.Instance.UnRegister(this);
            }
            //LogHelper.Log(LogHelper.FRAMEWORK,this.GetType()+" Distroyed");
        }

 
    }
}