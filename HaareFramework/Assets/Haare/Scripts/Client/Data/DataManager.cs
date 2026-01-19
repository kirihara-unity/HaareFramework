using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Haare.Client.Routine;
using Haare.Util.Loader;
using Haare.Util.Logger;
using UnityEngine;
using VContainer;

namespace Haare.Scripts.Client.Data
{
    public class DataManager : NativeRoutine
    {
        private Dictionary<Type, IDataModel> modelRegistry = new Dictionary<Type, IDataModel>();
        private Dictionary<long, ScriptableObject> soRegistry =
            new Dictionary<long, ScriptableObject>();

        public void Register<T>(T data) where T : IDataModel
        {
            LogHelper.Log(LogHelper.DATAMANAGER, $"Registering data for '{typeof(T).Name}'.");
            modelRegistry[typeof(T)] = data;
        }

        /// <summary>
        /// Local 데이터를 가져옵니다.
        /// </summary>
        public async UniTask<T> GetModel<T>() where T : class, IDataModel
        {
            try
            {
                var modelType = typeof(T);

                if (modelRegistry.TryGetValue(modelType, out var cachedModel))
                {
                    return (T)cachedModel;
                }

                LogHelper.Log(LogHelper.DATAMANAGER,
                    $"Cache miss for '{modelType.Name}'. Attempting to load...");
                
                var sourceAttribute = modelType.GetCustomAttribute<DataModelAttribute>();
                if (sourceAttribute == null)
                {
                    LogHelper.Error(LogHelper.DATAMANAGER,
                        $"Model type '{modelType.Name}' has no [DataModelSource] attribute.");
                    return default;
                }
                Type targetDataType = sourceAttribute.dataType;
                string address = sourceAttribute.JsonDataPath;
                
               
                TextAsset loadedAsset;
                if (AssetLoader.Exists(address))
                {
                    // Load With File. JsonLoad
                    var loadedLocalJsonAsset = await AssetLoader.LoadJsonAsync(address);
                    loadedAsset = loadedLocalJsonAsset;
                }
                else
                {
                    address = sourceAttribute.AddressableJsonDataPath;
                    // Load With Addressable JSON Template Data (Template)
                    var loadedTemplateJsonAsset =
                        await AssetLoader.LoadAsset<TextAsset>(address);
                    loadedAsset = loadedTemplateJsonAsset;
                }
                
                // Load With Addressable JSON Data (Template) -> Finish
                
                if (loadedAsset == null)
                {
                    LogHelper.Error(LogHelper.DATAMANAGER,
                        $"Failed to load template asset from address: {address}");
                    return default;
                }
                
                var deserializedObject =
                    JsonUtil.JsonUtilityGeneric.FromJson(loadedAsset.text,targetDataType);
                
                T newModel;
                try
                {
                    //Debug.Log($"Loaded Asset Full Type: {loadedAsset.GetType().FullName}");
                    var constructor = modelType.GetConstructor(new Type[] { deserializedObject.GetType() });
                    if (constructor != null)
                    {
                        newModel = constructor.Invoke(new object[] { deserializedObject }) as T;
                    }
                    else
                    {
                        LogHelper.Error(LogHelper.FRAMEWORK,
                            $"Constructor not found for '{modelType.Name}' that accepts a parameter of type '{loadedAsset.GetType().Name}'.");
                        newModel = default;
                    }
                    
                }
                catch (Exception e)
                {
                    LogHelper.Error(LogHelper.DATAMANAGER,
                        $"Failed to create instance of '{modelType.Name}'. Check for a public constructor that accepts '{loadedAsset.GetType().Name}'.\nError: {e.Message}");
                    return default;
                }


                if (newModel == null)
                {
                    LogHelper.Error(LogHelper.DATAMANAGER,
                        $"Model instance is null after creation. Ensure the constructor is correct for '{modelType.Name}'.");
                    return default;
                }

                Register(newModel);
                
                if (!AssetLoader.Exists(address))
                {
                    address = sourceAttribute.JsonDataPath;
                    await AssetLoader.SaveJson(address,deserializedObject);
                }
                return newModel;
            }
            catch (Exception e)
            {
                LogHelper.Error(LogHelper.DATAMANAGER,
                    $"Failed to Get<>.\nError: {e.Message}");
                throw;
            }
        }
        
        // 통합 저장 함수
        public async UniTask SaveData<TW,T>(IDataModel model,IData data)  where TW : IDataModel where T : IData
        {
            var modelType = typeof(TW);
            var sourceAttribute = modelType.GetCustomAttribute<DataModelAttribute>();
            string address = sourceAttribute.JsonDataPath; 
            await AssetLoader.SaveJson(address,data);
            LogHelper.Log(LogHelper.DATAMANAGER, "데이터 저장 완료.");
        }
    }
}