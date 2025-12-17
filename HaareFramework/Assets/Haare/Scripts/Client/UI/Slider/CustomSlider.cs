using System.Threading;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

using Haare.Client.Routine;

namespace Haare.Client.UI
{
    
    [RequireComponent(typeof(Slider))]
    public class CustomSlider : MonoRoutine
    {
        [SerializeField]
        private Slider _slider;

        [SerializeField] private CustomImage background;
        [SerializeField] private CustomImage fill;
        public float Value => _slider.value;
        
        public override async UniTask Initialize(CancellationToken cts)
        {
            await base.Initialize(cts);
            _slider = GetComponent<Slider>();
            Setup(0,1,0);
        }

        /// <summary>
        /// 슬라이더를 초기화합니다.
        /// </summary>
        public void Setup(float minValue, float maxValue, float currentValue)
        {
            _slider.minValue = minValue;
            _slider.maxValue = maxValue;
            _slider.value = Mathf.Clamp(currentValue, minValue, maxValue);
        }
        public void SetValue(float value)
        {
            _slider.value = Mathf.Clamp(value, _slider.minValue, _slider.maxValue);
        }
    }
}