using Cysharp.Threading.Tasks;
using Haare.Client.Routine;
using Haare.Scripts.Client.UI.Animator;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

namespace Haare.Client.UI
{
    public class CustomImage : MonoRoutine 
    {
        public Image _image;
        
        [SerializeField]
        private Sprite CommonSprite;
        [SerializeField]
        private Sprite HoveredSprite;
        [SerializeField]
        private Sprite ClickedSprite;
        
        [SerializeField]
        public bool OPTION_ANIMATION = false;
        
        [SerializeField]
        public bool ANIMATION_SLIDE = false;
        [SerializeField] private float slideDuration = 0.5f;
        [SerializeField] private Ease slideEaseType = Ease.OutQuad;
        [SerializeField] private RectTransform onScreenPosition;  
        [SerializeField] private RectTransform offScreenPosition;  

        [SerializeField]
        public bool ANIMATION_POPUP = false;
        [SerializeField] private float popupDuration = 0.2f;
        [SerializeField] private Ease popupEaseType = Ease.Linear;

        
        private UIAnimator _animator;

        protected override void Constructor()
        {
            base.Constructor();
            _image = GetComponent<Image>();
            
            if (OPTION_ANIMATION)
            {
                _animator = new UIAnimator(
                    this.gameObject
                );
                _animator.panelRectTransform = this.gameObject.GetComponent<RectTransform>();
                
                if (ANIMATION_SLIDE)
                {
                    ClearSlidePosition();
                    SlideOpenPanel();
                }
                if (ANIMATION_POPUP)
                {
                    PopupOpenPanel();
                }

            }
        }

        public void ClearSlidePosition()
        {
            if(_animator!=null)
                _animator.clearSlidePostion(offScreenPosition);
        }   
        public void SlideOpenPanel()
        {
            if(_animator!=null)
                _animator.SlideOpenPanel(slideDuration,onScreenPosition,slideEaseType);
        }
        public void PopupOpenPanel()
        {
            if(_animator!=null)
                _animator.OpenPopup(popupDuration,popupEaseType);
        }
        public void PopupclosePanel()
        {
            if(_animator!=null)
                _animator.ClosePopup(popupDuration,popupEaseType);
        }
        public override async UniTask Initialize()
        {
            await base.Initialize();
            SetupImage();
        }

        private void SetupImage()
        {
            if(CommonSprite!=null)
                _image.sprite = CommonSprite;
        }

        public void ChangeRGB(int r, int g, int b)
        {
            _image.color = new Color(r, g, b);
        }
        
        public async UniTask Fade(float startAlpha,float targetAlpha, float duration)
        {
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float progress = time / duration;
                ChangeAlpha(Mathf.Lerp(startAlpha, targetAlpha, progress));
                await UniTask.Yield();
            }
            ChangeAlpha(targetAlpha);
        }
        public void ChangeAlpha(float alpha)
        {
            _image.color = new Color(255, 255, 255, alpha);
        }
        
        public void ChangeCommonImage()
        {
            _image.sprite = CommonSprite;
        }

        public void ChangeHoverImage()
        {
            _image.sprite = HoveredSprite;
        }

        public void ChangeClickedImage()
        {
            _image.sprite = ClickedSprite;
        }

        public void ChangeImage(Sprite value)
        {
            _image.sprite = value;
        }

    }
}