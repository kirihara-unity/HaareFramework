using System;

namespace Haare.Client.UI
{
    // 모델 클래스가 어떤 데이터 소스(ScriptableObject)를 사용하는지 지정하는 Attribute
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PanelAttribute : Attribute
    {
        public string AddressablePath { get; }

        public PanelAttribute(string addressablePath)
        {
            AddressablePath = addressablePath;
        }
    }
}