using UnityEngine;

namespace TeamSuneat
{
    public class UIPage : XBehaviour
    {
        [SerializeField] private bool _startActive;

        public bool IsActive { get; private set; }

        private void Awake()
        {
            if (_startActive)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public virtual void Show()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            SetActive(true);
            OnShow();
        }

        public virtual void Hide()
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;
            SetActive(false);
            OnHide();
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }
    }
}

