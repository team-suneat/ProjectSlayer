using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 강화 레벨업 버튼 컴포넌트
    public class UIEnhancementButton : UIButton
    {
        private const CurrencyNames ENHANCEMENT_COST_CURRENCY = CurrencyNames.Gold;

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

            VProfile profile = GetProfile();
            if (profile == null)
            {
                return;
            }

            int currentLevel = profile.Enhancement.GetLevel(_data.StatName);
            RefreshCurrencyText();
            RefreshCostText(currentLevel);
            RefreshLevelUpButton(profile, currentLevel);
        }

        protected override void OnButtonClick()
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

            int currentLevel = profile.Enhancement.GetLevel(_data.StatName);

            if (!TryLevelUp(profile, currentLevel))
            {
                return;
            }

            int newLevel = profile.Enhancement.GetLevel(_data.StatName);
            int cost = CalculateCost(currentLevel);

            Log.Info(LogTags.UI_Page, "{0} 강화 레벨업! Lv.{1} → Lv.{2}, 비용: {3}",
                _data.StatName, currentLevel, newLevel, cost);

            Refresh();
            _parentItem?.Refresh();
            OnLevelUpSuccess?.Invoke();
        }

        private VProfile GetProfile()
        {
            return GameApp.GetSelectedProfile();
        }

        private void RefreshCurrencyText()
        {
            if (_costCurrencyText == null)
            {
                return;
            }

            string stringKey = ENHANCEMENT_COST_CURRENCY.GetStringKey();
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
            Color buttonColor = canLevelUp ? GameColors.MediumAquamarine : GameColors.IndianRed;
            SetFrameImageColor(buttonColor);
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

            if (currentLevel >= _data.MaxLevel)
            {
                return false;
            }

            if (_data.HasRequirement)
            {
                int requiredLevel = profile.Enhancement.GetLevel(_data.RequiredStatName);
                if (requiredLevel < _data.RequiredStatLevel)
                {
                    return false;
                }
            }

            int cost = CalculateCost(currentLevel);
            if (!profile.Currency.CanUse(ENHANCEMENT_COST_CURRENCY, cost))
            {
                return false;
            }

            return true;
        }

        private bool TryLevelUp(VProfile profile, int currentLevel)
        {
            if (!CanLevelUp(profile, currentLevel))
            {
                Log.Warning(LogTags.UI_Page, "{0} 강화 레벨업 조건을 충족하지 못했습니다.", _data.StatName);
                return false;
            }

            int cost = CalculateCost(currentLevel);
            if (!profile.Currency.CanUseOrNotify(ENHANCEMENT_COST_CURRENCY, cost))
            {
                return false;
            }

            profile.Currency.Use(ENHANCEMENT_COST_CURRENCY, cost);
            profile.Enhancement.AddLevel(_data.StatName);

            return true;
        }
    }
}