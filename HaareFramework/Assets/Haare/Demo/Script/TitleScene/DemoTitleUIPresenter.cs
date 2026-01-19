using Cysharp.Threading.Tasks;

using Demo.UI;

using R3;
using UnityEngine;
using VContainer;

using Haare.Client.Core.DI;
using Haare.Client.Routine.Service.SceneService;
using Haare.Client.UI;
using Haare.Util.Logger;
using Unity.VisualScripting;

namespace Demo.TitleScene
{
    public class DemoTitleUIPresenter : IPresenter
    {
        [Inject]
        private CoreUIManager _coreUIManager;
        [Inject]
        private SceneUIManager _sceneUiManager;
        [Inject]
        private readonly IObjectResolver _resolver;
        [Inject] 
        public SceneService sceneService;

        public void Dispose()
        {
            disposables.Dispose();
            LogHelper.Log(LogHelper.DEMO,$"{this.GetType()}Disposed");

        }

        public CompositeDisposable disposables { get;}  = new CompositeDisposable();

        public void PostInitialize()
        {
            
            disposables.Add(
                _sceneUiManager.OnLoadedPanel.AsObservable()
                .Select(panel => panel.ConvertTo(typeof(TitlePanel)) as TitlePanel)
                .Where(fadepanel => fadepanel != null) 
                .Subscribe(panel =>
                {
                    panel.StartButton.Onclicked.AsObservable().Subscribe(_ =>
                    {
                        StartGameSequence().Forget();
                    });
                    panel.OpenPanel();
                }));

            
        }

        private async UniTask StartGameSequence()
        {
            var loadingPanelID = await _coreUIManager.LoadPanel<LoadingFadePanel>(null,false, true);
            var loadingPanel = _coreUIManager.RentPanel<LoadingFadePanel>(loadingPanelID);
            loadingPanel.OpenPanel();
            await loadingPanel.FadeIn();

            OnFinishedFadePanel(); 
        }
        
        private void OnFinishedFadePanel()
        {
             sceneService.LoadSceneWithLoad(SceneName.DemoLobbyScene).Forget();
        }
        
        private void BindIPanel(ICustomPanel panel)
        {
            panel.uiManager = _sceneUiManager;
            panel.BindEvent();
        }
    }
}