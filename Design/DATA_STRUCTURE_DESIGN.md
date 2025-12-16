# 데이터 구조 설계 문서

## 개요
현재 저장되는 데이터 구조는 이전 게임(다중 캐릭터 시스템)에서 가져온 것으로, 현재 게임(단일 플레이어 + 동료 시스템)에 맞게 재설계가 필요합니다.

## 현재 구조의 문제점

### VCharacter 구조
- **이전 게임**: 여러 캐릭터를 가질 수 있었음 (`Dictionary<string, VCharacterInfo>`)
- **현재 게임**: 플레이어는 하나만 존재
- **문제**: 불필요한 다중 캐릭터 관리 로직이 포함되어 있음

### 동료 시스템 부재
- **현재 게임 요구사항**: 동료 시스템 필요
  - 총 4명의 동료 (엘리, 지크, 미호, 루나)
  - 각 동료별 해금 상태
  - 각 동료별 레벨 (플레이어와 별개)
  - 각 동료별 강화패시브 레벨
  - 각 동료별 전직 정보 (1~7차)
  - 각 동료별 승급 옵션

## 재설계된 데이터 구조

### 1. VCharacter (플레이어 데이터)
단일 플레이어 데이터만 저장하도록 단순화

```csharp
[System.Serializable]
public partial class VCharacter
{
    // 플레이어 기본 정보
    public int PlayerLevel;  // 플레이어 레벨 (VCharacterLevel과 통합 가능)
    public int PlayerExperience;
    
    // 스킬 슬롯 해금 상태
    public int UnlockedSkillSlotCount;  // 최대 10개, 처음 1개 해금
    
    public void OnLoadGameData()
    {
        // 초기화 로직
    }
    
    public static VCharacter CreateDefault()
    {
        return new VCharacter()
        {
            PlayerLevel = 1,
            PlayerExperience = 0,
            UnlockedSkillSlotCount = 1
        };
    }
}
```

### 1-1. VCharacterEnhancement (캐릭터 강화 데이터)
골드로 구매하는 강화 능력치와 성장 지식 데이터

```csharp
[System.Serializable]
public class VCharacterEnhancement
{
    // 골드로 구매하는 강화 능력치
    public int AttackLevel;           // 공격력 MAX LV: 1,900,000
    public int HpLevel;               // 체력 MAX LV: 1,900,000
    public int HpRecoveryLevel;       // 체력 회복량 MAX LV: 1,900,000
    public int CriticalAttackLevel;   // 치명타 공격력 MAX LV: 10,000
    public int CriticalChanceLevel;  // 치명타 확률 MAX LV: 1,000
    public int CriticalHitLevel;      // 회심의 일격 MAX LV: 4,000 (성장 지식으로 확장 가능)
    public int CriticalHitChanceLevel; // 회심의 일격 확률 MAX LV: 1,000
    
    // 성장 지식 (강화 레벨 특정 수치 이상 달성 시 도전 가능)
    public int PowerKnowledgeLevel;      // 괴력 MAX LV: 75단계
    public int SteelBodyKnowledgeLevel;  // 강철의 육체 MAX LV: 75단계
    public int StrongHeartKnowledgeLevel; // 강인한 심장 MAX LV: 75단계
    public int SuperHumanPowerLevel;     // 초인의 힘 MAX LV: 75단계
    
    public void OnLoadGameData()
    {
        // 초기화 로직
    }
    
    public static VCharacterEnhancement CreateDefault()
    {
        return new VCharacterEnhancement()
        {
            AttackLevel = 0,
            HpLevel = 0,
            HpRecoveryLevel = 0,
            CriticalAttackLevel = 0,
            CriticalChanceLevel = 0,
            CriticalHitLevel = 0,
            CriticalHitChanceLevel = 0,
            PowerKnowledgeLevel = 0,
            SteelBodyKnowledgeLevel = 0,
            StrongHeartKnowledgeLevel = 0,
            SuperHumanPowerLevel = 0
        };
    }
}
```

