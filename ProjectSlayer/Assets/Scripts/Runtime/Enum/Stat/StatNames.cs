namespace TeamSuneat
{
    public enum StatNames
    {
        None, // 없음

        // 기본 스탯
        Attack,                  // 총 공격력 (기본 공격력 + 장비 공격력 + 강화 공격력)
        Health,                  // 최대 체력
        HealthRegen,             // 체력 회복량
        CriticalChance,          // 치명타 확률(%)
        CriticalDamage,          // 치명타 피해(%)
        Mana,                    // 마나
        ManaRegen,               // 마나 회복량
        Accuracy,                // 명중
        Evasion,                 // 회피
        GoldGain,                // 추가 골드 획득량
        XPGain,                  // 추가 경험치
        DamageReduction,         // 피해 감소(%)

        // 강화 시스템 스탯
        DevastatingStrike,       // 회심의 일격(%)
        DevastatingStrikeChance, // 회심의 일격 확률(%)

        // 성장 시스템 능력치
        Strength,                // Strength (공격력에 영향)
        HealthPoint,             // Health Point (체력에 영향)
        Vitality,                // Vitality (체력 회복량에 영향)
        Critical,                // Critical (치명타 확률 및 치명타 피해에 영향)
        Luck,                    // Luck (추가 골드 획득량, 추가 경험치 등에 영향)
        AccuracyStat,            // Accuracy (명중률에 영향)
        Dodge,                   // Dodge (회피율에 영향)

        // 보호막 관련 스탯
        Shield,                    // 보호막
        ShieldMulti,               // 보호막 배율
    }
}