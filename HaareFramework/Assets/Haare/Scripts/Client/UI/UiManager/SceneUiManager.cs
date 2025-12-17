using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Haare.Client.Routine;
using Haare.Client.Routine.Service.SceneService;
using Haare.Util.Loader;
using Haare.Util.LogHelper;
using R3;
using UnityEngine;

namespace Haare.Client.UI
{
    public abstract class SceneUIManager : MonoRoutine, ISceneWasLoaded
    {
        // UI Manager 가 시현한 Panel들
        private readonly Dictionary<PanelType, ICustomPanel> PanelDic =
            new Dictionary<PanelType, ICustomPanel>();

        // UI Manager 가 시현중인 Panel이지만 Stack로 관리되는 개체들
        private readonly Stack<PanelType> TypePanelStack = new Stack<PanelType>();

        public Subject<ICustomPanel> OnLoadedPanel { get; } = new Subject<ICustomPanel>();
        public Subject<ICustomPanel> OnReLoadedPanel { get; } = new Subject<ICustomPanel>();

        public bool ILoadedScene = false;

        [SerializeField] private RectTransform safePannelRect;

        public override UniTask Initialize(CancellationToken cts)
        {
            //ApplySafeArea();
            return base.Initialize(cts);
        }

        private void OnValidate()
        {
            if (safePannelRect == null)
            {
                // 콘솔에 경고 메시지를 출력합니다.
                LogHelper.Warning(LogHelper.FRAMEWORK, "safePannelRect is not Found!!");
            }
        }

        private void ApplySafeArea()
        {
            if (safePannelRect == null)
            {
                return;
            }

            Rect safeArea = Screen.safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            safePannelRect.anchorMin = anchorMin;
            safePannelRect.anchorMax = anchorMax;
            LogHelper.Log(LogHelper.FRAMEWORK, "safePannelRect Fixed");

        }

        /// <summary>
        /// ICustomPanel의 파생을 가져오기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T RentPanel<T>(int instanceID = 0) where T : Component, ICustomPanel
        {
            PanelType key;
            if (instanceID != 0)
            {
                key = GetKeybyinstanceID<T>(instanceID);
            }
            else
            {
                key = GetKeybyinstanceID<T>();
            }

            if (key == null)
            {
                return null;
            }

            var findRentPanel = typeof(T);
            return PanelDic[key] as T;

        }

        /// <summary>
        /// Register Panel!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private UniTask<T> Register<T>() where T : Component, ICustomPanel
        {
            var pageTypeToRegister = typeof(T);
            var panel = pageTypeToRegister.GetCustomAttribute<PanelAttribute>();
            var component = AssetLoader.InstantiatePrefab<T>(
                safePannelRect.transform, panel.AddressablePath);
            return component;
        }

        /// <summary>
        /// get PeekPanel Type Information
        /// </summary>
        /// <returns></returns>
        public ICustomPanel PeekPanel()
        {
            if (TypePanelStack.Count == 0)
            {
                return null;
            }

            return PanelDic[TypePanelStack.Peek()];
        }

        /// <summary>
        /// LoadPanel Only
        /// </summary>
        /// <param name="onCompletedTask"> on Completed Load Task Then Bind Event From Presenter </param>
        /// <param name="isOverlay"></param>
        /// <typeparam name="T"></typeparam>
        public async UniTask<int> LoadPanel<T>(
            Func<UniTask> onCompletedTask = null, bool isOverlay = false, bool IsStack = true) where T : Component, ICustomPanel
        {
            var panel = await LoadPanel<T>(isOverlay, IsStack);
            
            if (onCompletedTask != null)
            {
                await onCompletedTask();
            }
            
            OnLoadedPanel.OnNext(panel);
            
            return panel.GetInstanceID();
        }

        /// <summary>
        /// LoadPanel With PanelData
        /// </summary>
        /// <param name="onCompletedTask"></param>
        /// <param name="isOverlay"></param>
        /// <param name="IsStack"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public async UniTask<int> LoadPanel<Ttype,Tdata>(
            Tdata panelData, Func<UniTask> onCompletedTask = null, bool isOverlay = false, bool IsStack = true)
            where Ttype : Component, ICustomPanel where Tdata : IPanelData
        {
            var panel = await LoadPanel<Ttype>(isOverlay, IsStack);

            panel.BindEvent();
            await panel.SetUpTask(
                async ()=> await panel.SetData(panelData),
                async ()=> await panel.BindEvent(panelData)
                );
            
            if (onCompletedTask != null)
            {
                await onCompletedTask();
            }
            OnLoadedPanel.OnNext(panel);
            
            panel.OpenPanel();
            return panel.GetInstanceID();
        }

