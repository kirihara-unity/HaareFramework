using System.Threading;
using Cysharp.Threading.Tasks;


using Haare.Client.Routine;

namespace  Demo.LoadScene
{
 
    public class DemoLoadMono : MonoRoutine
    {
        public override async UniTask Initialize(CancellationToken cts)
        {
            await base.Initialize(cts);
        }
        
    }
       
}