using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Haare.Scripts.Client.Data;
using UnityEngine;
using VContainer;

namespace Haare.Client.UI
{
    public interface ICustomPanel
    {
        public SceneUIManager uiManager { get; set; }
        public GameObject panel { get; set; }
        
        public void OpenPanel() {}

        public void ClosePanel()
        { }
        public void ReloadPanel() {}


        sealed async UniTask SetUpTask(
            Func<UniTask> setData = null, 
            Action bindEvent = null)
        {
            if(setData!=null)
                await setData();
            if(bindEvent!=null)
                bindEvent();
        }
        sealed async UniTask SetUpTask(
            Func<UniTask> setData = null,
            Func<UniTask> setDataWithData = null, 
            Func<UniTask> bindEventWithData = null, 
            Action bindEvent = null)
        {
            if(setData!=null)
                await setData();
            if(setDataWithData!=null)
                await setDataWithData();
            if(bindEventWithData!=null)
                await bindEventWithData();
            if(bindEvent!=null)
                bindEvent();
        }
        
        public void BindEvent() {}
        public virtual async UniTask BindEvent(IPanelData data)
        {
            await UniTask.CompletedTask;
        }
        public virtual async UniTask SetData(IPanelData data) 
        { 
            await UniTask.CompletedTask;
        }
        public virtual async UniTask SetData() 
        { 
            await UniTask.CompletedTask;
        }
        
    }
}