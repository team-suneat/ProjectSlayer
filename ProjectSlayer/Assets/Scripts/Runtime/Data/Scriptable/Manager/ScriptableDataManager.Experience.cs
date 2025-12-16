namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 경험치 설정 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region ExperienceConfig Get Methods

        /// <summary>
        /// 경험치 설정 에셋을 가져옵니다.
        /// </summary>
        public ExperienceConfigAsset GetExperienceConfigAsset()
        {
            return _experienceConfigAsset;
        }

        #endregion ExperienceConfig Get Methods

        #region ExperienceConfig Load Methods

        // LoadExperienceConfigSync는 ScriptableDataManager.Load.cs에 정의됨

        #endregion ExperienceConfig Load Methods

        #region ExperienceConfig Refresh Methods

        /// <summary>
        /// 경험치 설정 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllExperienceConfig()
        {
            _experienceConfigAsset?.Refresh();
        }

        #endregion ExperienceConfig Refresh Methods

        #region ExperienceConfig Validation Methods

        /// <summary>
        /// 경험치 설정 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidExperienceConfigOnLoadAssets()
        {
#if UNITY_EDITOR
            if (_experienceConfigAsset == null)
            {
                Log.Warning(LogTags.ScriptableData, "경험치 설정 에셋이 설정되지 않았습니다.");
                return;
            }

            if (_experienceConfigAsset.InitialExperienceRequired <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "경험치 설정의 초기 경험치 필요량이 0 이하입니다.");
            }

            if (_experienceConfigAsset.ExperienceGrowthRate <= 1.0f)
            {
                Log.Warning(LogTags.ScriptableData, "경험치 설정의 증가 배율이 1.0 이하입니다.");
            }

            if (_experienceConfigAsset.StatPointPerLevel <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "경험치 설정의 레벨업 시 능력치 포인트가 0 이하입니다.");
            }
#endif
        }

        #endregion ExperienceConfig Validation Methods
    }
}

