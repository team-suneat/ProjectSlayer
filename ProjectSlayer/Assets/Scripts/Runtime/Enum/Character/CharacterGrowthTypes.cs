namespace TeamSuneat
{
    public enum CharacterGrowthTypes
    {
        None,
        Strength,
        HealthPoint,
        Vitality,
        Critical,
        Luck,
        AccuracyStat,
        Dodge,
    }

    public static class GrowthTypeConverter
    {
        public static StatNames ConvertToStatName(this CharacterGrowthTypes growthType)
        {
            return growthType switch
            {
                CharacterGrowthTypes.Strength => StatNames.Attack,
                CharacterGrowthTypes.HealthPoint => StatNames.Health,
                CharacterGrowthTypes.Vitality => StatNames.HealthRegen,
                CharacterGrowthTypes.Critical => StatNames.CriticalDamage,
                CharacterGrowthTypes.Luck => StatNames.GoldGain,
                CharacterGrowthTypes.AccuracyStat => StatNames.AccuracyChance,
                CharacterGrowthTypes.Dodge => StatNames.DodgeChance,
                _ => StatNames.None
            };
        }
    }
}