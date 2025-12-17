using System.Threading;
using Cysharp.Threading.Tasks;
using Haare.Client.UI;
using Haare.Scripts.Client.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Haare.Util.Loader
{
    public static class AssetLoader
    {
        public static async UniTask<T>  InstantiatePrefab<T>(Transform parent, string param,
            CancellationToken cts = default) where T : Component
        {
            AsyncOperationHandle<GameObject> handle;
            try
            {
                // 1. Addressables를 통해 비동기적으로 인스턴스 생성
                handle = Addressables.InstantiateAsync(param, parent, false);
                GameObject instance = await handle.WithCancellation(cts);

                // 2. 인스턴스 생성 후 컴포넌트 가져오기
                if (instance != null && instance.TryGetComponent<T>(out var component))
                {
                    return component;
                }

                // 3. 컴포넌트를 찾지 못했을 경우
                if (instance != null)
                {
                    LogHelper.LogHelper.Warning(LogHelper.LogHelper.ASSETLOADER,$"생성된 프리팹 '{instance.name}'에서 '{typeof(T).Name}' 컴포넌트를 찾을 수 없습니다.");
                }
                
                return null;
            }
            catch (System.Exception e)
            {
                // 4. 주소가 잘못되었거나 로딩 중 예외 발생 시 오류 처리
                LogHelper.LogHelper.Error(LogHelper.LogHelper.ASSETLOADER,$"에셋 인스턴스화 실패! 주소: {param}\n오류: {e.Message}");
                return null;
            }
        }
        
        // Data 관련
        public static async UniTask<T> LoadAsset<T>(string assetAddress) where T : Object
        {
            AsyncOperationHandle<T> handle;
            try
            {
                handle = Addressables.LoadAssetAsync<T>(assetAddress);
                T asset = await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    // 중요: 이 에셋을 더 이상 사용하지 않을 때 Addressables.Release(handle) 또는 Addressables.Release(asset)을 호출해 메모리를 해제해야 합니다.
                    return asset;
                }
            
                LogHelper.LogHelper.Warning(LogHelper.LogHelper.ASSETLOADER, $"에셋 '{assetAddress}'을(를) 로드할 수 없습니다.");
                return null;
            }
            catch (System.Exception e)
            { 
                LogHelper.LogHelper.Error(LogHelper.LogHelper.ASSETLOADER, 
                    $"에셋 로드 실패! 주소: {assetAddress}\n오류: {e.Message}");
                return null;
            }
        }
        
        public static async UniTask<T> GetItemSO<T>(long itemID) where T : ScriptableObject
        {
            var path  = AssetPath.ItemSOdict[itemID];
            var loadTask = await Addressables.LoadAssetAsync<T>(path);
            var loadedAsset = loadTask;
            return loadedAsset;
        }
    }
}