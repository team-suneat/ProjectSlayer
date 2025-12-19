using UnityEngine;

namespace TeamSuneat.Data.Game
{
    public class VCharacterLevel
    {
        public int Level;
        public int Experience;

        public void ResetValues()
        {
            Level = 1;
            Experience = 0;

            Log.Info(LogTags.GameData_Character, "플레이어 캐릭터의 레벨과 경험치를 초기화합니다.");
        }

        public void AddExperience(int experience)
        {
            if (CharacterManager.Instance.Player == null)
            {
                return;
            }

            float multiplier = CharacterManager.Instance.Player.Stat.FindValueOrDefault(StatNames.XPGain);

            Log.Info(LogTags.GameData_Character, "받는 경험치 획득량을 결정합니다. EXP: {0}, 추가 획득량 {1}", experience, multiplier);

            if (multiplier > 0)
            {
                Experience += Mathf.RoundToInt(experience * multiplier);
            }
            else
            {
                Experience += experience;
            }

            GlobalEvent.Send(GlobalEventType.GAME_DATA_CHARACTER_ADD_EXPERIENCE);
        }

        public int GetRequiredExperience()
        {
            ExperienceConfigAsset config = ScriptableDataManager.Instance.GetExperienceConfigAsset();
            if (config != null)
            {
                return config.GetExperienceRequiredForNextLevel(Level);
            }

            return 0;
        }

        public bool CanLevelUp()
        {
            if (Level >= GameDefine.CHARACTER_MAX_LEVEL)
            {
                return false;
            }

            int requiredExperience = GetRequiredExperience();
            return Experience >= requiredExperience;
        }

        public bool CanLevelUpOrNotify()
        {
            bool canLevelUp = CanLevelUp();
            if (!canLevelUp)
            {
                GlobalEvent.Send(GlobalEventType.EXPERIENCE_SHORTAGE);
            }
            return canLevelUp;
        }

        public int LevelUp()
        {
            return LevelUp(1);
        }

        public int LevelUp(int addLevel)
        {
            if (Level >= GameDefine.CHARACTER_MAX_LEVEL)
            {
                return 0;
            }

            int addedLevel = 0;
            for (int i = 0; i < addLevel; i++)
            {
                if (Level >= GameDefine.CHARACTER_MAX_LEVEL)
                {
                    break;
                }

                int requiredExperience = GetRequiredExperience();
                if (Experience < requiredExperience)
                {
                    break;
                }

                Experience -= requiredExperience;
                Level += 1;
                addedLevel += 1;
                Log.Info(LogTags.GameData_Character, "플레이어 캐릭터의 레벨이 올랐습니다: Lv.{0}", Level.ToString());
            }

            if (addedLevel > 0 && CharacterManager.Instance.Player != null)
            {
                CharacterManager.Instance.Player.OnLevelup(addedLevel);
            }

            if (addedLevel > 0)
            {
                GlobalEvent<int>.Send(GlobalEventType.GAME_DATA_CHARACTER_LEVEL_CHANGED, Level);
            }

            return addedLevel;
        }

        public static VCharacterLevel CreateDefault()
        {
            return new VCharacterLevel()
            {
                Level = 1,
                Experience = 0,
            };
        }
    }
}