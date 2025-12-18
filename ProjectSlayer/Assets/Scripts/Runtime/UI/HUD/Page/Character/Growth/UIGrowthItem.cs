using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 성장 아이템 - 스크롤 뷰 내 개별 성장 항목
    public class UIGrowthItem : XBehaviour
    {
        [Title("#UIGrowthItem")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private Image _statIconImage;
        [SerializeField] private UILocalizedText _typeText;
        [SerializeField] private UILocalizedText _maxLevelText;
        [SerializeField] private UILocalizedText _levelText;
        [SerializeField] private UILocalizedText _statText;

        [Title("#LevelUp Button")]
        [SerializeField] private UIGrowthButton _levelUpButton;

        private GrowthData _growthData;

        public UnityEvent OnLevelUpSuccess;

        private void Awake()
        {
            AutoGetComponents();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _backgroundImage ??= this.FindComponent<Image>("Background Image");
            _frameImage ??= this.FindComponent<Image>("Top/Frame Image");
            _statIconImage ??= this.FindComponent<Image>("Top/Growth Icon Image");

            _typeText ??= this.FindComponent<UILocalizedText>("Top/Growth Type Text");
            _maxLevelText ??= this.FindComponent<UILocalizedText>("Top/Growth MaxLevel Text");

            _levelText ??= this.FindComponent<UILocalizedText>("Growth Level Text");
            _statText ??= this.FindComponent<UILocalizedText>("Growth Stat Text");

            Transform levelUpButtonTransform = this.FindTransform("LevelUp Button");
            if (levelUpButtonTransform != null)
            {
                _levelUpButton ??= levelUpButtonTransform.GetComponent<UIGrowthButton>();
            }
        }

        public void Setup(GrowthData data)
        {
            _growthData = data;

            if (_levelUpButton != null)
            {
                _levelUpButton.Setup(data, this);
                _levelUpButton.OnLevelUpSuccess.AddListener(() => OnLevelUpSuccess?.Invoke());
            }

            Refresh();
        }

        public void Refresh()
        {
            if (_growthData == null)
            {
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            int currentLevel = profile.Growth.GetLevel(_growthData.StatName);

            RefreshStatName();
            RefreshLevel(currentLevel);
            RefreshMaxLevel();
            RefreshStatValue(currentLevel);

            if (_levelUpButton != null)
            {
                _levelUpButton.Refresh();
            }
        }

        private void RefreshStatName()
        {
            if (_typeText == null)
            {
                return;
            }

            string stringKey = _growthData.GrowthType.GetStringKey();
            _typeText.SetStringKey(stringKey);
        }

        private void RefreshLevel(int currentLevel)
        {
            if (_levelText == null)
            {
                return;
            }

            StringData data = JsonDataManager.FindStringData("Format_Level");
            string content = StringGetter.Format(data, currentLevel.ToString());
            _levelText.SetText(content);
        }

        void RefreshMaxLevel()
        {
            if (_maxLevelText == null)
            {
                return;
            }

            StringData data = JsonDataManager.FindStringData("Format_MaxLevel");
            string content = StringGetter.Format(data, _growthData.MaxLevel.ToString());
            _maxLevelText.SetText(content);
        }

        private void RefreshStatValue(int currentLevel)
        {
            if (_statText == null)
            {
                return;
            }

            float currentValue = CalculateStatValue(currentLevel);
            float nextValue = CalculateStatValue(currentLevel + 1);

            string nameContent = _growthData.StatName.GetLocalizedString();
            string currentContent = _growthData.StatName.GetStatValueString(currentValue, true);
            string nextContent = _growthData.StatName.GetStatValueString(nextValue, true);
            _statText.SetText($"{nameContent} {currentContent} → {nextContent}");
        }

        private float CalculateStatValue(int level)
        {
            // 능력치 값 = 레벨 × 성장값 (InitialValue 없음)
            return level * _growthData.StatIncreasePerLevel;
        }

        public StatNames GetStatName()
        {
            return _growthData?.StatName ?? StatNames.None;
        }
    }
}

