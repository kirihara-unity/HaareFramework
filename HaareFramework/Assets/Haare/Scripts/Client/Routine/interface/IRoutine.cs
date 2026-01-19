using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Haare.Client.Routine
{
    public interface IRoutine
    {
        CancellationTokenSource _cts { get; }

        //Processor Register?
        bool isRegistered	{ get; }
        //SceneOnly
        bool isInSceneOnly { get; }
        //Init?
        bool isInitialized	{ get; }
        
        Func<CancellationToken,UniTask> Oninitialize  { get; }
        Func<UniTask> Onfinalize	{ get; }
        
        UniTask Initialize(CancellationToken cts);
        UniTask Finalize();
    }
}