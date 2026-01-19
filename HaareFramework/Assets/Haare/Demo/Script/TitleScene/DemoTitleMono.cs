
using System.Threading;
using Cysharp.Threading.Tasks;
using Haare.Client.Routine;
using Haare.Util.Logger;
using UnityEngine;

namespace  Demo.TitleScene
{
 
    public class DemoTitleMono : MonoRoutine
    {
     
        public override async UniTask Initialize(CancellationToken cts)
        {
            await base.Initialize(cts);
            LogHelper.LogTask(LogHelper.DEMO,"DemoBootInit");
        }
        
    }
       
}