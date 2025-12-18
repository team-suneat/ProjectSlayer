using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 강화 아이템 - 스크롤 뷰 내 개별 강화 항목
    public class UIEnhancementItem : XBehaviour
    {
        [Title("#UIEnhancementItem")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private Image _statIconImage;
        [SerializeField] private UILocalizedText _statNameText;
        [SerializeField] private UILocalizedText _levelText;
        [SerializeField] private TextMeshProUGUI _statValueText;

        [Title("#LevelUp Button")]
        [SerializeField] private UIEnhancementButton _levelUpButton;

        private EnhancementData _enhancementData;

        public UnityEvent OnLevelUpSuccess;

        private void Awake()
        {
            AutoGetComponents();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _backgroundImage ??= this.FindComponent<Image>("Background Image");
            _frameImage ??= this.FindComponent<Image>("Frame Image");
            _statIconImage ??= this.FindComponent<Image>("Stat Icon Image");
            _statNameText ??= this.FindComponent<UILocalizedText>("Stat Name Text");
            _levelText ??= this.FindComponent<UILocalizedText>("Level Text");
            _statValueText ??= this.FindComponent<TextMeshProUGUI>("StatValue Text");

            Transform levelUpButtonTransform = this.FindTransform("LevelUp Button");
            if (levelUpButtonTransform != null)
            {
                _levelUpButton ??= levelUpButtonTransform.GetComponent<UIEnhancementButton>();
            }
        }

        public void Setup(EnhancementData data)
        {
            _enhancementData = data;

            if (_levelUpButton != null)
            {
                _levelUpButton.Setup(data, this);
                _levelUpButton.OnLevelUpSuccess.AddListener(() => OnLevelUpSuccess?.Invoke());
            }

            Refresh();
        }

        public void Refresh()
        {
            if (_enhancementData == null)
            {
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            int currentLevel = profile.Enhancement.GetLevel(_enhancementData.StatName);

            RefreshStatName();
            RefreshLevel(currentLevel);
            RefreshStatValue(currentLevel);

            if (_levelUpButton != null)
            {
                _levelUpButton.Refresh();
            }
        }

        private void RefreshStatName()
        {
            if (_statNameText == null)
            {
                return;
            }

            string stringKey = _enhancementData.StatName.GetStringKey();
            _statNameText.SetStringKey(stringKey);
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

        private void RefreshStatValue(int currentLevel)
        {
            if (_statValueText == null)
            {
                return;
            }

            float currentValue = CalculateStatValue(currentLevel);
            float nextValue = CalculateStatValue(currentLevel + 1);

            string currentContent = _enhancementData.StatName.GetStatValueString(currentValue);
            string nextContent = _enhancementData.StatName.GetStatValueString(nextValue);
            _statValueText.SetText($"{currentContent} → {nextContent}");
        }

        private float CalculateStatValue(int level)
        {
            // 능력치 값 = 초기값 + (레벨 × 성장값)
            return _enhancementData.InitialValue + (level * _enhancementData.GrowthValue);
        }

        public StatNames GetStatName()
        {
            return _enhancementData?.StatName ?? StatNames.None;
        }
    }
}