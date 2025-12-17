using TMPro;

using Haare.Client.Routine;
using UnityEngine;

namespace Haare.Client.UI
{
    public class CustomText : MonoRoutine
    {
        public TMP_Text _Text;
        
        protected override void Constructor()
        {
            base.Constructor();
            _Text = GetComponent<TMP_Text>();
        }
        public void SetupText(string value)
        {
            _Text.text = value;
        }
        public void SetupTextColor(Color32 color)
        {
            _Text.color = color;
        }
    }
}