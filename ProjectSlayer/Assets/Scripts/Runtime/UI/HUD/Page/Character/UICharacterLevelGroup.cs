using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TeamSuneat.UserInterface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat
{
    // 캐릭터 정보 영역 - 아이콘, 레벨, 경험치, 레벨업 버튼
    public class UICharacterLevelGroup : XBehaviour
    {
        [Title("#UICharacterLevelGroup")]
        [SerializeField] private Image _characterIconImage;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _expText;
        [SerializeField] private UIGauge _expGauge;
        [SerializeField] private Button _levelUpButton;

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

            _characterIconImage ??= this.FindComponent<Image>("Character Icon Image");
            _levelText ??= this.FindComponent<TextMeshProUGUI>("Level Name Text");
            _expText ??= this.FindComponent<TextMeshProUGUI>("Exp Name Text");
            _expGauge ??= GetComponentInChildren<UIGauge>();
            _levelUpButton ??= this.FindComponent<Button>("LevelUp Button");
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

        public void Refresh()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            RefreshLevelText(profile);
            RefreshExpText(profile);
            RefreshExpGauge(profile);
            RefreshLevelUpButton(profile);
        }

        private void RefreshLevelText(VProfile profile)
        {
            if (_levelText == null)
            {
                return;
            }

            int level = profile.Level.Level;
            _levelText.SetText($"Lv.{level}");
        }

        private void RefreshExpText(VProfile profile)
        {
            if (_expText == null)
            {
                return;
            }

            float expRatio = CalculateExpRatio(profile);
            _expText.SetText($"{expRatio * 100f:F2}%");
        }

        private void RefreshExpGauge(VProfile profile)
        {
            if (_expGauge == null)
            {
                return;
            }

            float expRatio = CalculateExpRatio(profile);
            _expGauge.SetFrontValue(expRatio);
        }

        private void RefreshLevelUpButton(VProfile profile)
        {
            if (_levelUpButton == null)
            {
                return;
            }

            float expRatio = CalculateExpRatio(profile);
            _levelUpButton.interactable = expRatio >= 1.0f;
        }

        private float CalculateExpRatio(VProfile profile)
        {
            int currentExp = profile.Level.Experience;
            int currentLevel = profile.Level.Level;

            CharacterLevelExpData expData = JsonDataManager.FindCharacterLevelExpDataClone(currentLevel);
            if (expData == null)
            {
                return 0f;
            }

            int requiredExp = expData.RequiredExperience;
            if (requiredExp <= 0)
            {
                return 0f;
            }

            return Mathf.Clamp01((float)currentExp / requiredExp);
        }

        private void OnLevelUpButtonClicked()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            float expRatio = CalculateExpRatio(profile);
            if (expRatio < 1.0f)
            {
                Log.Warning(LogTags.UI_Page, "경험치가 부족하여 레벨업할 수 없습니다.");
                return;
            }

            // 레벨업 처리
            CharacterLevelExpData expData = JsonDataManager.FindCharacterLevelExpDataClone(profile.Level.Level);
            if (expData != null)
            {
                // 필요 경험치 차감
                profile.Level.Experience -= expData.RequiredExperience;

                // 레벨 증가
                int addedLevel = profile.Level.LevelUp();

                if (addedLevel > 0)
                {
                    // 능력치 포인트 추가
                    int statPointPerLevel = GetStatPointPerLevel();
                    profile.Growth.AddStatPoint(statPointPerLevel * addedLevel);

                    Log.Info(LogTags.UI_Page, "레벨업! Lv.{0}, 능력치 포인트 +{1}", profile.Level.Level, statPointPerLevel * addedLevel);
                }
            }

            // UI 갱신
            Refresh();
        }

        private int GetStatPointPerLevel()
        {
            ExperienceConfigAsset config = ScriptableDataManager.Instance?.GetExperienceConfigAsset();
            if (config != null)
            {
                return config.StatPointPerLevel;
            }

            return 3; // 기본값
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}