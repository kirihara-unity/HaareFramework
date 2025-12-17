using Haare.Client.Routine;
using Haare.Client.UI;
using Haare.Scripts.Client.Data;
using Haare.Util.LogHelper;
using R3;
using UnityEngine;


namespace Demo.UI
{
    
    [PanelAttribute("Prefabs/Demo_TitlePanel")]
    public class TitlePanel : MonoRoutine,ICustomPanel
    {
        [SerializeField] public CustomButton StartButton;
        private ICustomPanel _customPanelImplementation;


        public void BindEvent()
        {
            disposables.Add(StartButton.Onclicked.AsObservable().Subscribe(_ =>
            {
                LogHelper.Log(LogHelper.DEMO,"StartButton.Onclicked");
            })
            );
        }

        public void BindEvent(IDataInstance data)
        {
            throw new System.NotImplementedException();
        }

        public void SetData(IDataInstance data)
        {
            throw new System.NotImplementedException();
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
            LogHelper.Log(LogHelper.FRAMEWORK,"ClosePanel");
            //Destroy(this.gameObject);
        }
    }
}