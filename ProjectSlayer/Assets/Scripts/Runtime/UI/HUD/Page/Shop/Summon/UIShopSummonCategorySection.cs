using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 소환 카테고리 섹션 - 무기/악세사리/스킬 카드 중 하나, 3개 옵션 버튼 보유
    public class UIShopSummonCategorySection : XBehaviour
    {
        [Title("#UIShopSummonCategorySection")]
        [SerializeField] private UIShopSummonOptionButton _gemSingleButton;
        [SerializeField] private UIShopSummonOptionButton _gemMultiButton;
        [SerializeField] private UIShopSummonOptionButton _adMultiButton;
        [SerializeField] private UILocalizedText _categoryTitleText;
        [SerializeField] private UILocalizedText _summonLevelText;
        [SerializeField] private UIGauge _summonLevelGauge;

        public ShopSummonOptionUnityEvent OnOptionClicked => _onOptionClicked;
        private readonly ShopSummonOptionUnityEvent _onOptionClicked = new();
        private ShopSummonCategory _category;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            UIShopSummonOptionButton[] buttons = GetComponentsInChildren<UIShopSummonOptionButton>(true);
            if (buttons.Length >= 3)
            {
                _gemSingleButton ??= buttons[0];
                _gemMultiButton ??= buttons[1];
                _adMultiButton ??= buttons[2];
            }

            _categoryTitleText ??= GetComponentInChildren<UILocalizedText>(true);
            _summonLevelText ??= this.FindComponent<UILocalizedText>("Summon Level Text");
            _summonLevelGauge ??= GetComponentInChildren<UIGauge>(true);
        }

        public void Setup(ShopSummonCategory category)
        {
            _category = category;
            RegisterButtonEvents();

            _gemSingleButton?.Setup(category, ShopSummonOptionType.GemSingle);
            _gemMultiButton?.Setup(category, ShopSummonOptionType.GemMulti);
            _adMultiButton?.Setup(category, ShopSummonOptionType.AdMulti);

            RefreshCategoryTitle(category);
            RefreshSummonLevel();
            RefreshSummonLevelGauge();
        }

        public void Refresh()
        {
            _gemSingleButton?.Refresh();
            _gemMultiButton?.Refresh();
            _adMultiButton?.Refresh();
            RefreshSummonLevel();
            RefreshSummonLevelGauge();
        }

        public void RefreshSummonLevel()
        {
            if (_summonLevelText == null)
            {
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                _summonLevelText.SetText(string.Empty);
                return;
            }

            string format = JsonDataManager.FindStringClone(StringDataLabels.FORMAT_SUMMON_LEVEL);
            if (string.IsNullOrEmpty(format))
            {
                format = "소환 레벨 {0}";
            }
            int level = GetSummonLevelForCategory(profile, _category);
            _summonLevelText.SetText(string.Format(format, level.ToString()));
        }

        private void RefreshSummonLevelGauge()
        {
            if (_summonLevelGauge == null)
            {
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                _summonLevelGauge.ResetFrontValue();
                _summonLevelGauge.ResetValueText();
                return;
            }

            (int level, int experience) = GetSummonLevelAndExperienceForCategory(profile, _category);
            SummonLevelConfigAsset asset = ScriptableDataManager.Instance?.GetSummonLevelConfigAsset();
            if (asset == null)
            {
                _summonLevelGauge.SetFrontValue(0f);
                _summonLevelGauge.SetValueText(experience, 0);
                return;
            }

            int currentTotal = level == 0 ? 0 : asset.GetRequiredSummonCountForLevel(level + 1);
            int nextTotal = asset.GetRequiredSummonCountForLevel(level + 2);
            int requiredExperience = nextTotal - currentTotal;

            float fillAmount = requiredExperience > 0
                ? experience.SafeDivide01(requiredExperience)
                : 1f;
            _summonLevelGauge.SetFrontValue(fillAmount);
            _summonLevelGauge.SetBackValue(fillAmount);
            _summonLevelGauge.SetValueText(experience, requiredExperience > 0 ? requiredExperience : 0);
        }

        private void RegisterButtonEvents()
        {
            UnregisterButtonEvents();

            if (_gemSingleButton != null)
            {
                _gemSingleButton.OnOptionClicked.AddListener(ForwardOptionClicked);
            }

            if (_gemMultiButton != null)
            {
                _gemMultiButton.OnOptionClicked.AddListener(ForwardOptionClicked);
            }

            if (_adMultiButton != null)
            {
                _adMultiButton.OnOptionClicked.AddListener(ForwardOptionClicked);
            }
        }

        private void UnregisterButtonEvents()
        {
            if (_gemSingleButton != null)
            {
                _gemSingleButton.OnOptionClicked.RemoveListener(ForwardOptionClicked);
            }

            if (_gemMultiButton != null)
            {
                _gemMultiButton.OnOptionClicked.RemoveListener(ForwardOptionClicked);
            }

            if (_adMultiButton != null)
            {
                _adMultiButton.OnOptionClicked.RemoveListener(ForwardOptionClicked);
            }
        }

        private void ForwardOptionClicked(ShopSummonCategory category, ShopSummonOptionType optionType)
        {
            _onOptionClicked?.Invoke(category, optionType);
            RefreshSummonLevel();
            RefreshSummonLevelGauge();
        }

        private void RefreshCategoryTitle(ShopSummonCategory category)
        {
            if (_categoryTitleText == null)
            {
                return;
            }

            if (category == ShopSummonCategory.None)
            {
                return;
            }

            string stringKey = GetCategoryTitleStringKey(category);
            _categoryTitleText.SetStringKey(stringKey);
        }

        private static string GetCategoryTitleStringKey(ShopSummonCategory category)
        {
            return category switch
            {
                ShopSummonCategory.Weapon => "Shop_Summon_Weapon",
                ShopSummonCategory.Accessory => "Shop_Summon_Accessory",
                ShopSummonCategory.SkillCard => "Shop_Summon_SkillCard",
                _ => "Shop_Summon_Unknown"
            };
        }

        private static int GetSummonLevelForCategory(VProfile profile, ShopSummonCategory category)
        {
            return category switch
            {
                ShopSummonCategory.Weapon => profile?.Weapon?.SummonLevel ?? 0,
                ShopSummonCategory.Accessory => profile?.Accessory?.SummonLevel ?? 0,
                ShopSummonCategory.SkillCard => 0,
                _ => 0
            };
        }

        private static (int level, int experience) GetSummonLevelAndExperienceForCategory(VProfile profile, ShopSummonCategory category)
        {
            if (profile == null)
            {
                return (0, 0);
            }

            return category switch
            {
                ShopSummonCategory.Weapon => (profile.Weapon?.SummonLevel ?? 0, profile.Weapon?.SummonExperience ?? 0),
                ShopSummonCategory.Accessory => (profile.Accessory?.SummonLevel ?? 0, profile.Accessory?.SummonExperience ?? 0),
                ShopSummonCategory.SkillCard => (0, 0),
                _ => (0, 0)
            };
        }
    }
}