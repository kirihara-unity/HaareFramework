using System.Threading;
using Cysharp.Threading.Tasks;
using Haare.Client.Routine;
using UnityEngine;

namespace Demo.LobbyScene
{
    public class DemoCubeRotator : MonoRoutine
    {
        public float rotationSpeed = 50f;

        public override async UniTask Initialize(CancellationToken cts)
        {
            await base.Initialize(cts);
            await UniTask.CompletedTask;
        }
        // 매 프레임마다 호출되는 Update 함수입니다.
        protected override void UpdateProcess()
        {
            this.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Destroy(gameObject);
            }
        }
        
        
        
    }
}