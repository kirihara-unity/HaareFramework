using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Haare.Client.Routine;
using Haare.Util.Logger;
using Unity.VisualScripting;

namespace Haare.Client.Core.Singleton
{
    public abstract class SingletonMonoBehaviour<T> : MonoRoutine where T : MonoRoutine
    {		
        
        /// <summary>
        /// is already Created?
        /// </summary>
        public static bool isCreated => instance != null;

        private static bool QuittingProgram = false;
        
        private static T instance;
        public static T Instance
        {
            get
            {
                if (!isCreated)
                {
                    if (QuittingProgram)
                    {
                        LogHelper.Warning(LogHelper.FRAMEWORK,"Instace Gen Canceled while Quit Process It will be Ignore");
                        return null;
                        
                    }
                    Type t = typeof(T);
                    Create();
                    instance = (T)FindObjectOfType(t);
                }
              
                return instance;
            }
        }
        
        
        public override async UniTask Initialize(CancellationToken cts)
        {
            await base.Initialize(cts);
            Type t = typeof(T);
            instance = (T)FindObjectOfType(t);
        }
        
        static protected void Create() {
            if ( isCreated )	{ return; }
            GameObject Obj = new GameObject($"{typeof(T).Name}");
            LogHelper.Log(LogHelper.FRAMEWORK,$"Singleton Generated {typeof(T).Name}");
            Obj.AddComponent<T>();
            DontDestroyOnLoad(Obj);
        }
        
        static public async UniTask WaitForCreation() {
            var i = Instance;
            await UniTask.Delay( 1 );
        }

        void OnDestroy()
        {
            LogHelper.Log(LogHelper.FRAMEWORK,"instance destroyed"); 
        }
        private void OnApplicationQuit()
        {
            QuittingProgram = true;
        }
    }
}
