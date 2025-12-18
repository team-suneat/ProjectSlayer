using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIEnhancementButton : UIButton
    {
        [FoldoutGroup("#UIButton-Enhancement"), SerializeField]
        private Image _currencyIconImage;

        [FoldoutGroup("#UIButton-Enhancement"), SerializeField]
        private UILocalizedText _costCurrencyText;

        [FoldoutGroup("#UIButton-Enhancement"), SerializeField]
        private TextMeshProUGUI _costText;

        [FoldoutGroup("#UIButton-Enhancement"), SerializeField]
        public UnityEvent OnLevelUpSuccess;

        private EnhancementData _data;
        private UIEnhancementItem _parentItem;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _currencyIconImage ??= this.FindComponent<Image>("Currency Icon Image");
            _costCurrencyText ??= this.FindComponent<UILocalizedText>("LevelUp Currency Text");
            _costText ??= this.FindComponent<TextMeshProUGUI>("LevelUp Cost Text");
        }

        public void Setup(EnhancementData data, UIEnhancementItem parentItem)
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

            int currentLevel = profile.Enhancement.GetLevel(_data.StatName);
            RefreshCurrencyText();
            RefreshCostText(currentLevel);
            RefreshLevelUpButton(profile, currentLevel);
        }

        private void RefreshCurrencyText()
        {
            if (_costCurrencyText == null)
            {
                return;
            }

            string stringKey = CurrencyNames.Gold.GetStringKey();
            _costCurrencyText.SetStringKey(stringKey);
        }

        private void RefreshCostText(int currentLevel)
        {
            if (_costText == null)
            {
                return;
            }

            int cost = CalculateCost(currentLevel);
            _costText.SetText(cost.ToString("N0"));
        }

        private void RefreshLevelUpButton(VProfile profile, int currentLevel)
        {
            bool canLevelUp = CanLevelUp(profile, currentLevel);

            if (canLevelUp)
            {
                SetFrameImageColor(GameColors.MediumAquamarine);
            }
            else
            {
                SetFrameImageColor(GameColors.IndianRed);
            }
        }

        private int CalculateCost(int level)
        {
            if (level <= 0)
            {
                return _data.InitialCost;
            }

            // 비용 = 초기 비용 × 비용 성장률^(레벨-1)
            return Mathf.RoundToInt(_data.InitialCost * Mathf.Pow(_data.CostGrowthRate, level - 1));
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

            // 요구 능력치 확인
            if (_data.HasRequirement)
            {
                int requiredLevel = profile.Enhancement.GetLevel(_data.RequiredStatName);
                if (requiredLevel < _data.RequiredStatLevel)
                {
                    return false;
                }
            }

            // 재화 확인
            int cost = CalculateCost(currentLevel);
            if (!profile.Currency.CanUseOrNotify(CurrencyNames.Gold, cost))
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

            int currentLevel = profile.Enhancement.GetLevel(_data.StatName);

            // 레벨업 가능 여부 확인
            if (!CanLevelUp(profile, currentLevel))
            {
                Log.Warning(LogTags.UI_Page, "{0} 강화 레벨업 조건을 충족하지 못했습니다.", _data.StatName);
                return;
            }

            // 비용 차감
            int cost = CalculateCost(currentLevel);
            profile.Currency.Use(CurrencyNames.Gold, cost);

            // 레벨 증가
            int newLevel = profile.Enhancement.AddLevel(_data.StatName);

            Log.Info(LogTags.UI_Page, "{0} 강화 레벨업! Lv.{1} → Lv.{2}, 비용: {3}",
                _data.StatName, currentLevel, newLevel, cost);

            // UI 갱신
            Refresh();
            _parentItem?.Refresh();

            // 레벨업 성공 이벤트 발생
            OnLevelUpSuccess?.Invoke();
        }
    }
}