### 1-2. VCharacterGrowth (캐릭터 성장 데이터)
레벨업 시 획득하는 SP(능력치 포인트)로 구매하는 능력치 데이터

```csharp
[System.Serializable]
public class VCharacterGrowth
{
    // 능력치 포인트 (레벨업 시 3개씩 획득, SP로 표시)
    public int AbilityPoints;  // SP
    
    // SP로 구매하는 능력치
    public int StrLevel;       // STR MAX LV: 1,000
    public int HpLevel;         // HP MAX LV: 1,000
    public int VitLevel;       // VIT MAX LV: 1,000
    public int CriLevel;       // CRI MAX LV: 200
    public int LukLevel;       // LUK MAX LV: 1,000
    public int AccLevel;       // ACC MAX LV: 200
    public int DodgeLevel;     // DODGE MAX LV: 200
    
    // 훈련 일지로 맥스 레벨 확장 가능
    // (각 능력치별 최대 레벨 확장 정보는 별도 관리)
    
    public void OnLoadGameData()
    {
        // 초기화 로직
    }
    
    public static VCharacterGrowth CreateDefault()
    {
        return new VCharacterGrowth()
        {
            AbilityPoints = 0,
            StrLevel = 0,
            HpLevel = 0,
            VitLevel = 0,
            CriLevel = 0,
            LukLevel = 0,
            AccLevel = 0,
            DodgeLevel = 0
        };
    }
}
```

### 2. VCompanion (동료 시스템)
동료 데이터를 관리하는 새로운 클래스

```csharp
[System.Serializable]
public partial class VCompanion
{
    public Dictionary<string, VCompanionInfo> Companions = new();
    
    public void OnLoadGameData()
    {
        foreach (VCompanionInfo companionInfo in Companions.Values)
        {
            companionInfo.OnLoadGameData();
        }
    }
    
    public bool IsUnlocked(CompanionNames companionName)
    {
        VCompanionInfo companionInfo = GetCompanionInfo(companionName);
        return companionInfo != null && companionInfo.IsUnlocked;
    }
    
    public VCompanionInfo GetCompanionInfo(CompanionNames companionName)
    {
        string key = companionName.ToString();
        Companions.TryGetValue(key, out VCompanionInfo companionInfo);
        return companionInfo;
    }
    
    public void Unlock(CompanionNames companionName)
    {
        VCompanionInfo companionInfo = GetOrCreateCompanionInfo(companionName);
        if (!companionInfo.IsUnlocked)
        {
            companionInfo.IsUnlocked = true;
            Log.Info(LogTags.GameData_Companion, "{0} 동료를 해금합니다.", companionName);
        }
    }
    
    public static VCompanion CreateDefault()
    {
        return new VCompanion()
        {
            Companions = new Dictionary<string, VCompanionInfo>()
        };
    }
    
    private VCompanionInfo GetOrCreateCompanionInfo(CompanionNames companionName)
    {
        string key = companionName.ToString();
        if (!Companions.TryGetValue(key, out VCompanionInfo companionInfo))
        {
            companionInfo = new VCompanionInfo(companionName);
            Companions.Add(key, companionInfo);
        }
        return companionInfo;
    }
}
```

### 3. VCompanionInfo (개별 동료 정보)
각 동료의 상세 정보를 저장

