using TeamSuneat;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 성장 시스템 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Growth Get Methods

        /// <summary>
        /// 성장 시스템 데이터 에셋을 가져옵니다.
        /// </summary>
        public GrowthDataAsset GetGrowthDataAsset()
        {
            return _growthDataAsset;
        }

        /// <summary>
        /// 스탯 이름으로 성장 데이터를 가져옵니다.
        /// </summary>
        public GrowthData GetGrowthData(StatNames statName)
        {
            if (_growthDataAsset == null)
            {
                return null;
            }

            return _growthDataAsset.FindGrowthData(statName);
        }

        #endregion Growth Get Methods

        #region Growth Load Methods

        // LoadGrowthDataSync는 ScriptableDataManager.Load.cs에 정의됨

        #endregion Growth Load Methods

        #region Growth Refresh Methods

        /// <summary>
        /// 성장 시스템 데이터 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllGrowth()
        {
            _growthDataAsset?.Refresh();
        }

        #endregion Growth Refresh Methods

        #region Growth Validation Methods

        /// <summary>
        /// 성장 시스템 데이터 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidGrowthsOnLoadAssets()
        {
#if UNITY_EDITOR
            if (_growthDataAsset == null)
            {
                Log.Warning(LogTags.ScriptableData, "성장 시스템 데이터 에셋이 설정되지 않았습니다.");
                return;
            }

            if (_growthDataAsset.DataArray == null || _growthDataAsset.DataArray.Length == 0)
            {
                Log.Warning(LogTags.ScriptableData, "성장 시스템 데이터 배열이 비어있습니다.");
                return;
            }

            // 성장 시스템 능력치만 체크
            StatNames[] growthStatNames = new StatNames[]
            {
                StatNames.Strength,
                StatNames.HealthPoint,
                StatNames.Vitality,
                StatNames.Critical,
                StatNames.Luck,
                StatNames.AccuracyStat,
                StatNames.Dodge
            };

            for (int i = 0; i < growthStatNames.Length; i++)
            {
                StatNames statName = growthStatNames[i];
                GrowthData data = _growthDataAsset.FindGrowthData(statName);
                if (data == null)
                {
                    Log.Warning(LogTags.ScriptableData, "성장 시스템 데이터가 설정되지 않았습니다. {0}({1})", statName, statName.ToLogString());
                }
            }
#endif
        }

        #endregion Growth Validation Methods
    }
}

