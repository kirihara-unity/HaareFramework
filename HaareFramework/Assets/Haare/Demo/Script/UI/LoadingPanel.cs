using Haare.Client.Routine;
using Haare.Client.UI;
using Haare.Scripts.Client.Data;
using UnityEngine;

namespace Demo.UI
{
    [PanelAttribute("Prefabs/Demo_LoadingPanel")]
    public class LoadingPanel : MonoRoutine,ICustomPanel
    {
        [SerializeField] public CustomSlider LoadingSlider;
        public SceneUIManager uiManager { get; set; }
        public GameObject panel { get; set; }

        public void BindEvent()
        {
            
        }
        public void SetData(IDataInstance data)
        {
            
        }

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