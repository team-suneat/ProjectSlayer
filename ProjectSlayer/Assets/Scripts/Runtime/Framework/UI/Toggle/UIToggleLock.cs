using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat
{
    public class UIToggleLock : XBehaviour
    {
        [Title("#UIToggleLock")]
        [SerializeField] private bool _isLocked;
        [SerializeField] private string _lockMessage;
        [SerializeField] private Graphic _toggleGraphic;
        [SerializeField] private Image _lockImage;
        [SerializeField] private UIToggleIcon _icon;
        [SerializeField] private UIToggleName _name;

        [Title("#Events")]
        public UnityEvent<string> OnLockMessageRequested;

        public bool IsLocked => _isLocked;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _icon ??= GetComponent<UIToggleIcon>();
            _name ??= GetComponent<UIToggleName>();

            if (_toggleGraphic == null)
            {
                var toggle = GetComponent<Toggle>();
                if (toggle != null)
                {
                    _toggleGraphic = toggle.targetGraphic;
                }
            }

            _lockImage ??= this.FindComponent<Image>("Toggle Lock Image");
        }

        public void SetLocked(bool locked)
        {
            _isLocked = locked;
            UpdateVisualState();
        }

        public void RequestLockMessage()
        {
            if (!_isLocked)
            {
                return;
            }

            OnLockMessageRequested?.Invoke(_lockMessage);
        }

        public void UpdateVisualState()
        {
            AutoGetComponents();

            SetGrayScale(_toggleGraphic, _isLocked);
            _icon?.SetGrayScale(_isLocked);
            _name?.SetGrayScale(_isLocked);

            if (_lockImage != null)
            {
                _lockImage.gameObject.SetActive(_isLocked);
            }
        }

        private void SetGrayScale(Graphic graphic, bool isGray)
        {
            if (graphic == null)
            {
                return;
            }

            graphic.color = isGray ? GameColors.Gray : GameColors.White;
        }
    }
}