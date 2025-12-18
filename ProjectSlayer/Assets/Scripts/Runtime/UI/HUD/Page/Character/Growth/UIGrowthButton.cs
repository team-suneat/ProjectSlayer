using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            int currentLevel = profile.Growth.GetLevel(_data.StatName);    
            RefreshLevelUpButton(profile, currentLevel);
        }

     
        private void RefreshLevelUpButton(VProfile profile, int currentLevel)
        {
            bool canLevelUp = CanLevelUp(profile, currentLevel);

            if (canLevelUp)
            {
                SetFrameImageColor(GameColors.SteelBlue);
            }
            else
            {
                SetFrameImageColor(GameColors.IndianRed);
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

            // 최대 레벨 확인
            if (currentLevel >= _data.MaxLevel)
            {
                return false;
            }

            // 능력치 포인트 확인
            int cost = CalculateCost();
            if (profile.Growth.StatPoint < cost)
            {
                return false;
            }

            return true;
        }

        protected override void OnButtonClick()
        {
            if (_data == null)
            {
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            int currentLevel = profile.Growth.GetLevel(_data.StatName);

            // 레벨업 가능 여부 확인
            if (!CanLevelUp(profile, currentLevel))
            {
                Log.Warning(LogTags.UI_Page, "{0} 성장 레벨업 조건을 충족하지 못했습니다.", _data.StatName);
                return;
            }

            // 능력치 포인트 차감
            int cost = CalculateCost();
            if (!profile.Growth.ConsumeStatPoint(cost))
            {
                Log.Warning(LogTags.UI_Page, "능력치 포인트가 부족합니다.");
                return;
            }

            // 레벨 증가
            int newLevel = profile.Growth.AddLevel(_data.StatName);

            Log.Info(LogTags.UI_Page, "{0} 성장 레벨업! Lv.{1} → Lv.{2}, 비용: {3}",
                _data.StatName, currentLevel, newLevel, cost);

            // UI 갱신
            Refresh();
            _parentItem?.Refresh();

            // 레벨업 성공 이벤트 발생
            OnLevelUpSuccess?.Invoke();
        }
    }
}

