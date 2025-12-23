namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 몬스터 경험치 드랍 설정 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Get Methods

        /// <summary>
        /// 몬스터 경험치 드랍 설정 에셋을 가져옵니다.
        /// </summary>
        public MonsterExperienceDropConfigAsset GetMonsterExperienceDropConfigAsset()
        {
            return _monsterExperienceDropConfigAsset;
        }

        #endregion Get Methods

        #region Refresh Methods

        /// <summary>
        /// 몬스터 경험치 드랍 설정 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllMonsterExperienceDropConfig()
        {
            _monsterExperienceDropConfigAsset?.Refresh();
        }

        #endregion Refresh Methods

        #region Validation Methods

        /// <summary>
        /// 몬스터 경험치 드랍 설정 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidMonsterExperienceDropConfigOnLoadAssets()
        {
#if UNITY_EDITOR
            if (_monsterExperienceDropConfigAsset == null)
            {
                Log.Warning(LogTags.ScriptableData, "몬스터 경험치 드랍 설정 에셋이 설정되지 않았습니다.");
                return;
            }

            if (_monsterExperienceDropConfigAsset.BaseExp <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "몬스터 경험치 드랍 설정의 일반 몬스터 최대 경험치가 0 이하입니다.");
            }

            if (_monsterExperienceDropConfigAsset.ExpGrowthRate <= 1.0f)
            {
                Log.Warning(LogTags.ScriptableData, "몬스터 경험치 드랍 설정의 일반 몬스터 증가 배율이 1.0 이하입니다.");
            }

#endif
        }
        // 137
        #endregion Validation Methods
    }
}