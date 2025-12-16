using System.Collections.Generic;
using TeamSuneat;

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
        public Dictionary<string, int> GrowthLevels = new Dictionary<string, int>();

        public void OnLoadGameData()
        {
            if (GrowthLevels == null)
            {
                GrowthLevels = new Dictionary<string, int>();
            }

            Log.Info(LogTags.GameData, "[Character] 성장 능력치 레벨 데이터를 불러옵니다. 총 {0}개, 능력치 포인트: {1}", 
                GrowthLevels.Count, StatPoint);
        }

        public void ClearIngameData()
        {
            GrowthLevels.Clear();
            StatPoint = 0;
            Log.Info(LogTags.GameData, "[Character] 성장 능력치 레벨 및 능력치 포인트 데이터를 초기화합니다.");
        }

        /// <summary>
        /// 특정 성장 능력치의 레벨을 가져옵니다.
        /// </summary>
        public int GetLevel(StatNames statName)
        {
            if (GrowthLevels == null)
            {
                return 0;
            }

            string key = statName.ToString();
            return GrowthLevels.TryGetValue(key, out int level) ? level : 0;
        }

        /// <summary>
        /// 특정 성장 능력치의 레벨을 설정합니다.
        /// </summary>
        public void SetLevel(StatNames statName, int level)
        {
            if (GrowthLevels == null)
            {
                GrowthLevels = new Dictionary<string, int>();
            }

            string key = statName.ToString();
            GrowthLevels[key] = level;
            Log.Info(LogTags.GameData, "[Character] 성장 능력치 {0}의 레벨을 {1}로 설정합니다.", statName, level);
        }

        /// <summary>
        /// 특정 성장 능력치의 레벨을 증가시킵니다.
        /// </summary>
        public int AddLevel(StatNames statName, int addLevel = 1)
        {
            int currentLevel = GetLevel(statName);
            int newLevel = currentLevel + addLevel;
            SetLevel(statName, newLevel);
            return newLevel;
        }

        /// <summary>
        /// 능력치 포인트를 추가합니다.
        /// </summary>
        public int AddStatPoint(int addPoint)
        {
            StatPoint += addPoint;
            Log.Info(LogTags.GameData, "[Character] 능력치 포인트를 {0} 추가합니다. 총 능력치 포인트: {1}", addPoint, StatPoint);
            return StatPoint;
        }

        /// <summary>
        /// 능력치 포인트를 소비합니다.
        /// </summary>
        public bool ConsumeStatPoint(int consumePoint)
        {
            if (StatPoint < consumePoint)
            {
                Log.Warning(LogTags.GameData, "[Character] 능력치 포인트가 부족합니다. 현재: {0}, 필요: {1}", StatPoint, consumePoint);
                return false;
            }

            StatPoint -= consumePoint;
            Log.Info(LogTags.GameData, "[Character] 능력치 포인트를 {0} 소비합니다. 남은 능력치 포인트: {1}", consumePoint, StatPoint);
            return true;
        }

        /// <summary>
        /// 모든 성장 능력치 레벨을 초기화합니다.
        /// </summary>
        public void ResetAllLevels()
        {
            if (GrowthLevels != null)
            {
                GrowthLevels.Clear();
            }
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

