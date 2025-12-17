using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Haare.Client.Routine;
using Haare.Client.UI;
using Haare.Util.LogHelper;
using TMPro;
using UnityEngine;

public class FPSLogger : MonoRoutine
{
    public override async UniTask Initialize(CancellationToken cts)
    {
        LogHelper.Log(LogHelper.TASK,"FPSLogger Init");
        base.isInSceneOnly = false;
        await base.Initialize(cts);
    }
    
    // FPS를 표시할 TextMeshPro UI
    public CustomText fpsText;

    // FPS 계산을 위한 변수들
    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    
    protected override void UpdateProcess()
    {
        base.UpdateProcess();
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
        fpsText.SetupText(fps.ToString());
    }
}
