using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIButton : UIInteractiveElement, IPointerDownHandler, IPointerUpHandler
    {
        private const float ALPHA_TRANSPARENT = 0f;
        private const float ALPHA_SEMI_TRANSPARENT = 0.5f;
        private const float ALPHA_ANIMATION_DURATION_RATIO = 0.5f;
        private const float DEFAULT_HOLD_INTERVAL = 0.1f;
        private const float DEFAULT_HOLD_DELAY = 0.3f;
        private const float DEFAULT_BUTTON_IMAGE_ALPHA_DURATION = 0.15f;

        [FoldoutGroup("#UIButton")]
        [SerializeField] private Button _button;

        private Tween _alphaTween;
        private Coroutine _holdCoroutine;
        private bool _isHolding;

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
                color.a = ALPHA_TRANSPARENT;
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

            StopHoldCoroutine();
            KillAllTweens();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            StopHoldCoroutine();
            KillAllTweens();
        }

        private void OnButtonClicked()
        {
            if (!CheckClickCooldown())
            {
                return;
            }

            OnButtonClick();
        }

        protected virtual void OnButtonClick()
        {
            PlayPunchScaleAnimation();
            PlayButtonImageAlphaAnimation();
        }

        protected virtual void OnButtonHold()
        {
            PlayPunchScaleAnimation();
            PlayButtonImageAlphaAnimation();
        }

        private void PlayButtonImageAlphaAnimation()
        {
            KillAlphaTween();

            if (_buttonImage != null)
            {
                float halfDuration = DEFAULT_BUTTON_IMAGE_ALPHA_DURATION * ALPHA_ANIMATION_DURATION_RATIO;
                _alphaTween = _buttonImage.DOFade(ALPHA_SEMI_TRANSPARENT, halfDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        if (_buttonImage != null)
                        {
                            _buttonImage.DOFade(ALPHA_TRANSPARENT, halfDuration)
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

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isClickable)
            {
                return;
            }

            _isHolding = true;
            StartHoldCoroutine();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopHoldCoroutine();
            _isHolding = false;
        }

        private void StartHoldCoroutine()
        {
            StopHoldCoroutine();
            _holdCoroutine = StartCoroutine(HoldCoroutine());
        }

        private void StopHoldCoroutine()
        {
            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
                _holdCoroutine = null;
            }
        }

        private IEnumerator HoldCoroutine()
        {
            // 첫 터치 후 홀드 시작까지 대기
            yield return new WaitForSeconds(DEFAULT_HOLD_DELAY);

            while (_isHolding && _isClickable)
            {
                yield return new WaitForSeconds(DEFAULT_HOLD_INTERVAL);

                if (_isHolding && _isClickable)
                {
                    OnButtonHold();
                }
            }
        }
    }
}

