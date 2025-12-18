using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UIExperienceGauge : XBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _levelText;

        [SerializeField]
        private TextMeshProUGUI _expText;

        [SerializeField]
        private UIGauge _experienceGauge;

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent.Register(GlobalEventType.GAME_DATA_CHARACTER_ADD_EXPERIENCE, Refresh);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent.Unregister(GlobalEventType.GAME_DATA_CHARACTER_ADD_EXPERIENCE, Refresh);
        }

        public void Refresh()
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();

            int level = profileInfo.Level.Level;
            int currentEXP = profileInfo.Level.Experience;
            int maxEXP = profileInfo.Level.GetRequiredExperience();
            float expRatio = currentEXP.SafeDivide(maxEXP);

            RefreshGauge(currentEXP, maxEXP, expRatio);
            RefreshLevelText(level);
            RefreshExperienceText(expRatio);
        }

        public void RefreshGauge(int current, int max, float rate)
        {
            if (_experienceGauge != null)
            {
                _experienceGauge.SetValueText(current, max);
                _experienceGauge.SetFrontValue(rate);
            }
        }

        private void RefreshLevelText(int level)
        {
            if (_levelText == null)
            {
                return;
            }

            StringData stringData = JsonDataManager.FindStringData("Format_Level");
            string content = StringGetter.Format(stringData, level.ToString());
            _levelText.SetText(content);
        }

        private void RefreshExperienceText(float expRatio)
        {
            if (_expText == null)
            {
                return;
            }

            _expText.SetText(ValueStringEx.GetPercentString(expRatio));
        }
    }
}