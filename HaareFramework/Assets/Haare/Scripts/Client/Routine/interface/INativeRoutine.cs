using System;
using Cysharp.Threading.Tasks;
using R3;

namespace Haare.Client.Routine
{
    public interface INativeRoutine : IRoutine
    {		
        Subject<Unit> Onupdate		{ get; }
        void UpdateProcess();
        void OnApplicationQuit();
        void OnApplicationPause(bool pauseStatus);
    }
}