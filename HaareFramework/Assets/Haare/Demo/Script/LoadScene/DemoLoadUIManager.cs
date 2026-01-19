using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Demo.UI;
using Haare.Client.Routine.Service.SceneService;
using Haare.Client.UI;
using UnityEngine;

namespace Demo.LoadScene
{
    public class DemoLoadUIManager : SceneUIManager
    {
        private int loadingPanelID;
        
        public override async UniTask Initialize(CancellationToken cts)
        {
            await base.Initialize(cts);
            
            loadingPanelID = await LoadPanel<LoadingPanel>(null,false, true);
           
        }
        
    }
}