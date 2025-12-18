using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    // 레벨업 버튼 - 경험치가 충분할 때 활성화되고 레벨업을 수행
    public class UICharacterLevelUpButton : UIButton
    {
        [FoldoutGroup("#UIButton-CharacterLevelUp"), SerializeField]
        public UnityEvent OnLevelUpSuccess;

        protected override void OnButtonClick()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            if (!CanLevelUp(profile))
            {
                Log.Warning(LogTags.UI_Page, "경험치가 부족하여 레벨업할 수 없습니다.");
                return;
            }

            int addedLevel = LevelUp(profile);
            if (addedLevel > 0)
            {
                AddStatPoint(profile);
                RefreshUI();
                OnLevelUpSuccess?.Invoke();
            }
        }

        private int LevelUp(VProfile profile)
        {
            return profile.Level.LevelUp();
        }

        private void AddStatPoint(VProfile profile)
        {

            ExperienceConfigAsset experienceConfigAsset = ScriptableDataManager.Instance.GetExperienceConfigAsset();
            if (experienceConfigAsset == null)
            {
                return;
            }

            int statPointPerLevel = experienceConfigAsset.StatPointPerLevel;
            profile.Growth.AddStatPoint(statPointPerLevel);

        }

        public void RefreshUI()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                SetFrameImageColor(GameColors.IndianRed);
                return;
            }

            bool canLevelUp = CanLevelUp(profile);

            if (canLevelUp)
            {
                SetFrameImageColor(GameColors.SteelBlue);
            }
            else
            {
                SetFrameImageColor(GameColors.IndianRed);
            }
        }

        private bool CanLevelUp(VProfile profile)
        {
            if (profile == null)
            {
                return false;
            }

            float expRatio = CalculateExpRatio(profile);
            return expRatio >= 1.0f;
        }

        private float CalculateExpRatio(VProfile profile)
        {
            int currentExp = profile.Level.Experience;
            int currentLevel = profile.Level.Level;

            ExperienceConfigAsset experienceConfigAsset = ScriptableDataManager.Instance.GetExperienceConfigAsset();
            if (experienceConfigAsset == null)
            {
                return 0f;
            }

            int requiredExperience = experienceConfigAsset.GetExperienceRequiredForNextLevel(currentLevel);
            return currentExp.SafeDivide01(requiredExperience);
        }
    }
}