```csharp
[System.Serializable]
public class VCompanionInfo
{
    [NonSerialized]
    public CompanionNames CompanionName;
    public string CompanionNameString;
    
    // 해금 상태
    public bool IsUnlocked;
    
    // 동료 레벨 (플레이어와 별개)
    public int Level;
    public int Experience;
    
    // 강화패시브 레벨
    // 강화패시브 1
    public Dictionary<string, int> Passive1Levels = new();  // 각 패시브별 레벨 (최대 100)
    // 강화패시브 2
    public Dictionary<string, int> Passive2Levels = new();  // 각 패시브별 레벨 (최대 100)
    // 강화패시브 3
    public Dictionary<string, int> Passive3Levels = new();  // 속성 데미지 (최대 1500), 나머지 (최대 100)
    
    // 전직 정보
    public int PromotionLevel;  // 0 = 전직 안함, 1~7 = 1차~7차 전직
    
    // 승급 옵션 (전직마다 별도 저장)
    public Dictionary<int, List<VCompanionPromotionOption>> PromotionOptions = new();  // Key: 전직 레벨 (1~7)
    
    public void OnLoadGameData()
    {
        _ = EnumEx.ConvertTo(ref CompanionName, CompanionNameString);
    }
    
    public VCompanionInfo()
    {
        InitializeDefaults();
    }
    
    public VCompanionInfo(CompanionNames companionName)
    {
        CompanionName = companionName;
        CompanionNameString = companionName.ToString();
        InitializeDefaults();
    }
    
    private void InitializeDefaults()
    {
        IsUnlocked = false;
        Level = 1;
        Experience = 0;
        PromotionLevel = 0;
        Passive1Levels = new Dictionary<string, int>();
        Passive2Levels = new Dictionary<string, int>();
        Passive3Levels = new Dictionary<string, int>();
        PromotionOptions = new Dictionary<int, List<VCompanionPromotionOption>>();
    }
}
```

### 4. VCompanionPromotionOption (승급 옵션)
동료 전직 시 해금되는 승급 옵션

```csharp
[System.Serializable]
public class VCompanionPromotionOption
{
    public int OptionIndex;  // 옵션 슬롯 인덱스 (0~8, 총 9개)
    public PromotionOptionType OptionType;  // 옵션 타입
    public PromotionOptionGrade OptionGrade;  // 옵션 등급 (회색, 초록, 주황, 보라, 빨강, 민트)
    public float OptionValue;  // 옵션 수치
    public bool IsLocked;  // 잠금 상태 (잠금 해제 시 가격 증가)
    
    public VCompanionPromotionOption()
    {
        OptionIndex = 0;
        OptionType = PromotionOptionType.None;
        OptionGrade = PromotionOptionGrade.Gray;
        OptionValue = 0f;
        IsLocked = true;
    }
}
```

### 5. VCharacterAccessory (악세사리 데이터)
악세사리 장비 데이터

```csharp
[System.Serializable]
public class VCharacterAccessory
{
    public Dictionary<string, VAccessory> Accessories = new();
    public List<string> EquippedAccessories = new();  // 장착된 악세사리 목록
    
    public void OnLoadGameData()
    {
        if (Accessories.IsValid())
        {
            foreach (KeyValuePair<string, VAccessory> accessory in Accessories)
            {
                accessory.Value.OnLoadGameData();
            }
        }
    }
    
    public static VCharacterAccessory CreateDefault()
    {
        return new VCharacterAccessory()
        {
            Accessories = new Dictionary<string, VAccessory>(),
            EquippedAccessories = new List<string>()
        };
    }
}
```

### 6. VRelic (유물 데이터)
유물 데이터

```csharp
[System.Serializable]
public class VRelic
{
    public Dictionary<string, VRelicInfo> Relics = new();
    public List<string> UnlockedRelics = new();
    
    public void OnLoadGameData()
    {
        if (Relics.IsValid())
        {
            foreach (KeyValuePair<string, VRelicInfo> relic in Relics)
            {
                relic.Value.OnLoadGameData();
            }
        }
    }
    
    public static VRelic CreateDefault()
    {
        return new VRelic()
        {
            Relics = new Dictionary<string, VRelicInfo>(),
            UnlockedRelics = new List<string>()
        };
    }
}
```

### 7. VSpirit (정령 데이터)
정령 데이터