        private async UniTask<T> LoadPanel<T>(
            bool isOverlay = false, bool IsStack = true)  where T : Component, ICustomPanel
        {
            var pageType = typeof(T);

            // 1. 새 패널 가져오기 (없으면 생성)
            var panel = await Register<T>();
            int instanceID = panel.GetInstanceID();
            var panelType = new PanelType(pageType, instanceID, IsStack);
          
            panel.uiManager = this;
            panel.gameObject.SetActive(false);
            
            PanelDic.Add(panelType,panel);

            if(IsStack)
                TypePanelStack.Push(panelType);
            return panel;
        }
        
        
        /// <summary>
        /// ClosePanel
        /// </summary>
        public void ClosePanel<T>(bool isOverlay = false) where T : Component, ICustomPanel
        { 
            var panel = RentPanel<T>();
            if (panel == null)
            {
                return;
            }
            panel.ClosePanel();
            TypePanelStack.Pop();
            
            PanelDic.Remove(GetKeybyinstanceID<T>());
            if (TypePanelStack.Count == 0)
            {
                LogHelper.Log(LogHelper.FRAMEWORK,"Empty UI Panel");
            }
            else
            {
                PanelDic[TypePanelStack.Peek()].ReloadPanel();
                OnReLoadedPanel.OnNext(PanelDic[TypePanelStack.Peek()]);
                //LogHelper.Log("UI Stack Count"+TypePanelStack.Count);
            }
            if (PanelDic.Count == 0)
            {
                LogHelper.Log(LogHelper.FRAMEWORK,"Empty UI Dic");
            }
            else
            {
                // foreach (var kv in PanelDic)
                // {
                //     LogHelper.Log(($"DICT : {kv.Key} : {kv.Value}"));
                // }
            }
            
            Destroy(panel.panel);
        }
            
        /// <summary>
        /// ClosePeekPanel
        /// </summary>
        public void ClosePeekPanel()
        {
            var panel = PeekPanel();
            panel.ClosePanel();
            TypePanelStack.Pop();
            PanelDic.Remove(TypePanelStack.Peek()); 
            
            if (TypePanelStack.Count == 0)
            {
                LogHelper.Log(LogHelper.FRAMEWORK,"Empty UI Panel");
            }
            if (PanelDic.Count == 0)
            {
                LogHelper.Log(LogHelper.FRAMEWORK,"Empty UI Panel");
            }
            foreach (var item in TypePanelStack)
            {
                LogHelper.Log(LogHelper.FRAMEWORK,$"{item}");
            }
            foreach (var kv in PanelDic)
            {
                LogHelper.Log(LogHelper.FRAMEWORK,($"{kv.Key} : {kv.Value}"));
            }
            Destroy(panel.panel);
        }

        private PanelType GetKeybyinstanceID<T>(int instanceID) where T : Component, ICustomPanel
        {
            var matchingEntry = 
                PanelDic
                .Where(kv => kv.Key.pageType == typeof(T) && kv.Key.instanceId == instanceID)
                .LastOrDefault();
            
            return matchingEntry.Key;
        }
        private PanelType GetKeybyinstanceID<T>() where T : Component, ICustomPanel
        {
            var matchingEntry = PanelDic
                .Where(kv => kv.Key.pageType == typeof(T))
                .LastOrDefault();
            
            return matchingEntry.Key;
        }
        
        private class PanelType
        {
            public readonly Type pageType;
            public readonly bool isOverlay;
            public readonly int instanceId;
            public PanelType(Type _pageType,int _instanceId, bool _isOverlay )
            {
                pageType = _pageType;
                isOverlay = _isOverlay;
                instanceId = _instanceId;
            }

        }

        public void OnSceneWasLoaded(object argument)
        {  
            ILoadedScene = true;
        }
    }
}