namespace TeamSuneat
{
    // ========================================
    // 인덱스 규칙
    // ========================================
    // 플레이어: 1~99
    // 몬스터: 1000 + (지역번호-1) × 100
    //   - 일반 몬스터: +01 ~ +07
    //   - 보스 몬스터: +51 ~ +55
    // 예) Area01: 1000~1099, Area02: 1100~1199, ... Area41: 5000~5099
    // ========================================

    public enum CharacterNames
    {
        None,

        #region Player

        PlayerCharacter = 1,

        #endregion Player

        #region Area01

        Area01_Mushroom = 1001,           // 버섯
        Area01_KingMushroom = 1002,       // 왕버섯
        Area01_Goblin = 1003,             // 고블린
        Area01_StrongGoblin = 1004,       // 강화 고블린
        Area01_Slime = 1005,              // 슬라임
        Area01_Boar = 1006,               // 멧돼지
        Area01_Wolf = 1007,               // 늑대

        // 보스 몬스터 (5종)
        Area01_Boss_MushroomKing = 1051,  // 버섯 대왕 (1~5 스테이지)
        Area01_Boss_SlimeKing = 1052,     // 슬라임 킹 (6~10 스테이지)
        Area01_Boss_GoblinChief = 1053,   // 고블린 대장 (11~15 스테이지)
        Area01_Boss_BoarLord = 1054,      // 멧돼지 군주 (16~19 스테이지)
        Area01_Boss_WolfKing = 1055,      // 늑대 왕 (20 스테이지)

        #endregion Area01
    }
}