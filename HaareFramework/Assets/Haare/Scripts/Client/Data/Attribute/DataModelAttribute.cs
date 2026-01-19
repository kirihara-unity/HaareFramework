using System;

namespace Haare.Scripts.Client.Data
{
    // 모델 클래스가 어떤 데이터 소스(ScriptableObject)를 사용하는지 지정하는 Attribute
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DataModelAttribute : Attribute
    {
        public Type dataType { get; }
        public string AddressableJsonDataPath { get; }
        public string JsonDataPath { get; }

        public DataModelAttribute(Type templateType, string addressableJsonDataPath, string jsonDataPath)
        {
            dataType = templateType;
            AddressableJsonDataPath = addressableJsonDataPath;
            JsonDataPath = jsonDataPath;
        }
    }
}