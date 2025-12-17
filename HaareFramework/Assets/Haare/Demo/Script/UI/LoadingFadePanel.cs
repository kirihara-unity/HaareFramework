using System.Threading;
using Cysharp.Threading.Tasks;
using Haare.Client.Routine;
using Haare.Client.UI;
using Haare.Scripts.Client.Data;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using Unit = R3.Unit;

namespace Demo.UI
{
    
    [PanelAttribute("Prefabs/Demo_LoadingFadePanel")]
    public class LoadingFadePanel : MonoRoutine,ICustomPanel
    {
        [SerializeField] public CustomImage FadeImage;
        private ICustomPanel _customPanelImplementation;
        public SceneUIManager uiManager { get; set; }
        public GameObject panel { get; set; }
        
        public Subject<Unit> OnFinishedFade { get; }= new Subject<Unit>();

        public override async UniTask Initialize(CancellationToken cts)
        {
            await base.Initialize(cts);
            FadeImage.ChangeAlpha(0);
            await UniTask.CompletedTask;
        }

        public async UniTask FadeIn(float duration = 0.4f)
        {
            await FadeImage.Fade(0f, 1f,duration);
            OnFinishedFade.OnNext(Unit.Default);
        }
        public async UniTask FadeOut(float duration = 0.4f)
        {
            await FadeImage.Fade(1f, 0f, duration);
            OnFinishedFade.OnNext(Unit.Default);
        }
        
        public void BindEvent()
        { }

        public void BindEvent(IDataInstance data)
        { }
        public void SetData(IDataInstance data)
        { }

        public void OpenPanel()
        {
            this.gameObject.SetActive(true);
            panel = this.gameObject;
        }

        public void ClosePanel()
        {
            
        }
    }
}