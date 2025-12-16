using System;
using TeamSuneat.Data;
using TMPro;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UIStageTitleNotice : XBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _titleText;

        [SerializeField]
        private UICanvasGroupFader _fader;

        public event Action OnCompleted;

        private void Awake()
        {
            AutoGetComponents();
            _fader ??= GetComponentInChildren<UICanvasGroupFader>(true);
        }

        public void Show(StageNames stageName)
        {
            Show(stageName.GetLocalizedString());
        }

        public void Show(string content)
        {
            if (_titleText != null)
            {
                _titleText.text = content;
            }

            gameObject.SetActive(true);

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

        private void OnFadeOutComplete()
        {
            _fader?.SetCompletedCallback(null);
            gameObject.SetActive(false);
            OnCompleted?.Invoke();
        }

        private void OnDisable()
        {
            _fader?.KillFade();
        }
    }
}