```csharp
[System.Serializable]
public class VSpirit
{
    public Dictionary<string, VSpiritInfo> Spirits = new();
    public List<string> UnlockedSpirits = new();
    
    public void OnLoadGameData()
    {
        if (Spirits.IsValid())
        {
            foreach (KeyValuePair<string, VSpiritInfo> spirit in Spirits)
            {
                spirit.Value.OnLoadGameData();
            }
        }
    }
    
    public static VSpirit CreateDefault()
    {
        return new VSpirit()
        {
            Spirits = new Dictionary<string, VSpiritInfo>(),
            UnlockedSpirits = new List<string>()
        };
    }
}
```

### 8. VProfile 업데이트
VProfile에 새로운 데이터 구조 반영

```csharp
[System.Serializable]
public partial class VProfile
{
    public int IssuedItemSID;
    public VCharacter Character;  // 단일 플레이어 데이터
    public VCharacterLevel Level;  // 플레이어 레벨 (VCharacter와 통합 검토 필요)
    
    // 캐릭터 강화/성장 데이터 (VCharacterStat 분리)
    public VCharacterEnhancement Enhancement;  // 골드로 구매하는 강화 능력치 + 성장 지식
    public VCharacterGrowth Growth;  // SP로 구매하는 성장 능력치
    
    public VCompanion Companion;  // 동료 시스템
    
    // 장비 데이터
    public VCharacterWeapon Weapon;  // 무기
    public VCharacterAccessory Accessory;  // 악세사리 (신규)
    
    // 유물/정령 데이터 (신규)
    public VRelic Relic;  // 유물
    public VSpirit Spirit;  // 정령
    
    public VCurrency Currency;
    public VCharacterStage Stage;
    public VStatistics Statistics;
    
    // 삭제된 데이터:
    // - VCharacterPotion (물약)
    // - VCharacterItem (아이템)
    // - VCharacterSlot (슬롯 - 스킬 슬롯은 VCharacter에 포함)
    
    public void OnLoadGameData()
    {
        CreateEmptyData();
        
        Character.OnLoadGameData();
        Level.OnLoadGameData();
        Enhancement.OnLoadGameData();
        Growth.OnLoadGameData();
        Companion.OnLoadGameData();
        Weapon.OnLoadGameData();
        Accessory.OnLoadGameData();
        Relic.OnLoadGameData();
        Spirit.OnLoadGameData();
        Currency.OnLoadGameData();
        Stage.OnLoadGameData();
        Statistics.OnLoadGameData();
    }
    
    public void CreateEmptyData()
    {
        Character ??= VCharacter.CreateDefault();
        Level ??= VCharacterLevel.CreateDefault();
        Enhancement ??= VCharacterEnhancement.CreateDefault();
        Growth ??= VCharacterGrowth.CreateDefault();
        Companion ??= VCompanion.CreateDefault();
        Weapon ??= VCharacterWeapon.CreateDefault();
        Accessory ??= VCharacterAccessory.CreateDefault();
        Relic ??= VRelic.CreateDefault();
        Spirit ??= VSpirit.CreateDefault();
        Currency ??= VCurrency.CreateDefault();
        Stage ??= VCharacterStage.CreateDefault();
        Statistics ??= VStatistics.CreateDefault();
    }
}
```

## 캐릭터 강화/성장 시스템 상세 설계

### 강화 시스템 (골드 구매)
골드를 사용하여 구매하는 강화 능력치입니다.

#### 기본 강화 능력치
- **공격력**: MAX LV: 1,900,000
- **체력**: MAX LV: 1,900,000
- **체력 회복량**: MAX LV: 1,900,000
- **치명타 공격력**: MAX LV: 10,000
- **치명타 확률**: MAX LV: 1,000
- **회심의 일격**: MAX LV: 4,000 (성장 지식으로 확장 가능)
- **회심의 일격 확률**: MAX LV: 1,000

#### 성장 지식
공격력/체력/체력 회복량을 특정 레벨 이상 올리면 도전 가능한 시스템입니다.
전투를 통해 획득할 수 있으며, 성장 지식을 깰 때마다 회심의 일격의 최대 레벨이 확장됩니다.

