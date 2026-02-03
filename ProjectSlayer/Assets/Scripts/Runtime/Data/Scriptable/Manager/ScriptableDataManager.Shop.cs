namespace TeamSuneat.Data
{
    public partial class ScriptableDataManager
    {
        #region 악세사리 소환 (Accessory Summon)

        public AccessorySummonConfigAsset GetAccessorySummonConfigAsset()
        {
            return _accessorySummonConfigAsset;
        }

        public void RefreshAccessorySummonConfig()
        {
            _accessorySummonConfigAsset?.Refresh();
        }

        #endregion 악세사리 소환 (Accessory Summon)

        #region 스킬 카드 소환 (SkillCard Summon)

        public SkillCardSummonConfigAsset GetSkillCardSummonConfigAsset()
        {
            return _skillCardSummonConfigAsset;
        }

        public void RefreshSkillCardSummonConfig()
        {
            _skillCardSummonConfigAsset?.Refresh();
        }

        #endregion 스킬 카드 소환 (SkillCard Summon)

        #region 무기 소환 (Weapon Summon)

        public WeaponSummonConfigAsset GetWeaponSummonConfigAsset()
        {
            return _weaponSummonConfigAsset;
        }

        public void RefreshWeaponSummonConfig()
        {
            _weaponSummonConfigAsset?.Refresh();
        }

        #endregion 무기 소환 (Weapon Summon)

        #region 소환 레벨 설정 (Summon Level Config)

        public SummonLevelConfigAsset GetSummonLevelConfigAsset()
        {
            return _summonLevelConfigAsset;
        }

        public void RefreshSummonLevelConfig()
        {
            _summonLevelConfigAsset?.Refresh();
        }

        #endregion 소환 레벨 설정 (Summon Level Config)
    }
}
