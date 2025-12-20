namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 몬스터 능력치 설정 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region MonsterStatConfig Get Methods

        /// <summary>
        /// 몬스터 능력치 설정 에셋을 가져옵니다.
        /// </summary>
        public MonsterStatConfigAsset GetMonsterStatConfigAsset()
        {
            return _monsterStatConfigAsset;
        }

        #endregion MonsterStatConfig Get Methods

        #region MonsterStatConfig Load Methods

        // LoadMonsterStatConfigSync는 ScriptableDataManager.Load.cs에 정의됨

        #endregion MonsterStatConfig Load Methods

        #region MonsterStatConfig Refresh Methods

        /// <summary>
        /// 몬스터 능력치 설정 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllMonsterStatConfig()
        {
            _monsterStatConfigAsset?.Refresh();
        }

        #endregion MonsterStatConfig Refresh Methods

        #region MonsterStatConfig Validation Methods

        /// <summary>
        /// 몬스터 능력치 설정 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidMonsterStatConfigOnLoadAssets()
        {
#if UNITY_EDITOR
            if (_monsterStatConfigAsset == null)
            {
                Log.Warning(LogTags.ScriptableData, "몬스터 능력치 설정 에셋이 설정되지 않았습니다.");
                return;
            }

            if (_monsterStatConfigAsset.BaseHealth <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "몬스터 능력치 설정의 기본 체력이 0 이하입니다.");
            }

            if (_monsterStatConfigAsset.BaseAttack <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "몬스터 능력치 설정의 기본 공격력이 0 이하입니다.");
            }

            if (_monsterStatConfigAsset.NormalStageHealthGrowthRate <= 1.0f)
            {
                Log.Warning(LogTags.ScriptableData, "몬스터 능력치 설정의 일반 스테이지 체력 증가 배율이 1.0 이하입니다.");
            }

            if (_monsterStatConfigAsset.NormalStageAttackGrowthRate <= 1.0f)
            {
                Log.Warning(LogTags.ScriptableData, "몬스터 능력치 설정의 일반 스테이지 공격력 증가 배율이 1.0 이하입니다.");
            }

            if (_monsterStatConfigAsset.BossStageHealthGrowthRate <= 1.0f)
            {
                Log.Warning(LogTags.ScriptableData, "몬스터 능력치 설정의 보스 스테이지 체력 증가 배율이 1.0 이하입니다.");
            }

            if (_monsterStatConfigAsset.BossStageAttackGrowthRate <= 1.0f)
            {
                Log.Warning(LogTags.ScriptableData, "몬스터 능력치 설정의 보스 스테이지 공격력 증가 배율이 1.0 이하입니다.");
            }
#endif
        }

        #endregion MonsterStatConfig Validation Methods
    }
}