- **괴력**: MAX LV: 75단계
- **강철의 육체**: MAX LV: 75단계
- **강인한 심장**: MAX LV: 75단계
- **초인의 힘**: MAX LV: 75단계
  - 성장 지식 한 줄을 깨고 도전할 수 있는 초인의 힘 전투를 통해 추가로 확장 가능

### 성장 시스템 (SP 구매)
레벨업 시 획득하는 SP(능력치 포인트)를 투자하여 강화할 수 있는 능력치입니다.
훈련 일지를 통해 최대 레벨 확장이 가능합니다.

#### SP로 구매하는 능력치
- **STR**: MAX LV: 1,000
- **HP**: MAX LV: 1,000
- **VIT**: MAX LV: 1,000
- **CRI**: MAX LV: 200
- **LUK**: MAX LV: 1,000
- **ACC**: MAX LV: 200
- **DODGE**: MAX LV: 200

## 동료 시스템 상세 설계

### 동료 종류
1. **엘리** (Elly)
   - 클래스: 궁수
   - 속성: 바람
   - 종족: 엘프
   - 해금 조건: 시작의 숲 클리어

2. **지크** (Zeke)
   - 클래스: 전사
   - 속성: 땅
   - 종족: 인간
   - 해금 조건: 탐욕의 언덕 클리어

3. **미호** (Miho)
   - 클래스: 도적
   - 속성: 불
   - 종족: 인간
   - 해금 조건: 그림자 계곡 클리어

4. **루나** (Luna)
   - 클래스: 마법사
   - 속성: 물
   - 종족: 엘프
   - 해금 조건: 홉고블린 족장 처치

### 강화패시브 구조

각 동료별로 3개의 강화패시브 카테고리가 있으며, 각 카테고리마다 여러 패시브가 있습니다.

#### 강화패시브 1 (기본 패시브)
- 각 패시브 최대 레벨: 100
- 엘리: 집중 사격, 숲의 도움, 요정의 노래
- 지크: 투지, 광기, 검무
- 미호: 섀도우 스텝, 섀도우 댄스, 골드 러시
- 루나: 마나 도프, 마나 증폭, 전쟁의 지혜

#### 강화패시브 2 (해금 필요)
- 각 패시브 최대 레벨: 100
- 해금 조건: n-2-1, n-2-2, n-2-3 (동료 레벨 기반)
- 엘리: 바람의 노래, 요정의 찬가, 바람의 이해
- 지크: 소울 캐치, 연성의 지혜, 대지의 이해
- 미호: 붉은 탐욕, 골드러시 II, 화염의 이해
- 루나: 심해의 노래, 심연의 찬가, 바다의 이해

#### 강화패시브 3 (고급 패시브)
- 속성 데미지 패시브: 최대 레벨 1500
- 기타 패시브: 최대 레벨 100
- 해금 조건: n-3-1, n-3-2, n-3-3 (동료 레벨 기반)
- 엘리: 요정의 손길, 약점 간파, 바람의 교감
- 지크: 무질서, 봉쇄, 대지의 교감
- 미호: 캐스팅, 그림자 낙인, 화염의 교감
- 루나: 룬 마법, 마법 차단, 바다의 교감

### 전직 시스템

- 전직 레벨: 0 (전직 안함) ~ 7 (7차 전직)
- 전직 조건: 전직 대상과 1대1 전투 승리
- 전직 보상: 외형 변경, 다이아몬드, 에메랄드 지급
- 전직 제약:
  - 1~2차: 대상과 같은 속성 스킬 사용 불가
  - 3~4차: 같은 속성 스킬 사용 시 대상 체력 증가 디버프
  - 5~6차: 같은 속성 스킬 사용 시 체력 증가 + 전투 시간 감소 디버프
  - 7차: 같은 속성 스킬 사용 시 체력 증가 + 전투 시간 감소 + 최대 마나 감소 디버프

