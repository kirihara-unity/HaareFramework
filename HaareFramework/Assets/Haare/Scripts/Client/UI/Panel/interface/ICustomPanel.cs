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
        public void ClosePanel() {}
        public void ReloadPanel() {}


        sealed async UniTask SetUpTask(Func<UniTask> setData = null, Func<UniTask> bindEvent = null)
        {
            if(setData!=null)
                await setData();
            if(bindEvent!=null)
                await bindEvent();
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

    }
}