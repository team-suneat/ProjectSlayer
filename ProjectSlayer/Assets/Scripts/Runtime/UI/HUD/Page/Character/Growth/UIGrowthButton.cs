using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    public class UIGrowthButton : UIButton
    {
        [FoldoutGroup("#UIButton-Growth"), SerializeField]
        public UnityEvent OnLevelUpSuccess;

        private GrowthData _data;
        private UIGrowthItem _parentItem;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
        }

        private VProfile GetProfile()
        {
            return GameApp.GetSelectedProfile();
        }

        public void Setup(GrowthData data, UIGrowthItem parentItem)
        {
            _data = data;
            _parentItem = parentItem;
            Refresh();
        }

        public void Refresh()
        {
            if (_data == null)
            {
                return;
            }

            VProfile profile = GetProfile();
            if (profile == null)
            {
                return;
            }

            int currentLevel = profile.Growth.GetLevel(_data.StatName);
            RefreshLevelUpButton(profile, currentLevel);
        }

        protected override void OnButtonClick()
        {
            TryPerformLevelUp();
        }

        protected override void OnButtonHold()
        {
            TryPerformLevelUp();
        }

        private void TryPerformLevelUp()
        {
            if (_data == null)
            {
                return;
            }

            VProfile profile = GetProfile();
            if (profile == null)
            {
                return;
            }

            int currentLevel = profile.Growth.GetLevel(_data.StatName);
            if (TryLevelUp(profile, currentLevel))
            {
                Refresh();
                _parentItem?.Refresh();
                OnLevelUpSuccess?.Invoke();
            }
        }

        private int CalculateCost()
        {
            // 비용은 항상 1로 고정 (레벨에 관계없이 능력치 포인트 1개 소비)
            return 1;
        }

        private bool CanLevelUp(VProfile profile, int currentLevel)
        {
            if (_data == null)
            {
                return false;
            }

            if (currentLevel >= _data.MaxLevel)
            {
                return false;
            }

            return true;
        }

        private bool TryLevelUp(VProfile profile, int currentLevel)
        {
            if (!CanLevelUp(profile, currentLevel))
            {
                return false;
            }

            int cost = CalculateCost();
            if (!profile.Growth.CanConsumeStatPointOrNotify(cost))
            {
                Log.Warning(LogTags.UI_Page, "{0} 성장 레벨업 조건을 충족하지 못했습니다.", _data.StatName);

                return false;
            }

            profile.Growth.ConsumeStatPoint(cost);
            profile.Growth.AddLevel(_data.StatName);

            return true;
        }

        private void RefreshLevelUpButton(VProfile profile, int currentLevel)
        {
            bool canLevelUp = CanLevelUp(profile, currentLevel);
            if (canLevelUp)
            {
                int cost = CalculateCost();
                if (!profile.Growth.CanConsumeStatPoint(cost))
                {
                    canLevelUp = false;
                }
            }

            Color buttonColor = canLevelUp ? GameColors.SteelBlue : GameColors.IndianRed;
            SetFrameImageColor(buttonColor);
        }
    }
}