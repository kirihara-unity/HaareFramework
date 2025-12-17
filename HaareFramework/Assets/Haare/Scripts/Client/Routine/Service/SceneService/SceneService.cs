using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Demo.UI;
using Haare.Client.UI;
using Haare.Util.Loader;
using Haare.Util.LogHelper;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using VContainer;

namespace Haare.Client.Routine.Service.SceneService
{   
    public enum SceneName
    {
        DemoTitleScene,
        DemoLoadScene,
        DemoLobbyScene
    }
    
    public enum SceneLoadPhase
    {
        UnloadCurrent,
        StartLoad,
        Loading,
        EndLoad
    }
    
    /// <summary>
    /// Scene의 변경에 대한 모든 관리가 진행되는 Routine -> Singleton입니다
    /// </summary>
    public class SceneService : NativeRoutine
    {  
        [Inject]
        private CoreUIManager _coreUIManager;
        
        SceneName sceneToUnload;
        
        public override bool isInSceneOnly => false;

        private readonly ReactiveProperty<SceneLoadPhase> currentPhaseReactive = new ReactiveProperty<SceneLoadPhase>(SceneLoadPhase.UnloadCurrent);

        public ReactiveProperty<SceneLoadPhase> CurrentPhase => currentPhaseReactive;
        
        private readonly ReactiveProperty<float> _loadProgress = new ReactiveProperty<float>(0f);
        public ReadOnlyReactiveProperty<float> LoadProgress => _loadProgress;

        public SceneLoadRequest LoadSceneRequest;
        /// <summary>
        /// 초기화
        /// </summary>
        public override async UniTask Initialize(CancellationToken cts)
        {
            await base.Initialize(cts);
            LogHelper.LogTask(LogHelper.FRAMEWORK,"SceneRoutine Initialize");
            await UniTask.CompletedTask;
        }
        
        /// <summary>
        /// Load를 끼는 씬이동
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        public async UniTask LoadSceneWithLoad(SceneName scene, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            SceneLoadRequest req = new SceneLoadRequest(SceneName.DemoLoadScene,mode,scene);
            LoadSceneRequest = new  SceneLoadRequest(scene,mode);
            await LoadSceneInternal(req,false);
        }
        /// <summary>
        /// 직접적 씬이동 (LoadScene이 아닌이상 비추천)
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        public async void LoadScene()
        {
            await LoadSceneInternal(LoadSceneRequest,true);
        }
        
        /// <summary>
        /// Scene 이동의 공통처리
        /// </summary>
        /// <param name="request"></param>
        private async UniTask LoadSceneInternal(SceneLoadRequest request,bool withLoad)
        {
            LogHelper.LogTask(LogHelper.FRAMEWORK,">>> Phase: StartLoad");
            currentPhaseReactive.Value = SceneLoadPhase.StartLoad;

            if(request.Mode==LoadSceneMode.Additive){
                if (Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, out var initialScene))
                {
                     sceneToUnload = initialScene;
                }
            }
            
            
            LogHelper.LogTask(LogHelper.FRAMEWORK,">>> Phase: Loading");
            
            currentPhaseReactive.Value = SceneLoadPhase.Loading;

            var loadOperation = Addressables.LoadSceneAsync(
                AssetPath.SCENE_PATH+request.Scene.ToString()+AssetPath.SCENE_EXT, // "주소"로 씬을 찾음 (경로가 아님)
                request.Mode,
                activateOnLoad: false);

            if(withLoad){
                var loadSceneTask = LoadSceneProgressTask(loadOperation);
                var minTimeTask = FakeLoadingProgressTask(1.5f);
                await UniTask.WhenAll(loadSceneTask , minTimeTask);
                await ExitLoadingSceneTask();
            }
            else
            {
                await LoadSceneProgressTask(loadOperation);
            }
            await UniTask.Delay(1000); // 1초 대기
            
            var sceneInstance = loadOperation.Result;
            await sceneInstance.ActivateAsync();
            OnSceneLoadedHandler(sceneInstance.Scene, request.Argument);
            
            
            LogHelper.LogTask(LogHelper.FRAMEWORK,">>> UnLoadCurrent Scene");
            
            if (request.Mode == LoadSceneMode.Additive && 
                SceneManager.GetSceneByName(sceneToUnload.ToString()).isLoaded)
            {
                await SceneManager.UnloadSceneAsync(sceneToUnload.ToString());
            }
            
            LogHelper.LogTask(LogHelper.FRAMEWORK,">>> Phase: EndLoad");
            currentPhaseReactive.Value = SceneLoadPhase.EndLoad;
            
        }

        private async UniTask ExitLoadingSceneTask()
        {
            var loadingPanelID = await _coreUIManager.LoadPanel<LoadingFadePanel>(null,false, true);
            var loadingPanel = _coreUIManager.RentPanel<LoadingFadePanel>(loadingPanelID);
            await loadingPanel.FadeIn();
        }
        
        private async UniTask LoadSceneProgressTask(AsyncOperationHandle loadOperation)
        {
            while (loadOperation.PercentComplete  < 0.9f)
            {
                var progress = loadOperation.PercentComplete  / 0.9f;
                //_loadProgress.Value = progress;

                await UniTask.Yield();
            } 
            //_loadProgress.Value = 1f; 
        }
        /// <summary>
        /// 지정된 시간 동안 _loadProgress 값을 0에서 1로 부드럽게 증가시키는 Task입니다.
        /// </summary>
        private async UniTask FakeLoadingProgressTask(float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _loadProgress.Value = Mathf.Clamp01(elapsedTime / duration);
                await UniTask.Yield();
            }
            _loadProgress.Value = 1f;
        }
        
        /// <summary>
        /// Scene이 새로 로드되었을때 ISceneWasLoaded 상속클래스들의 공통 트리거 마련
        /// </summary>
        /// <param name="loadedscene"></param>
        /// <param name="Argument"></param>
        private void OnSceneLoadedHandler(Scene loadedscene, object Argument)
        {
            foreach (var root in loadedscene.GetRootGameObjects())
            {
                ExecuteEvents.Execute<ISceneWasLoaded>(root, null,
                    (receiver, e) => receiver.OnSceneWasLoaded(Argument));
            }
        }
    }
        
}
