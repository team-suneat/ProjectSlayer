using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat
{
    public abstract class UIInteractiveElement : XBehaviour
    {
        [Title("#UIInteractiveElement")]
        [SerializeField] protected Image _frameImage;
        [SerializeField] protected Image _buttonImage;
        [SerializeField] protected TextMeshProUGUI _nameText;

        [Title("#UIInteractiveElement-Settings")]
        [SerializeField] protected float _clickCooldown = 0.3f;
        [SerializeField] protected float _punchScaleDuration = 0.2f;
        [SerializeField] protected Vector3 _punchScale = new Vector3(-0.1f, -0.1f, -0.1f);

        protected float _lastClickTime;
        protected Tween _scaleTween;
        protected bool _isClickable = true;

        public bool IsClickable => _isClickable;

        protected Color _frameImageOriginalColor;
        protected Color _nameTextOriginalColor;


        Vector3 _originalScale;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _frameImage ??= this.FindComponent<Image>("Frame Image");
            _buttonImage ??= this.FindComponent<Image>("Button Image");
            _nameText ??= this.FindComponent<TextMeshProUGUI>("Button Name Text");
        }

        void Awake()
        {
            _originalScale = transform.localScale;
        }

        protected override void OnStart()
        {
            base.OnStart();
            InitializeVisuals();
        }

        protected virtual void InitializeVisuals()
        {
            if (_frameImage != null)
            {
                _frameImageOriginalColor = _frameImage.color;
            }

            if (_nameText != null)
            {
                _nameTextOriginalColor = _nameText.color;
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            KillAllTweens();
        }

        protected bool CheckClickCooldown()
        {
            if (!_isClickable)
            {
                return false;
            }

            float currentTime = Time.time;
            if (currentTime - _lastClickTime < _clickCooldown)
            {
                return false;
            }

            _lastClickTime = currentTime;
            return true;
        }

        protected void PlayPunchScaleAnimation()
        {
            KillScaleTween();

            if (transform != null)
            {
                _scaleTween = transform.DOPunchScale(_punchScale, _punchScaleDuration, 1, 0.5f)
                    .SetEase(Ease.OutQuad).OnComplete(OnCompletedPunchScale);
            }
        }

        private void OnCompletedPunchScale()
        {
            transform.localScale = _originalScale;
        }

        protected virtual void KillAllTweens()
        {
            KillScaleTween();
        }

        protected void KillScaleTween()
        {
            _scaleTween?.Kill();
            _scaleTween = null;
        }

        public void SetName(string content)
        {
            AutoGetComponents();

            if (_nameText != null)
            {
                _nameText.text = content;
            }
        }

        public virtual void SetClickable(bool isClickable)
        {
            _isClickable = isClickable;
        }

        protected void SetFrameImageColor(Color color, float duration = 0f)
        {
            if (_frameImage == null)
            {
                return;
            }

            if (duration > 0f)
            {
                _frameImage.DOColor(color, duration).SetEase(Ease.OutQuad);
            }
            else
            {
                _frameImage.color = color;
            }
        }

        protected void SetNameTextColor(Color color, float duration = 0f)
        {
            if (_nameText == null)
            {
                return;
            }

            if (duration > 0f)
            {
                _nameText.DOColor(color, duration).SetEase(Ease.OutQuad);
            }
            else
            {
                _nameText.color = color;
            }
        }

        protected void SetButtonImageColor(Color color, float duration = 0f)
        {
            if (_buttonImage == null)
            {
                return;
            }

            if (duration > 0f)
            {
                _buttonImage.DOColor(color, duration).SetEase(Ease.OutQuad);
            }
            else
            {
                _buttonImage.color = color;
            }
        }
    }
}