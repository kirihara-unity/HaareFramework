using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.EventSystems;

using Haare.Client.Core;
using Haare.Client.Core.Singleton;
using Haare.Client.UI;
using Haare.Util.LogHelper;

public class HaareClient
{
    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
    static async void Main() {
        
        LogHelper.Log(LogHelper.FRAMEWORK,"Start Haare Framework");
        await Task.Delay( 1 );
        await Processer.WaitForCreation();
        
        await Processer.Instance.Constructor(InitializePlugin, RegisterProcesses);
        
    }

    static async UniTask InitializePlugin() {
        // SDK 로드
        await UniTask.Delay( 0 );
    }
    

    static async UniTask RegisterProcesses()
    {
        LogHelper.LogTask(LogHelper.FRAMEWORK,"RegisterProcesses");
        
        // 씬 전역에서 사용하는 object 
        var eventSystemObj = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        Object.DontDestroyOnLoad(eventSystemObj);
        
        var audioObj = new GameObject("AudioListener", typeof(AudioListener));
        Object.DontDestroyOnLoad(audioObj);

        
        LogHelper.LogTask(LogHelper.FRAMEWORK,"RegisterProcesses -> end");
        await UniTask.CompletedTask;
    }
    
}
