using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Demo.UI;
using R3;
using UnityEngine;
using VContainer;
using Haare.Client.UI;
using Haare.Util.Logger;
using Unity.VisualScripting;


namespace Haare.Client.Core.DI
{
    public abstract class UIPresenter : IPresenter
    {
        public CompositeDisposable disposables { get; } = new CompositeDisposable();
        public bool isInitialized { get; private set; } = false;
        
        [Inject] protected CoreUIManager _coreUIManager;
        [Inject] protected SceneUIManager _sceneUiManager;
        [Inject] protected readonly IObjectResolver _resolver;

        public virtual void Dispose()
        {
            disposables.Dispose();
            LogHelper.Log(LogHelper.FRAMEWORK, $"{this.GetType()} Disposed");
        }

        public virtual void PostInitialize()
        { }

        protected virtual async UniTask PostInitializeAsync()
        {
            await Task.CompletedTask;
            isInitialized = true;
        }

        protected virtual void BindPanelEvents()
        {
            
        }

        protected void BindPanelEvents<TPanel>(
            Subject<ICustomPanel> sourceObservable, Action<TPanel> onPanelReady)
            where TPanel : class, ICustomPanel
        {
            
            sourceObservable
                .Select(panel => panel.ConvertTo(typeof(TPanel)) as TPanel)
                .Where(panel => panel != null)
                .Subscribe(onPanelReady).AddTo(disposables);;
        }

        protected async UniTask OpenPanelWithFade<T>() where T : Component, ICustomPanel
        {
            await FadeIn();
            await _sceneUiManager.LoadPanel<T>();
            await FadeOut();
        }

        protected async UniTask ClosePanelWithFade<T>() where T : Component, ICustomPanel
        {
            await FadeIn();
            _sceneUiManager.ClosePanel<T>();
            await FadeOut();
        }

        // UI 초동처리 종료 후 반드시 불러올것
        protected virtual async UniTask StartSequence()
        {
            await UniTask.WaitUntil(() => isInitialized);
            if(_sceneUiManager.ILoadedScene)
                await FadeOut();
        }
        protected async UniTask FadeIn()
        {
            LogHelper.Log("FADEIN");
            await _coreUIManager.LoadPanel<LoadingFadePanel>();
            var fadepanel = _coreUIManager.RentPanel<LoadingFadePanel>();
            fadepanel.OpenPanel();
            await fadepanel.FadeIn();
        }

        protected async UniTask FadeOut()
        {
            LogHelper.Log("FADEOUT");
            var fadepanel = _coreUIManager.RentPanel<LoadingFadePanel>();
            await fadepanel.FadeOut();
            _coreUIManager.ClosePanel<LoadingFadePanel>();
        }
    }
}
