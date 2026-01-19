
using Cysharp.Threading.Tasks;
using Demo.UI;
using Haare.Client.Core.DI;

using R3;
using UnityEngine;
using VContainer;

using Haare.Client.Routine.Service.SceneService;
using Haare.Client.UI;
using Haare.Util.Logger;
using Unity.VisualScripting;

namespace Demo.LoadScene
{
    public class DemoLoadUIPresenter : IPresenter 
    {
        [Inject]
        private CoreUIManager _coreUIManager;
        [Inject]
        private SceneUIManager _sceneUiManager;
        [Inject]
        private readonly IObjectResolver _resolver;
        [Inject] 
        public SceneService sceneService;
        [Inject]
        private DemoLoadMono _loadMono;
        public void Dispose()
        {
            disposables.Dispose();
            LogHelper.Log(LogHelper.DEMO,$"{this.GetType()} Disposed");

        }

        public CompositeDisposable disposables { get;}  = new CompositeDisposable();

        public void PostInitialize()
        {
            disposables.Add(
                _sceneUiManager.OnLoadedPanel.AsObservable()
                .Select(panel => panel.ConvertTo(typeof(LoadingPanel)) as LoadingPanel)
                .Where(loadingpanel => loadingpanel != null) 
                .Subscribe(panel =>
                {
                    sceneService.LoadProgress.AsObservable().Subscribe(value =>
                    {
                        panel.LoadingSlider.SetValue(value);
                    });
                    panel.OpenPanel();
                    LoadStartSequence().Forget();
                    sceneService.LoadScene();
                }));
        }
        
        private void BindIPanel(ICustomPanel panel)
        {
            panel.uiManager = _sceneUiManager;
            panel.BindEvent();
        }

        private async UniTask LoadStartSequence()
        {
            var panel = _coreUIManager.RentPanel<LoadingFadePanel>();
            panel.OpenPanel();
            await panel.FadeOut();
            _coreUIManager.ClosePanel<LoadingFadePanel>();
        }
    }
}