using System.Collections.Generic;
using TeamSuneat;

namespace TeamSuneat.UserInterface
{
    // Notice UI 관리자 - 일시적으로 표시되는 알림 메시지들을 관리
    public class UINoticeManager : XBehaviour
    {
        private List<XBehaviour> _activeNotices = new List<XBehaviour>();

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent<CurrencyNames>.Register(GlobalEventType.CURRENCY_SHORTAGE, OnCurrencyShortage);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent<CurrencyNames>.Unregister(GlobalEventType.CURRENCY_SHORTAGE, OnCurrencyShortage);
        }

        private void OnCurrencyShortage(CurrencyNames currencyName)
        {
            SpawnCurrencyShortageNotice(currencyName);
        }

        public UICurrencyShortageNotice SpawnCurrencyShortageNotice(CurrencyNames currencyName)
        {
            UICurrencyShortageNotice notice = ResourcesManager.SpawnCurrencyShortageNotice(currencyName);
            if (notice != null)
            {
                RegisterNotice(notice);
                notice.OnCompleted += () => UnregisterNotice(notice);
            }
            return notice;
        }

        public UICurrencyShortageNotice SpawnCurrencyShortageNotice(string content)
        {
            UICurrencyShortageNotice notice = ResourcesManager.SpawnCurrencyShortageNotice(content);
            if (notice != null)
            {
                RegisterNotice(notice);
                notice.OnCompleted += () => UnregisterNotice(notice);
            }
            return notice;
        }

        public UIStageTitleNotice SpawnStageTitleNotice(StageNames stageName)
        {
            UIStageTitleNotice notice = ResourcesManager.SpawnStageTitleNotice(stageName);
            if (notice != null)
            {
                RegisterNotice(notice);
                notice.OnCompleted += () => UnregisterNotice(notice);
            }
            return notice;
        }

        public UIStageTitleNotice SpawnStageTitleNotice(string content)
        {
            UIStageTitleNotice notice = ResourcesManager.SpawnStageTitleNotice(content);
            if (notice != null)
            {
                RegisterNotice(notice);
                notice.OnCompleted += () => UnregisterNotice(notice);
            }
            return notice;
        }

        private void RegisterNotice(XBehaviour notice)
        {
            if (notice != null && !_activeNotices.Contains(notice))
            {
                _activeNotices.Add(notice);
            }
        }

        private void UnregisterNotice(XBehaviour notice)
        {
            if (notice != null && _activeNotices.Contains(notice))
            {
                _activeNotices.Remove(notice);
            }
        }

        public void Clear()
        {
            if (_activeNotices != null && _activeNotices.Count > 0)
            {
                Log.Info(LogTags.UI_Notice, "활성화된 Notice를 모두 제거합니다. 개수: {0}", _activeNotices.Count);

                List<XBehaviour> noticesToClear = new List<XBehaviour>(_activeNotices);
                for (int i = 0; i < noticesToClear.Count; i++)
                {
                    if (noticesToClear[i] != null)
                    {
                        noticesToClear[i].gameObject.SetActive(false);
                    }
                }

                _activeNotices.Clear();
            }
        }

        public void LogicUpdate()
        {
        }
    }
}