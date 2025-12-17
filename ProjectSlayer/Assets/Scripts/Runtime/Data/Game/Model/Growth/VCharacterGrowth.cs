using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// 캐릭터의 성장 능력치 레벨 및 능력치 포인트 저장 데이터
    /// </summary>
    [System.Serializable]
    public class VCharacterGrowth
    {
        /// <summary>
        /// 능력치 포인트 (레벨업 시 획득)
        /// </summary>
        public int StatPoint = 0;

        /// <summary>
        /// 성장 능력치별 레벨 (string -> Level)
        /// </summary>
        public Dictionary<string, int> GrowthLevels = new();

        public void OnLoadGameData()
        {
            GrowthLevels ??= new Dictionary<string, int>();

            Log.Info(LogTags.GameData_Character, "[Character] 성장 능력치 레벨 데이터를 불러옵니다. 총 {0}개, 능력치 포인트: {1}",
                GrowthLevels.Count, StatPoint);
        }

        public void ClearIngameData()
        {
            GrowthLevels.Clear();
            StatPoint = 0;
            Log.Info(LogTags.GameData_Character, "성장 능력치 레벨 및 능력치 포인트 데이터를 초기화합니다.");
        }

        public int GetLevel(StatNames statName)
        {
            if (GrowthLevels == null)
            {
                return 0;
            }

            string key = statName.ToString();
            return GrowthLevels.TryGetValue(key, out int level) ? level : 0;
        }

        public int GetLevel(CharacterGrowthTypes growthType)
        {
            if (growthType == CharacterGrowthTypes.None)
            {
                return 0;
            }

            StatNames statName = GrowthTypeToStatName(growthType);
            return GetLevel(statName);
        }

        public void SetLevel(StatNames statName, int level)
        {
            GrowthLevels ??= new Dictionary<string, int>();

            string key = statName.ToString();
            GrowthLevels[key] = level;
            Log.Info(LogTags.GameData_Character, "성장 능력치 {0}의 레벨을 {1}로 설정합니다.", statName, level);
        }

        public void SetLevel(CharacterGrowthTypes growthType, int level)
        {
            if (growthType == CharacterGrowthTypes.None)
            {
                Log.Warning(LogTags.GameData_Character, "잘못된 성장 타입입니다: {0}", growthType);
                return;
            }

            StatNames statName = GrowthTypeToStatName(growthType);
            SetLevel(statName, level);
        }

        public int AddLevel(StatNames statName, int addLevel = 1)
        {
            int currentLevel = GetLevel(statName);
            int newLevel = currentLevel + addLevel;
            SetLevel(statName, newLevel);
            return newLevel;
        }

        public int AddLevel(CharacterGrowthTypes growthType, int addLevel = 1)
        {
            if (growthType == CharacterGrowthTypes.None)
            {
                Log.Warning(LogTags.GameData_Character, "잘못된 성장 타입입니다: {0}", growthType);
                return 0;
            }

            StatNames statName = GrowthTypeToStatName(growthType);
            return AddLevel(statName, addLevel);
        }

        private static StatNames GrowthTypeToStatName(CharacterGrowthTypes growthType)
        {
            return growthType switch
            {
                CharacterGrowthTypes.Strength => StatNames.Strength,
                CharacterGrowthTypes.HealthPoint => StatNames.HealthPoint,
                CharacterGrowthTypes.Vitality => StatNames.Vitality,
                CharacterGrowthTypes.Critical => StatNames.Critical,
                CharacterGrowthTypes.Luck => StatNames.Luck,
                CharacterGrowthTypes.AccuracyStat => StatNames.AccuracyStat,
                CharacterGrowthTypes.Dodge => StatNames.Dodge,
                _ => StatNames.None
            };
        }

        public int AddStatPoint(int addPoint)
        {
            StatPoint += addPoint;
            Log.Info(LogTags.GameData_Character, "능력치 포인트를 {0} 추가합니다. 총 능력치 포인트: {1}", addPoint, StatPoint);
            return StatPoint;
        }

        public bool ConsumeStatPoint(int consumePoint)
        {
            if (StatPoint < consumePoint)
            {
                Log.Warning(LogTags.GameData_Character, "능력치 포인트가 부족합니다. 현재: {0}, 필요: {1}", StatPoint, consumePoint);
                return false;
            }

            StatPoint -= consumePoint;
            Log.Info(LogTags.GameData_Character, "능력치 포인트를 {0} 소비합니다. 남은 능력치 포인트: {1}", consumePoint, StatPoint);
            return true;
        }

        public void ResetAllLevels()
        {
            GrowthLevels?.Clear();
            StatPoint = 0;
        }

        public static VCharacterGrowth CreateDefault()
        {
            return new VCharacterGrowth
            {
                StatPoint = 0
            };
        }
    }
}