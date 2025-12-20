using TeamSuneat;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 강화 시스템 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Enhancement Get Methods

        /// <summary>
        /// 강화 시스템 데이터 에셋을 가져옵니다.
        /// </summary>
        public EnhancementDataAsset GetEnhancementDataAsset()
        {
            return _enhancementDataAsset;
        }

        /// <summary>
        /// 능력치 이름으로 강화 데이터를 가져옵니다.
        /// </summary>
        public EnhancementData GetEnhancementData(StatNames statName)
        {
            if (_enhancementDataAsset == null)
            {
                return null;
            }

            return _enhancementDataAsset.FindEnhancementData(statName);
        }

        #endregion Enhancement Get Methods

        #region Enhancement Load Methods

        // LoadEnhancementDataSync는 ScriptableDataManager.Load.cs에 정의됨

        #endregion Enhancement Load Methods

        #region Enhancement Refresh Methods

        /// <summary>
        /// 강화 시스템 데이터 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllEnhancement()
        {
            _enhancementDataAsset?.Refresh();
        }

        #endregion Enhancement Refresh Methods

        #region Enhancement Validation Methods

        /// <summary>
        /// 강화 시스템 데이터 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidEnhancementsOnLoadAssets()
        {
#if UNITY_EDITOR
            if (_enhancementDataAsset == null)
            {
                Log.Warning(LogTags.ScriptableData, "강화 시스템 데이터 에셋이 설정되지 않았습니다.");
                return;
            }

            if (_enhancementDataAsset.DataArray == null || _enhancementDataAsset.DataArray.Length == 0)
            {
                Log.Warning(LogTags.ScriptableData, "강화 시스템 데이터 배열이 비어있습니다.");
                return;
            }

            StatNames[] keys = EnumEx.GetValues<StatNames>();
            for (int i = 1; i < keys.Length; i++)
            {
                if (keys[i] == StatNames.None)
                {
                    continue;
                }

                EnhancementData data = _enhancementDataAsset.FindEnhancementData(keys[i]);
                if (data == null)
                {
                    Log.Warning(LogTags.ScriptableData, "강화 시스템 데이터가 설정되지 않았습니다. {0}({1})", keys[i], keys[i].ToLogString());
                }
            }
#endif
        }

        #endregion Enhancement Validation Methods
    }
}

