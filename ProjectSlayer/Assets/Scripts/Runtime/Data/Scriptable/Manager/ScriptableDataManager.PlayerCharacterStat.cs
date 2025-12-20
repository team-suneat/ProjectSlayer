namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 플레이어 캐릭터 능력치 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region PlayerCharacterStat Get Methods

        /// <summary>
        /// 플레이어 캐릭터 능력치 에셋을 가져옵니다.
        /// </summary>
        public PlayerCharacterStatAsset GetPlayerCharacterStatAsset()
        {
            return _playerCharacterStatAsset;
        }

        #endregion PlayerCharacterStat Get Methods

        #region PlayerCharacterStat Load Methods

        // LoadPlayerCharacterStatSync는 ScriptableDataManager.Load.cs에 정의됨

        #endregion PlayerCharacterStat Load Methods

        #region PlayerCharacterStat Refresh Methods

        /// <summary>
        /// 플레이어 캐릭터 능력치 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllPlayerCharacterStat()
        {
            _playerCharacterStatAsset?.Refresh();
        }

        #endregion PlayerCharacterStat Refresh Methods

        #region PlayerCharacterStat Validation Methods

        /// <summary>
        /// 플레이어 캐릭터 능력치 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidPlayerCharacterStatOnLoadAssets()
        {
#if UNITY_EDITOR
            if (_playerCharacterStatAsset == null)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 능력치 에셋이 설정되지 않았습니다.");
                return;
            }

            if (_playerCharacterStatAsset.BaseHealth <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 체력이 0 이하입니다.");
            }

            if (_playerCharacterStatAsset.BaseAttack <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 공격력이 0 이하입니다.");
            }

            if (_playerCharacterStatAsset.BaseHealthRegen < 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 체력 회복량이 음수입니다.");
            }

            if (_playerCharacterStatAsset.BaseMana <= 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 마나가 0 이하입니다.");
            }

            if (_playerCharacterStatAsset.BaseManaRegen < 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 마나 회복량이 음수입니다.");
            }

            if (_playerCharacterStatAsset.BaseCriticalChance < 0 || _playerCharacterStatAsset.BaseCriticalChance > 1.0f)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 치명타 확률이 0~1 범위를 벗어났습니다.");
            }

            if (_playerCharacterStatAsset.BaseCriticalDamage < 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 치명타 피해가 음수입니다.");
            }

            if (_playerCharacterStatAsset.BaseAccuracyChance < 0 || _playerCharacterStatAsset.BaseAccuracyChance > 1.0f)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 명중률이 0~1 범위를 벗어났습니다.");
            }

            if (_playerCharacterStatAsset.BaseGoldGain < 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 골드 획득량 배율이 음수입니다.");
            }

            if (_playerCharacterStatAsset.BaseXPGain < 0)
            {
                Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 스탯의 기본 경험치 획득량 배율이 음수입니다.");
            }
#endif
        }

        #endregion PlayerCharacterStat Validation Methods
    }
}