### 승급 옵션 시스템

- 전직마다 승급 옵션 해금
- 각 전직의 7번째 승급 후 다음 전직 해금
- 옵션 개수: 9개 (0~8 인덱스)
- 옵션 등급 확률:
  - 회색: 23%
  - 초록: 20%
  - 주황: 30%
  - 보라: 20%
  - 빨강: 5%
  - 민트: 2%
- 옵션 타입별 수치 범위는 기획 문서 참조
- 잠금 해제 시 변경 가격 증가
- 3차 전직 오픈 시 4만 다이스로 일괄 변경 기능 추가

## 데이터 구조 변경 사항

### 삭제된 데이터
1. **VCharacterPotion** (물약 데이터) - 삭제
2. **VCharacterItem** (아이템 데이터) - 삭제
3. **VCharacterSlot** (슬롯 데이터) - 삭제 (스킬 슬롯은 VCharacter에 포함)

### 추가된 데이터
1. **VCharacterAccessory** (악세사리 데이터) - 신규
2. **VRelic** (유물 데이터) - 신규
3. **VSpirit** (정령 데이터) - 신규

### 분리된 데이터
1. **VCharacterStat** → **VCharacterEnhancement** + **VCharacterGrowth**로 분리
   - **VCharacterEnhancement**: 골드로 구매하는 강화 능력치 + 성장 지식
   - **VCharacterGrowth**: SP(능력치 포인트)로 구매하는 성장 능력치

## 마이그레이션 계획

### 기존 데이터 호환성
1. 기존 `VCharacter.UnlockedCharacters` 데이터를 플레이어 데이터로 변환
2. 기존 `VCharacterInfo`의 Rank, RankExperience 등을 플레이어 레벨로 변환
3. 기존 `VCharacterStat` 데이터를 `VCharacterEnhancement`와 `VCharacterGrowth`로 분리
4. 동료 데이터는 새로 생성 (기본값)
5. 악세사리, 유물, 정령 데이터는 새로 생성 (기본값)

### 마이그레이션 코드 예시
```csharp
public void MigrateFromOldCharacterSystem(VCharacter oldCharacter, VCharacterStat oldStat)
{
    // 기존 캐릭터 데이터가 있으면 플레이어 데이터로 변환
    if (oldCharacter.UnlockedCharacters.Count > 0)
    {
        // 첫 번째 캐릭터의 랭크를 플레이어 레벨로 변환
        var firstChar = oldCharacter.UnlockedCharacters.Values.First();
        Character.PlayerLevel = firstChar.Rank;
        Character.PlayerExperience = firstChar.RankExperience;
    }
    
    // 기존 Stat 데이터를 Enhancement와 Growth로 분리
    // (기존 Stat 구조에 따라 적절히 분리)
    
    // 동료 데이터는 기본값으로 초기화
    Companion = VCompanion.CreateDefault();
    
    // 신규 데이터 초기화
    Accessory = VCharacterAccessory.CreateDefault();
    Relic = VRelic.CreateDefault();
    Spirit = VSpirit.CreateDefault();
}
```

## 구현 우선순위

1. **1단계**: VCharacter 단순화 (다중 캐릭터 제거)
2. **2단계**: VCompanion 기본 구조 구현 (해금, 레벨)
3. **3단계**: 강화패시브 시스템 구현
4. **4단계**: 전직 시스템 구현
5. **5단계**: 승급 옵션 시스템 구현
6. **6단계**: 마이그레이션 코드 작성 및 테스트

## 참고사항

- 동료 레벨은 플레이어 레벨과 별개로 관리
- 각 동료의 강화패시브는 Dictionary로 관리하여 확장성 확보
- 전직 정보는 전직 레벨로만 저장하고, 상세 정보는 데이터 테이블에서 조회
- 승급 옵션은 전직 레벨별로 별도 저장하여 전직마다 독립적인 옵션 관리

