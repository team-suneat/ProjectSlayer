using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat
{
    public class UIButton : UIInteractiveElement, IPointerDownHandler, IPointerUpHandler
    {
        [FoldoutGroup("#UIButton")]
        [SerializeField] private Button _button;

        [FoldoutGroup("#UIButton")]
        [SerializeField] private float _buttonImageAlphaDuration = 0.15f;

        [FoldoutGroup("#UIButton")]
        [SerializeField] private float _holdInterval = 0.1f;

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

            PlayClickAnimation();
            OnButtonClick();
        }

        protected virtual void OnButtonClick()
        {
            // 자식 클래스에서 오버라이드하여 클릭 이벤트 구현
        }

        protected virtual void OnButtonHold()
        {
            // 자식 클래스에서 오버라이드하여 홀드 이벤트 구현
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
            while (_isHolding && _isClickable)
            {
                yield return new WaitForSeconds(_holdInterval);

                if (_isHolding && _isClickable)
                {
                    OnButtonHold();
                }
            }
        }
    }
}

