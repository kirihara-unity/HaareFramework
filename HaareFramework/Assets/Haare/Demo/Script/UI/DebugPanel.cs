﻿using Haare.Client.Routine;
using Haare.Client.UI;
using Haare.Util.LogHelper;
using R3;
using UnityEngine;

namespace Demo.UI
{
    public class DebugPanel : MonoRoutine,ICustomPanel
    {
        [SerializeField] public CustomButton CloseButton;
        [SerializeField] public CustomImage background;
        [SerializeField] public CustomText StatusLog;
        
       
        private ICustomPanel _customPanelImplementation;

        public void BindEvent()
        {
            CloseButton.Onclicked.AsObservable()
                .Subscribe(_ =>
            {
                uiManager.ClosePanel<DebugPanel>();
            });
        }

        public SceneUIManager uiManager { get; set; }
        public GameObject panel { get; set; }

        public void OpenPanel()
        {
            this.gameObject.SetActive(true);
            panel = this.gameObject;
        }

        public void ClosePanel()
        {
            LogHelper.Log(LogHelper.DEMO,"ClosePanel");
            Destroy(this.gameObject);
        }
    }
}