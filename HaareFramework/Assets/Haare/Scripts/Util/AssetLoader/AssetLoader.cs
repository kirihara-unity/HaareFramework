using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Haare.Client.Routine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Haare.Util.Logger;
using R3;
using Object = UnityEngine.Object;

namespace Haare.Util.Loader
{
    public static class AssetLoader
    {
        
        private static readonly ReactiveProperty<float> _dlProgress = new ReactiveProperty<float>(0f);
        public static ReadOnlyReactiveProperty<float> DownloadProgress => _dlProgress;
        public static readonly Subject<bool> AssetDownloadTaskFinished  = new Subject<bool>();
        
        
        public static async UniTask<T>  InstantiatePrefab<T>(Transform parent, string param,
            CancellationToken cts = default) where T : Component
        {
            AsyncOperationHandle<GameObject> handle;
            handle = Addressables.InstantiateAsync(param, parent, false);
            try
            {
                // 1. Addressables를 통해 비동기적으로 인스턴스 생성
                GameObject instance = await handle.WithCancellation(cts);

                // 2. 인스턴스 생성 후 컴포넌트 가져오기
                if (instance != null && instance.TryGetComponent<T>(out var component))
                {
                    return component;
                }

                // 3. 컴포넌트를 찾지 못했을 경우
                if (instance != null)
                {
                    LogHelper.Warning(LogHelper.ASSETLOADER,$"생성된 프리팹 '{instance.name}'에서 '{typeof(T).Name}' 컴포넌트를 찾을 수 없습니다.");
                }
                
                return null;
            }
            catch (System.Exception e)
            {
                // 4. 주소가 잘못되었거나 로딩 중 예외 발생 시 오류 처리
                LogHelper.Error(LogHelper.ASSETLOADER,$"에셋 인스턴스화 실패! 주소: {param}\n오류: {e.Message}");
                return null;
            }
        }
        // Data 관련
        public static async UniTask<T> LoadAsset<T>(string assetAddress) where T : Object
        {
            AsyncOperationHandle<T> handle;
            handle = Addressables.LoadAssetAsync<T>(assetAddress);
            try
            {
                T asset = await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    // 중요: 이 에셋을 더 이상 사용하지 않을 때 Addressables.Release(handle) 또는 Addressables.Release(asset)을 호출해 메모리를 해제해야 합니다.
                    return asset;
                }

                LogHelper.Warning(LogHelper.ASSETLOADER, $"에셋 '{assetAddress}'을(를) 로드할 수 없습니다.");
                if (handle.IsValid()) Addressables.Release(handle);
                return null;
            }
            catch (System.Exception e)
            {
                LogHelper.Error(LogHelper.ASSETLOADER,
                    $"에셋 로드 실패! 주소: {assetAddress}\n오류: {e.Message}");
                if (handle.IsValid()) Addressables.Release(handle);
                return null;
            }
        }
        
        
        private static string BasePath => Application.persistentDataPath;
        /// <summary>
        /// 데이터를 로컬 파일(JSON)로 저장합니다.
        /// </summary>
        public static async UniTask SaveJson(string filePath, object data)
        {
            try
            {
                // 1. 경로 조합
                string path = Path.Combine(BasePath, filePath);
                
                // 2. 객체를 JSON 문자열로 변환 (들여쓰기 포함)
                string json = JsonUtility.ToJson(data, true);

                // 3. 비동기 파일 쓰기 (대용량 데이터 대비)
                await File.WriteAllTextAsync(path, json);
                
                LogHelper.Log(LogHelper.DATAMANAGER, $"Saved to local: {path}");
            }
            catch (Exception e)
            {
                LogHelper.Error(LogHelper.DATAMANAGER, $"File Save Error: {e.Message}");
            }
        }
        /// <summary>
        /// 로컬 파일이 존재하는지 확인합니다.
        /// </summary>
        public static bool Exists(string fileName)
        {
            string path = Path.Combine(BasePath, fileName);
            return File.Exists(path);
        }
        public static async UniTask<TextAsset> LoadJsonAsync(string fileName)
        {
            string path = Path.Combine(BasePath, fileName);
            
            if (!File.Exists(path))
            {
                return null; 
            }

            try
            {
                // 1. 로컬 파일에서 텍스트 읽기
                string json = await File.ReadAllTextAsync(path);

                // 2. 런타임에 TextAsset 생성해서 리턴
                var textAsset = new TextAsset(json);
                textAsset.name = fileName; // 디버깅용 이름 설정
                return textAsset;
            }
            catch (Exception e)
            {
                LogHelper.Error(LogHelper.DATAMANAGER, $"Local File Load Error: {e.Message}");
                return null;
            }
        }
        
        // SO에서 로드하는건 이제 사용안해!
        public static async UniTask<T> GetItemSO<T>(long itemID) where T : ScriptableObject
        {
            //var path  = AssetPath.ItemSOdict[itemID];
            var path  = "";
            var loadTask = await Addressables.LoadAssetAsync<T>(path);
            var loadedAsset = loadTask;
            return loadedAsset;
        }
    }
}