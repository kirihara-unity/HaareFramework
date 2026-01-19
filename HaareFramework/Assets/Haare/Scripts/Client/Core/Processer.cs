using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

using R3.Triggers;
using UnityEngine;

using Haare.Client.Core.Singleton;
using Haare.Client.Routine;
using Haare.Util.Logger;

namespace Haare.Client.Core
{
	/// <summary>
	/// 모든 Routine들의 최종 관리자. 생애주기에 대한 관리가 이곳에서 진행됩니다.
	/// </summary>
    public class Processer : SingletonMonoBehaviour<Processer>
    {		
	    public ReadOnlyReactiveProperty<bool> PROCESSING => processing;
	    private ReactiveProperty<bool> processing = new ReactiveProperty<bool>();
	    [Header("PROCESS_FLAG")]
	    [SerializeField] private bool processvalue = true;
	    void OnValidate()
	    {
		    if (processing != null)
		    {
			    // Inspector에서 값이 변경된 경우, ReactiveProperty의 값을 업데이트합니다.
			    processing.Value = processvalue;
		    }
	    }
	    public override bool isRegistered => false;
	    
	    //현재 관리중인 Routine
        List<IRoutine> Routines = new List<IRoutine>();
        //삭제 대기중인 Routine
        List<IRoutine> deleteRoutines = new List<IRoutine>();
        bool DeleteProcessing;
        
        /// <summary>
        /// Constructor 게임의 초동 처리가 이곳에서 
        /// </summary>
        public async UniTask Constructor( 
            Func<UniTask> initializePlugin, 
            Func<UniTask> registerProcesses 
            ) 
        {
	        processing.Value = processvalue;
	        QualitySettings.vSyncCount = 0;

			#if UNITY_IOS || UNITY_ANDROID
			        Application.targetFrameRate = 120;
			#elif UNITY_STANDALONE
			        Application.targetFrameRate = 120;
			#else
			        Application.targetFrameRate = 60;
			#endif
	        
            await initializePlugin();				
            await RegisterEvents( registerProcesses );	
            base.Constructor();
            await Initialize(_cts.Token);
			LogHelper.LogTask(LogHelper.FRAMEWORK,"Finished Constructor");   
        } 
        
        /// <summary>
        /// Processer의 event trigger에 맞춰서 구독중인 모든 Routine들이 함께 갱신됩니다
        /// 단 MonoRoutine은 별도의 타이밍으로 작동됩니다
        /// </summary>
        /// <param name="registerProcesses"></param>
        private async UniTask RegisterEvents( Func<UniTask> registerProcesses ) {

			Oninitialize += async (cts) => {
				await registerProcesses();
				// foreach ( var p in Routines ) {
				// 	await p.Initialize(cts);
				// }
			};

			Onupdate.Subscribe(
				_ => {
					foreach (var p in Routines)
					{
						if(p is INativeRoutine)
							((INativeRoutine)p).UpdateProcess();
					}
					CheckDeleteProcesses().Forget();
				}
			);
			
			Onfinalize += async () => {
				foreach ( var p in Routines ) {
					UnRegister( p );
				}
				await CheckDeleteProcesses();
			};
			
			Observable.EveryUpdate().Subscribe( _ => { 
				if(PROCESSING.CurrentValue)
					UpdateProcess();
			} );
			this.LateUpdateAsObservable().Subscribe(_ =>
			{
				if(PROCESSING.CurrentValue)
					LateUpdateProcess();
			});
			this.FixedUpdateAsObservable().Subscribe( _ => { 
				if(PROCESSING.CurrentValue)
					FixedUpdateProcess();
			} );
			// SceneRoutine.Instance.CurrentPhase.AsObservable().Subscribe(_ =>
			// {
			// 	if (_ != SceneLoadPhase.Loading)
			// 		return;
			// 	Processer.Instance.CheckDeleteProcessesForScene().Forget();
			// });
			await UniTask.Delay( 0 );
			
		}
        
        /// <summary>
        /// 현재의 Scene이 파기되고 Scene 종속적으로 생겨난 Routine들을 파기합니다
        /// </summary>
        public async UniTask CheckDeleteProcessesForScene() {
	        LogHelper.LogTask(LogHelper.FRAMEWORK,"Checked DeleteProcessesForScene");   
	        
	        foreach ( var p in Routines ) {
		        if ( p.isInSceneOnly ) {
			        UnRegister( p );
		        }
	        }
	        await UniTask.WaitUntil( () => deleteRoutines.Count == 0 );
	        await UniTask.WaitUntil( () => !DeleteProcessing );
        }
        
        /// <summary>
        /// Processer에 Routine을 등록합니다.
        /// </summary>
        /// <param name="process"></param>
        public async UniTask Register( IRoutine process ,CancellationToken cts) {
            if ( !Routines.Contains( process ) ) {
	            //LogHelper.LogTask(LogHelper.FRAMEWORK,$"Registered : {process}");   

	            Routines.Add( process );	
                
                if ( isInitialized ) {
                    await process.Initialize(cts);
                }
            }
        }
        /// <summary>
        /// Processer에 Routine을 등록 해제합니다.
        /// </summary>
        /// <param name="process"></param>
        public void UnRegister( IRoutine process ) {
	        deleteRoutines.Add( process );
        }
        
        /// <summary>
        /// Routine의 파기 처리가 완료될때까지 작동합니다.
        /// </summary>
        async UniTask CheckDeleteProcesses() {
	        
	        if ( deleteRoutines.Count == 0 )	{ return; }
	        if (DeleteProcessing){ return; }
	        DeleteProcessing = true;	
	        
	        var dps = deleteRoutines.Distinct().ToList();
	        deleteRoutines.Clear();
	        
	        var tasks = new List<UniTask>();
	        
	        foreach ( var p in dps ) {
		        if ( Routines.Remove( p ) ) {
			        tasks.Add(
				        new Func<UniTask>(
					        async () => {
						        await p.Finalize();
					        }
				        )()
			        );
		        }
	        }
	        await UniTask.WhenAll( tasks );
	        DeleteProcessing = false;
        }
    }
	
}