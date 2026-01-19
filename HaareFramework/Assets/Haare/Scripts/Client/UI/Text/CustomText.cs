using TMPro;

using Haare.Client.Routine;
using UnityEngine;

namespace Haare.Client.UI
{
    public class CustomText : MonoRoutine
    {
        [SerializeField]
        private TMP_Text _text;
        public TMP_Text Text 
        {
            // 2. 꺼낼 때: 없으면 찾아오고(??=), 있으면 그냥 줌
            get => _text ??= GetComponentInChildren<TMP_Text>(true);
    
            // 3. 넣을 때: 외부에서 강제로 다른 걸 끼워넣을 수 있게 함
            set => _text = value;
        }
        protected override void Constructor()
        {
            base.Constructor();
            Text = GetComponent<TMP_Text>();
        }
        public void SetupText(string value)
        {
            Text.text = value;
        }
        public void SetupTextColor(Color32 color)
        {
            Text.color = color;
        }
    }
}