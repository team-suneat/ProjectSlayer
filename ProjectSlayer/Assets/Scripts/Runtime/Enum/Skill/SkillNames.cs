namespace TeamSuneat
{
    public enum SkillNames
    {
        None,

        // 일반
        FlameSlash = 1101,        // 불꽃 베기
        IceStone,                // 아이스 스톤
        LightningSlash,          // 번개 베기
        StoneStrike,             // 스톤 스트라이크

        FireSword,               // 불의 검
        ManaBlessing,            // 마나의 축복
        ThunderStrike,           // 뇌격
        EarthBlessing,           // 대지의 축복

        // 고급
        HeatWave = 1201,         // 열풍
        IceShower,               // 아이스 샤워
        HighSpeedMovement,       // 고속이동
        PowerStrike,             // 파워 스트라이크

        // 레어
        FireSlash = 1301,         // 화염 베기
        WaveSlash,               // 파도베기
        ThunderSlash,            // 천둥 베기
        PowerImpact,             // 파워 임팩트

        BurningSword,            // 타오르는 검
        FlowingBlade,            // 흐르는 칼날
        AccelerationSword,       // 가속의 검
        EarthWill,               // 땅의 의지

        // 영웅
        FireWave = 1401,         // 화염 파동
        WindingBlade,            // 굽이치는 칼날
        LightningFast,           // 전광석화
        SteelWill,               // 강철의 의지

        HellfireSlash,           // 연옥 화염 베기
        DancingWave,             // 춤추는 파도
        WindSword,               // 바람의 검
        LifeMana,                // 라이프 마나

        // 전설
        TrueHeatWave = 1501,     // 진 열풍
        IceTime,                 // 아이스 타임
        AsuraLightningSlash,     // 수라 번개 베기
        GigaStrike,              // 기가 스트라이크

        Rage,                    // 분노
        Meditation,              // 메디테이션
        RedThunder,              // 적뢰
        GigaImpact,              // 기가 임팩트

        // 신화
        FirePillar = 1601,       // 불기둥
        Blizzard,                // 블리자드
        Swiftness,               // 신속
        BeastHunt,               // 마수 사냥

        WarriorBurn,             // 워리어번
        Torrent,                 // 격류
        ThunderGod,              // 뇌신
        SuperhumanStrength,      // 괴력난신

        // 무속성
        Rave,                    // 레이브
        Mantra,                  // 만트라
    }

    public static class SkillNameHelper
    {
        public static SkillNames PickSkillName(GradeNames gradeName)
        {
            int randomValue = 0;

            switch (gradeName)
            {
                case GradeNames.Uncommon:
                    randomValue = RandomEx.Range(0, 4);
                    break;

                default:
                    randomValue = RandomEx.Range(0, 8);
                    break;
            }

            int skillTID = 1000 + (int)gradeName * 100 + randomValue;
            return skillTID.ToEnum<SkillNames>();
        }
    }
}