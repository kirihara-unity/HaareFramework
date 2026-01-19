using System.Threading;
using Cysharp.Threading.Tasks;
using Demo.UI;
using Haare.Client.UI;


namespace Demo.LobbyScene
{
    public class DemoLobbyUIManager : SceneUIManager
    {
        
        public override async UniTask Initialize(CancellationToken cts)
        {
            await base.Initialize(cts);
        }
        
        private void BindIPanel(ICustomPanel panel)
        {
            panel.uiManager = this;
            panel.BindEvent();
        }
    }
}