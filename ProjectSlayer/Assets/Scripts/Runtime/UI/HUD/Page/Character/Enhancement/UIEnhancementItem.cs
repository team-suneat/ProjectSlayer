using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat
{
    // 강화 아이템 - 스크롤 뷰 내 개별 강화 항목
    public class UIEnhancementItem : XBehaviour
    {
        [Title("#UIEnhancementItem")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private Image _statIconImage;
        [SerializeField] private TextMeshProUGUI _statNameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _statValueText;

        [Title("#LevelUp Button")]
        [SerializeField] private Button _levelUpButton;
        [SerializeField] private Image _currencyIconImage;
        [SerializeField] private TextMeshProUGUI _costText;

        private EnhancementData _data;

        public UnityEvent OnLevelUpSuccess;

        private void Awake()
        {
            AutoGetComponents();
            RegisterEvents();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _backgroundImage ??= this.FindComponent<Image>("Background Image");
            _frameImage ??= this.FindComponent<Image>("Frame Image");
            _statIconImage ??= this.FindComponent<Image>("Stat Icon Image");
            _statNameText ??= this.FindComponent<TextMeshProUGUI>("Stat Name Text");
            _levelText ??= this.FindComponent<TextMeshProUGUI>("Level Text");
            _statValueText ??= this.FindComponent<TextMeshProUGUI>("StatValue Text");

            Transform levelUpButtonTransform = this.FindTransform("LevelUp Button");
            if (levelUpButtonTransform != null)
            {
                _levelUpButton ??= levelUpButtonTransform.GetComponent<Button>();
                _currencyIconImage ??= levelUpButtonTransform.FindComponent<Image>("Currency Icon Image");
                _costText ??= levelUpButtonTransform.FindComponent<TextMeshProUGUI>("Text (TMP)");
            }
        }

        private void RegisterEvents()
        {
            if (_levelUpButton != null)
            {
                _levelUpButton.onClick.AddListener(OnLevelUpButtonClicked);
            }
        }

        private void UnregisterEvents()
        {
            if (_levelUpButton != null)
            {
                _levelUpButton.onClick.RemoveListener(OnLevelUpButtonClicked);
            }
        }

        public void Setup(EnhancementData data)
        {
            _data = data;
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

            RefreshStatName();
            RefreshLevel(currentLevel);
            RefreshStatValue(currentLevel);
            RefreshCost(currentLevel);
            RefreshLevelUpButton(profile, currentLevel);
        }

        private void RefreshStatName()
        {
            if (_statNameText == null)
            {
                return;
            }

            string statName = _data.StatName.ToLogString();
            _statNameText.SetText(statName);
        }

        private void RefreshLevel(int currentLevel)
        {
            if (_levelText == null)
            {
                return;
            }

            _levelText.SetText($"Lv.{currentLevel}");
        }

        private void RefreshStatValue(int currentLevel)
        {
            if (_statValueText == null)
            {
                return;
            }

            float currentValue = CalculateStatValue(currentLevel);
            float nextValue = CalculateStatValue(currentLevel + 1);

            // 정수로 표시할지 소수로 표시할지 결정
            if (_data.GrowthValue >= 1f)
            {
                _statValueText.SetText($"{currentValue:N0} → {nextValue:N0}");
            }
            else
            {
                _statValueText.SetText($"{currentValue:F2} → {nextValue:F2}");
            }
        }

        private void RefreshCost(int currentLevel)
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
            if (_levelUpButton == null)
            {
                return;
            }

            bool canLevelUp = CanLevelUp(profile, currentLevel);
            _levelUpButton.interactable = canLevelUp;
        }

        private float CalculateStatValue(int level)
        {
            // 능력치 값 = 초기값 + (레벨 × 성장값)
            return _data.InitialValue + (level * _data.GrowthValue);
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

        private void OnLevelUpButtonClicked()
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

            // 레벨업 성공 이벤트 발생
            OnLevelUpSuccess?.Invoke();
        }

        public StatNames GetStatName()
        {
            return _data?.StatName ?? StatNames.None;
        }
    }
}