using System;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // Notice UI 기본 클래스 - 공통 기능 제공
    public abstract class UINoticeBase : XBehaviour
    {
        [SerializeField]
        protected UILocalizedText _titleText;

        [SerializeField]
        protected UICanvasGroupFader _fader;

        public event Action OnCompleted;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _titleText ??= GetComponentInChildren<UILocalizedText>(true);
            _fader ??= GetComponentInChildren<UICanvasGroupFader>(true);
        }

        public virtual void Show(string content)
        {
            if (_titleText != null)
            {
                _titleText.SetText(content);
            }

            StartFadeInOut();
        }

        private void StartFadeInOut()
        {
            if (_fader != null)
            {
                _fader.KillFade();
                _fader.SetCompletedCallback(OnFadeOutComplete);
                _fader.FadeInOut();
            }
            else
            {
                OnFadeOutComplete();
            }
        }

        protected virtual void OnFadeOutComplete()
        {
            _fader?.SetCompletedCallback(null);
            gameObject.SetActive(false);
            OnCompleted?.Invoke();
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            _fader?.KillFade();
        }
    }
}