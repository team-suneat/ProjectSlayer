using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat
{
    public class UIButton : UIInteractiveElement
    {
        [Title("#UIButton")]
        [SerializeField] private Button _button;

        [Title("#UIButton-Settings")]
        [SerializeField] private float _buttonImageAlphaDuration = 0.15f;

        private Tween _alphaTween;

        public Button Button => _button;



        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _button ??= GetComponent<Button>();
        }

        protected override void OnStart()
        {
            base.OnStart();
            InitializeButtonImage();
        }

        private void InitializeButtonImage()
        {
            if (_buttonImage != null)
            {
                Color color = _buttonImage.color;
                color.a = 0f;
                _buttonImage.color = color;
            }
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();
            if (_button != null)
            {
                _button.onClick.AddListener(OnButtonClicked);
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnButtonClicked);
            }

            KillAllTweens();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            KillAllTweens();
        }

        private void OnButtonClicked()
        {
            if (!CheckClickCooldown())
            {
                return;
            }

            PlayClickAnimation();
            OnButtonClick();
        }

        protected virtual void OnButtonClick()
        {
            // 자식 클래스에서 오버라이드하여 클릭 이벤트 구현
        }

        private void PlayClickAnimation()
        {
            PlayPunchScaleAnimation();
            PlayButtonImageAlphaAnimation();
        }

        private void PlayButtonImageAlphaAnimation()
        {
            KillAlphaTween();

            if (_buttonImage != null)
            {
                _alphaTween = _buttonImage.DOFade(0.5f, _buttonImageAlphaDuration * 0.5f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        if (_buttonImage != null)
                        {
                            _buttonImage.DOFade(0f, _buttonImageAlphaDuration * 0.5f)
                                .SetEase(Ease.InQuad);
                        }
                    });
            }
        }

        protected override void KillAllTweens()
        {
            base.KillAllTweens();
            KillAlphaTween();
        }

        private void KillAlphaTween()
        {
            _alphaTween?.Kill();
            _alphaTween = null;
        }

        public override void SetClickable(bool isClickable)
        {
            base.SetClickable(isClickable);

            if (_button != null)
            {
                _button.interactable = isClickable;
            }
        }

        public void AddListener(UnityAction action)
        {
            AutoGetComponents();

            if (_button != null)
            {
                _button.onClick.AddListener(action);
            }
        }

        public void RemoveListener(UnityAction action)
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(action);
            }
        }

        public void RemoveAllListeners()
        {
            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(OnButtonClicked);
            }
        }
    }
}

