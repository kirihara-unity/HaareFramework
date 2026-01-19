
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

namespace Demo.LobbyScene
{
    public class DemoLobbyUIPresenter : IPresenter 
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
            LogHelper.Log(LogHelper.DEMO, $"{this.GetType()} Disposed");
        }

        public CompositeDisposable disposables { get;}  = new CompositeDisposable();

        public void PostInitialize()
        {
            StartSequence();
        }
        
        private void BindIPanel(ICustomPanel panel)
        {
            panel.uiManager = _sceneUiManager;
            panel.BindEvent();
        }

        private async UniTask StartSequence()
        {
            _coreUIManager.ClosePanel<LoadingFadePanel>();
            var fadepanelID = await _coreUIManager.LoadPanel<LoadingFadePanel>();
            var panel = _coreUIManager.RentPanel<LoadingFadePanel>(fadepanelID);
            await panel.FadeOut();
            _coreUIManager.ClosePanel<LoadingFadePanel>();
        }
    }
}