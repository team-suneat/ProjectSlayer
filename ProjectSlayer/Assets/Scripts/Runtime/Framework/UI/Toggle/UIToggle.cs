using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat
{
    public class UIToggle : XBehaviour
    {
        [Title("#UIToggle")]
        [SerializeField] private Toggle _toggle;
        [SerializeField] private UIToggleIcon _icon;
        [SerializeField] private UIToggleName _name;
        [SerializeField] private UIToggleLock _lock;
        [SerializeField] private UIToggleUpdateIndicator _updateIndicator;

        public Toggle Toggle => _toggle;
        public bool IsOn => _toggle != null && _toggle.isOn;
        public bool IsLocked => _lock != null && _lock.IsLocked;

        private void Awake()
        {
            AutoGetComponents();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _toggle ??= GetComponent<Toggle>();
            _icon ??= GetComponent<UIToggleIcon>();
            _name ??= GetComponent<UIToggleName>();
            _lock ??= GetComponent<UIToggleLock>();
            _updateIndicator ??= GetComponentInChildren<UIToggleUpdateIndicator>(true);
        }

        public void SetIsOn(bool isOn)
        {
            if (_toggle != null)
            {
                _toggle.isOn = isOn;
            }
        }

        public void SetGroup(ToggleGroup group)
        {
            if (_toggle != null)
            {
                _toggle.group = group;
            }
        }

        public void AddListener(UnityAction<bool> action)
        {
            if (_toggle != null)
            {
                _toggle.onValueChanged.AddListener(action);
            }
        }

        public void SetIcon(Sprite sprite)
        {
            _icon?.SetIcon(sprite);
        }

        public void SetName(string content)
        {
            _name?.SetName(content);
        }

        public void SetHasUpdate(bool hasUpdate)
        {
            _updateIndicator?.SetHasUpdate(hasUpdate);
        }

        public void UpdateLockVisual()
        {
            _lock?.UpdateVisualState();
        }

        public UIToggleLock GetLock() => _lock;
        public UIToggleIcon GetIcon() => _icon;
        public UIToggleName GetName() => _name;
        public UIToggleUpdateIndicator GetUpdateIndicator() => _updateIndicator;
    }
}

