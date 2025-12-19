using System.Collections.Generic;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// Notice UI 관리자 - 일시적으로 표시되는 알림 메시지들을 관리
    /// </summary>
    public class UINoticeManager : XBehaviour
    {
        private List<UINoticeBase> _activeNotices = new List<UINoticeBase>();

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent<CurrencyNames>.Register(GlobalEventType.CURRENCY_SHORTAGE, OnCurrencyShortage);
            GlobalEvent.Register(GlobalEventType.STAT_POINT_SHORTAGE, OnStatPointShortage);
            GlobalEvent.Register(GlobalEventType.EXPERIENCE_SHORTAGE, OnExperienceShortage);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent<CurrencyNames>.Unregister(GlobalEventType.CURRENCY_SHORTAGE, OnCurrencyShortage);
            GlobalEvent.Unregister(GlobalEventType.STAT_POINT_SHORTAGE, OnStatPointShortage);
            GlobalEvent.Unregister(GlobalEventType.EXPERIENCE_SHORTAGE, OnExperienceShortage);
        }

        private void OnCurrencyShortage(CurrencyNames currencyName)
        {
            SpawnCurrencyShortageNotice(currencyName);
        }

        private void OnStatPointShortage()
        {
            SpawnStatPointShortageNotice();
        }

        private void OnExperienceShortage()
        {
            SpawnExperienceShortageNotice();
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

        public UIStatPointShortageNotice SpawnStatPointShortageNotice()
        {
            UIStatPointShortageNotice notice = ResourcesManager.SpawnStatPointShortageNotice();
            if (notice != null)
            {
                RegisterNotice(notice);
                notice.OnCompleted += () => UnregisterNotice(notice);
            }
            return notice;
        }

        public UIExperienceShortageNotice SpawnExperienceShortageNotice()
        {
            UIExperienceShortageNotice notice = ResourcesManager.SpawnExperienceShortageNotice();
            if (notice != null)
            {
                RegisterNotice(notice);
                notice.OnCompleted += () => UnregisterNotice(notice);
            }
            return notice;
        }

        //

        private void RegisterNotice(UINoticeBase notice)
        {
            if (notice != null && !_activeNotices.Contains(notice))
            {
                _activeNotices.Add(notice);
            }
        }

        private void UnregisterNotice(UINoticeBase notice)
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