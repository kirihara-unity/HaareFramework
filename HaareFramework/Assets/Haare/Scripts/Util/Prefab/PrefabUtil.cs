using Cysharp.Threading.Tasks;
using Haare.Util.Logger;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Haare.Util.Prefab
{
    public static class PrefabUtil
    {
        public static async UniTask<T>  InstantiatePrefab<T>(Transform parent, PrefabParam param) where T : Component
        {
            AsyncOperationHandle<GameObject> handle;
            try
            {
                // 1. Addressables를 통해 비동기적으로 인스턴스 생성
                handle = Addressables.InstantiateAsync(param.PrefabPath, parent, false);
                GameObject instance = await handle.ToUniTask();

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
                LogHelper.Error(LogHelper.ASSETLOADER,$"에셋 인스턴스화 실패! 주소: {param.PrefabPath}\n오류: {e.Message}");
                return null;
            }
        }
    }
    
    public class PrefabParam
    {
        public readonly string PrefabPath;

        public PrefabParam(string prefabPath)
        {
            PrefabPath = prefabPath;
        }
    }
}