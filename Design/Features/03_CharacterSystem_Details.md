# 캐릭터 시스템 상세 정보

> 이 문서는 캐릭터 시스템의 상세 정보를 담고 있습니다.
> 구현 기능은 `03_CharacterSystem.md`를 참고하세요.

## 1. 스탯 상세 정보

### 1.1 스탯 종류 및 설명

**공격력**
- 기본 공격 데미지에 영향을 주는 스탯
- 강화 시스템과 성장 시스템을 통해 증가

**체력**
- 캐릭터의 최대 생명력
- 강화 시스템과 성장 시스템을 통해 증가

**체력회복**
- 초당 또는 시간당 체력 회복량
- 강화 시스템을 통해 증가

**치명타 확률**
- 치명타가 발생할 확률 (%)
- 강화 시스템을 통해 증가

**치명타 피해**
- 치명타 발생 시 추가 데미지
- 강화 시스템을 통해 증가

**마나**
- 스킬 사용에 필요한 자원
- 기본값 및 최대값 존재

**마나회복**
- 초당 또는 시간당 마나 회복량
- 패시브 스킬 등을 통해 증가

**명중**
- 공격이 적중할 확률에 영향을 주는 스탯
- 성장 시스템을 통해 증가

**회피**
- 적의 공격을 회피할 확률에 영향을 주는 스탯
- 성장 시스템을 통해 증가

**추가 골드 획득량**
- 적 처치 시 획득하는 골드에 추가 보너스를 주는 스탯
- 장비나 스킬 등을 통해 증가

**추가 경험치**
- 적 처치 시 획득하는 경험치에 추가 보너스를 주는 스탯
- 장비나 스킬 등을 통해 증가

## 2. 강화 시스템 상세 정보

### 2.1 강화 능력치 상세

**공격력**
- MAX LV: 1,900,000
- 골드로 구매하여 레벨업
- 레벨당 공격력 증가량 적용

**체력**
- MAX LV: 1,900,000
- 골드로 구매하여 레벨업
- 레벨당 체력 증가량 적용

**체력 회복량**
- MAX LV: 1,900,000
- 골드로 구매하여 레벨업
- 레벨당 체력 회복량 증가량 적용

**치명타 공격력**
- MAX LV: 10,000
- 골드로 구매하여 레벨업
- 레벨당 치명타 피해 증가량 적용

**치명타 확률**
- MAX LV: 1,000
- 골드로 구매하여 레벨업
- 레벨당 치명타 확률 증가량 적용

**회심의 일격**
- MAX LV: 4,000 (성장 지식에 따라 확장 가능)
- 골드로 구매하여 레벨업
- 레벨당 회심의 일격 피해 증가량 적용
- 성장 지식 한 줄 완료 시 최대 레벨 확장
- 초인의 힘 전투를 통한 추가 확장

**회심의 일격 확률**
- MAX LV: 1,000
- 골드로 구매하여 레벨업
- 레벨당 회심의 일격 확률 증가량 적용

### 2.2 업그레이드 비용 계산 공식
- (상세 계산 공식은 게임 밸런스에 따라 결정)
- 레벨이 올라갈수록 비용이 증가하는 구조
- EnhancementData에 InitialCost, CostGrowthRate 필드로 관리
- 비용 계산: InitialCost × CostGrowthRate^(레벨-1)

### 2.3 강화 시스템 데이터 구조
- **EnhancementDataAsset**: 단일 ScriptableObject로 모든 강화 능력치 데이터 관리
  - XScriptableObject를 상속받아 ScriptableDataManager에서 관리
  - DataArray: EnhancementData 배열로 모든 강화 능력치 데이터 포함
  - FindEnhancementData(StatNames): 스탯 이름으로 데이터 검색
  - Editor 기능: "모든 강화 능력치 데이터 자동 생성" 버튼 제공
- **EnhancementData**: 개별 강화 능력치 데이터
  - StatName: 능력치 종류
  - MaxLevel: 최대 레벨
  - InitialValue: 능력치 초기값
  - GrowthValue: 레벨당 능력치 증가량
  - InitialCost: 초기 비용
  - CostGrowthRate: 비용 성장률
  - RequiredStatName: 요구 능력치 (None이면 요구사항 없음)
  - RequiredStatLevel: 요구 능력치 레벨 (0이면 요구사항 없음)
  - HasRequirement: 요구사항이 설정되어 있는지 확인하는 프로퍼티

## 3. 성장 지식 시스템 상세 정보

### 3.1 성장 지식 종류 상세

**괴력**
- MAX LV: 75단계
- 공격력 관련 성장 지식
- 도전 조건: 공격력 특정 레벨 이상 달성 시 도전 가능

**강철의 육체**
- MAX LV: 75단계
- 체력 관련 성장 지식
- 도전 조건: 체력 특정 레벨 이상 달성 시 도전 가능

**강인한 심장**
- MAX LV: 75단계
- 체력 회복량 관련 성장 지식
- 도전 조건: 체력 회복량 특정 레벨 이상 달성 시 도전 가능

**초인의 힘**
- MAX LV: 75단계
- 회심의 일격 최대 레벨 확장 관련 성장 지식
- 전투를 통한 추가 확장 가능

### 3.2 성장 지식 도전 조건
- 각 성장 지식마다 특정 강화 능력치 레벨 이상 달성 시 도전 가능
- 도전 성공 시 다음 단계로 진행

### 3.3 회심의 일격 최대 레벨 확장 규칙
- 성장 지식 한 줄 완료 시 회심의 일격 최대 레벨 확장
- 초인의 힘 전투를 통한 추가 확장

## 4. 성장 시스템 상세 정보

### 4.1 능력치 상세

**STR (Strength)**
- MAX LV: 1,000
- 능력치 포인트로 구매
- 공격력에 영향을 주는 능력치

**HP (Health Point)**
- MAX LV: 1,000
- 능력치 포인트로 구매
- 체력에 영향을 주는 능력치

**VIT (Vitality)**
- MAX LV: 1,000
- 능력치 포인트로 구매
- 체력 회복량에 영향을 주는 능력치

**CRI (Critical)**
- MAX LV: 200
- 능력치 포인트로 구매
- 치명타 확률 및 치명타 피해에 영향을 주는 능력치

**LUK (Luck)**
- MAX LV: 1,000
- 능력치 포인트로 구매
- 추가 골드 획득량, 추가 경험치 등에 영향을 주는 능력치

**ACC (Accuracy)**
- MAX LV: 200
- 능력치 포인트로 구매
- 명중률에 영향을 주는 능력치

**DODGE**
- MAX LV: 200
- 능력치 포인트로 구매
- 회피율에 영향을 주는 능력치

### 4.2 능력치 포인트 획득
- 레벨업 시 능력치 포인트 3개 획득
- 능력치 포인트는 능력치 레벨업에 사용
- 레벨업 비용: 항상 능력치 포인트 1개 고정 (레벨에 관계없이 동일)

### 4.2.1 CharacterGrowthTypes Enum
- 성장 시스템에서 사용하는 성장 타입 enum
- 값: None, Strength, HealthPoint, Vitality, Critical, Luck, Accuracy, Dodge
- GrowthTypeConverter: CharacterGrowthTypes를 StatNames로 변환하는 확장 메서드 제공
  - Strength → Attack
  - HealthPoint → Health
  - Vitality → HealthRegen
  - Critical → CriticalDamage
  - Luck → GoldGain
  - Accuracy → Accuracy
  - Dodge → Dodge

### 4.3 성장 시스템 데이터 구조
- **GrowthConfigAsset**: 단일 ScriptableObject로 모든 성장 능력치 데이터 관리
  - XScriptableObject를 상속받아 ScriptableDataManager에서 관리
  - DataArray: GrowthConfigData 배열로 모든 성장 능력치 데이터 포함
  - FindGrowthData(CharacterGrowthTypes): 성장 타입으로 데이터 검색
  - Editor 기능: "모든 성장 능력치 데이터 자동 생성" 버튼 제공
  - 성장 시스템 능력치만 자동 생성 (Strength, HealthPoint, Vitality, Critical, Luck, Accuracy, Dodge)
- **GrowthConfigData**: 개별 성장 능력치 데이터
  - GrowthType: 성장 타입 (CharacterGrowthTypes enum)
  - StatName: 능력치 종류 (StatNames enum)
  - MaxLevel: 최대 레벨
  - StatIncreasePerLevel: 레벨당 스탯 증가량
  - CalculateStatValue(int level): 레벨에 따른 능력치 값 계산 (레벨 × StatIncreasePerLevel)
  - 비용: 레벨업 비용은 항상 능력치 포인트 1개로 고정 (레벨에 관계없이 동일)

## 5. 계산 공식 상세

### 5.1 스탯 증가 계산 공식
- **기본 계산 공식**: 스탯 값 = 초기값 + (레벨 × 성장값)
- 고정 데이터에서 각 스탯의 초기값과 레벨별 성장값을 불러와 계산
- 세이브 데이터에는 스탯 값이 아닌 레벨만 저장
- 최종 스탯 = 기본 스탯(레벨 기반 계산) + 강화 시스템 보너스 + 성장 시스템 보너스

### 5.2 경험치 필요량 계산 공식

**레벨별 필요 경험치**
- 레벨 1 필요 경험치: 120
- 레벨 n 필요 경험치: 120 × 1.01^(n-1)
- 각 레벨마다 이전 레벨 대비 1.01배씩 증가

**레벨 n에 도달하기 위한 총 경험치**
- 총 경험치 = 120 × (1.01^n - 1) / (1.01 - 1)
- 총 경험치 = 120 × (1.01^n - 1) / 0.01
- 총 경험치 = 12000 × (1.01^n - 1)

**검증 데이터**
- 레벨 30 총 경험치: 14,016
- 레벨 31 총 경험치: 14,156
- 레벨 32 총 경험치: 14,298
- 레벨 33 총 경험치: 14,441
- 레벨 69 총 경험치: 40,959
- 레벨 801 총 경험치: 34,869,958,568

### 5.2.1 경험치 설정 데이터 구조
- **ExperienceConfigAsset**: 경험치 필요량 계산 설정을 관리하는 ScriptableObject
  - XScriptableObject를 상속받아 ScriptableDataManager에서 관리
  - InitialExperienceRequired: 초기 경험치 필요량 (120)
  - ExperienceGrowthRate: 경험치 증가 배율 (1.01)
  - StatPointPerLevel: 레벨업 시 획득하는 능력치 포인트 (3)
  - GetTotalExperienceRequired(int level): 레벨 n에 도달하기 위한 총 경험치 계산
  - GetExperienceRequiredForNextLevel(int level): 다음 레벨로 올라가기 위해 필요한 경험치 계산
  - Editor 기능: "검증 데이터 출력" 버튼으로 계산 공식 검증

### 5.3 능력치 효과 적용 계산 공식
- (상세 계산 공식은 게임 밸런스에 따라 결정)
- 각 능력치 레벨에 따른 스탯 증가량 적용

### 5.4 캐릭터 실제 피해량 계산식

**1단계: 스탯 합산**
- 총 공격력 = 기본 공격력 + 장비 공격력 + 강화 공격력
- 기본 공격력: 캐릭터 레벨 기반 계산된 공격력
- 장비 공격력: 장착한 장비의 공격력 합계
- 강화 공격력: 강화 시스템을 통해 증가한 공격력

**2단계: 치명타 계산**
- 최종 치명타 데미지 = 총 공격력 × (1 + 치명타 공격력 % × 치명타 확률 %)
- 치명타 공격력 %: 치명타 피해 증가량 (%)
- 치명타 확률 %: 치명타 발생 확률 (%)

**3단계: 총 데미지 계산**
- 총 데미지 = (평타/스킬 계수) × 총 공격력 × (1 + (치명타 데미지 - 평타 데미지) × 치명타 확률) + 회심의 일격 보너스
- 평타/스킬 계수: 공격 타입에 따른 데미지 배율
- 치명타 데미지: 치명타 발생 시 데미지
- 평타 데미지: 일반 공격 데미지
- 회심의 일격 보너스: 회심의 일격 발생 시 추가 데미지

### 5.5 치명타 계산 공식
- 치명타 발생: 치명타 확률에 따른 랜덤 판정
- 치명타 피해: 기본 데미지 × (1 + 치명타 피해%)

### 5.6 회심의 일격 계산 공식
- 회심의 일격 발생: 회심의 일격 확률에 따른 랜덤 판정
- 회심의 일격 피해: 기본 데미지 × (1 + 회심의 일격 피해%)

### 5.7 명중 및 회피 계산 공식
- 명중 판정: (명중률 - 적 회피율)에 따른 판정
- 회피 판정: (회피율 - 적 명중률)에 따른 판정

## 6. 데이터 저장 구조

### 6.1 저장 데이터 클래스

**VCharacterLevel**
- Level: 캐릭터 레벨
- Experience: 현재 경험치
- ResetValues(): 레벨과 경험치 초기화
- AddExperience(int): 경험치 추가 및 레벨업 처리
- LevelUp(): 레벨 증가
- CanLevelUp(): 레벨업 가능 여부 확인
- CanLevelUpOrNotify(): 레벨업 가능 여부 확인 및 부족 시 알림 표시

**VCharacterEnhancement**
- EnhancementLevels: 강화 능력치별 레벨 저장 (Dictionary<string, int>)
- GetLevel(StatNames): 특정 강화 능력치의 레벨 가져오기
- SetLevel(StatNames, int): 특정 강화 능력치의 레벨 설정
- AddLevel(StatNames, int): 특정 강화 능력치의 레벨 증가
- ClearIngameData(): 인게임 데이터 초기화

**VCharacterGrowth**
- StatPoint: 능력치 포인트
- GrowthLevels: 성장 능력치별 레벨 저장 (Dictionary<string, int>)
- GetLevel(CharacterGrowthTypes): 특정 성장 능력치의 레벨 가져오기
- SetLevel(CharacterGrowthTypes, int): 특정 성장 능력치의 레벨 설정 (private)
- AddLevel(CharacterGrowthTypes, int): 특정 성장 능력치의 레벨 증가
- AddStatPoint(int): 능력치 포인트 추가
- ConsumeStatPoint(int): 능력치 포인트 소비
- CanConsumeStatPoint(int): 능력치 포인트 소비 가능 여부 확인
- CanConsumeStatPointOrNotify(int): 능력치 포인트 소비 가능 여부 확인 및 부족 시 알림 표시
- GetTotalConsumedStatPoints(): 소비한 총 능력치 포인트 계산
- ResetGrowthLevels(): 성장 레벨 초기화 및 반환될 능력치 포인트 반환
- ClearIngameData(): 인게임 데이터 초기화

### 6.2 데이터 안정성
- Dictionary 키를 string으로 저장하여 enum 순서 변경에 영향받지 않도록 구현
- VCharacterEnhancement: StatNames enum을 ToString()으로 변환하여 저장
- VCharacterGrowth: CharacterGrowthTypes enum을 ToString()으로 변환하여 저장
- VCharacterEnhancement의 메서드는 StatNames를 받아 내부에서 string으로 변환
- VCharacterGrowth의 메서드는 CharacterGrowthTypes를 받아 내부에서 string으로 변환

### 6.3 ScriptableObject 관리
- **ScriptableDataManager**: 모든 ScriptableObject 에셋을 중앙에서 관리
  - EnhancementDataAsset: 단일 에셋으로 강화 시스템 데이터 관리
  - GrowthConfigAsset: 단일 에셋으로 성장 시스템 데이터 관리
  - ExperienceConfigAsset: 단일 에셋으로 경험치 설정 관리
  - GetEnhancementDataAsset(): 강화 데이터 에셋 가져오기
  - GetEnhancementData(StatNames): 스탯 이름으로 강화 데이터 가져오기
  - GetGrowthDataAsset(): 성장 데이터 에셋 가져오기 (GrowthConfigAsset 반환)
  - GetGrowthData(CharacterGrowthTypes): 성장 타입으로 성장 데이터 가져오기
  - GetExperienceConfigAsset(): 경험치 설정 에셋 가져오기
- **CharacterSystemAssetCreator**: Editor 스크립트로 기본값이 적용된 에셋 자동 생성
  - Unity 메뉴: TeamSuneat → Create Character System Assets
  - 생성 경로: Assets/Addressables/Scriptable/Character
  - EnhancementData.asset, GrowthConfig.asset, ExperienceConfig.asset 자동 생성

## 7. 캐릭터 페이지 UI 구조

### 7.1 캐릭터 페이지 개요
- **UITogglePageController**를 통해 캐릭터 토글을 눌러 캐릭터 페이지를 표시
- 캐릭터 페이지 내부에 또 하나의 **UITogglePageController**를 가짐
- 3개의 하위 토글: 강화 / 성장 / 승급

### 7.2 캐릭터 정보 영역 (Character Level Group)

**표시 조건**
- 강화 페이지: 표시
- 성장 페이지: 표시
- 승급 페이지: 숨김

**구성 요소**
- **캐릭터 아이콘 이미지**: 현재 캐릭터의 아이콘 표시
- **레벨 텍스트**: "Lv.{레벨}" 형식으로 현재 레벨 표시
- **경험치 비율 텍스트**: "{현재 경험치} / {필요 경험치}" 형식으로 표시
- **경험치 게이지**: UIGauge 컴포넌트를 사용하여 경험치 바 표시
  - FrontSlider: 현재 경험치 비율 표시
  - BackSlider: 이전 경험치 비율에서 부드럽게 감소 (선택적)
  - ValueText: 경험치 비율 텍스트 (선택적)
- **레벨업 버튼** (UICharacterLevelUpButton):
  - 활성화 조건: 경험치 비율이 100%일 때 (CanLevelUp() == true)
  - 비활성화 조건: 경험치 비율이 100% 미만일 때
  - 클릭 시 동작:
    - VCharacterLevel.CanLevelUpOrNotify()로 검증
    - 캐릭터 레벨 1 증가 (VCharacterLevel.LevelUp())
    - 현재 레벨의 필요 경험치만큼 차감 (ExperienceConfigAsset.GetExperienceRequiredForNextLevel 사용)
    - 레벨업 시 능력치 포인트 3개 획득 (VCharacterGrowth.AddStatPoint, ExperienceConfigAsset.StatPointPerLevel)
    - UI 갱신 (GAME_DATA_CHARACTER_LEVEL_CHANGED 이벤트 발생)
  - 다중 레벨업 지원: 경험치가 충분할 경우 여러 번 클릭하여 연속 레벨업 가능
- **경험치 게이지 UI** (UIExperienceGauge):
  - HUD에 표시되는 경험치 게이지
  - 경험치 획득 시 자동 업데이트 (GAME_DATA_CHARACTER_ADD_EXPERIENCE 이벤트 구독)
  - 레벨업 시 게이지 리셋

### 7.3 강화 페이지 (Enhancement Page)

**페이지 구조**
- **UITogglePageController**의 첫 번째 페이지 (인덱스 0)
- 강화 토글 클릭 시 표시

**강화 아이템 스크롤 뷰 (Character Enhancement Group)**
- Scroll View 컴포넌트를 사용하여 스크롤 가능한 목록 표시
- 강화할 수 있는 모든 타입(공격력, 체력, 체력 회복량, 치명타 공격력, 치명타 확률, 회심의 일격, 회심의 일격 확률)에 대해 하나씩 아이템 생성

**강화 아이템 (UIEnhancementItem) 구성 요소**
- **배경 이미지**: 아이템의 배경
- **프레임 이미지**: 아이템의 테두리
- **강화 아이콘 이미지**: 해당 강화 타입의 아이콘
- **강화 이름 텍스트**: 강화 타입 이름 (예: "공격력", "체력")
- **강화 레벨 텍스트**: "Lv.{현재 레벨}" 형식으로 현재 강화 레벨 표시
- **능력치 값 텍스트**: "{현재값} → {레벨업 후 값}" 형식으로 강화 시 능력치 변화 표시
  - 현재값: 현재 레벨의 능력치 값 (InitialValue + (현재 레벨 × GrowthValue))
  - 레벨업 후 값: 다음 레벨의 능력치 값 (InitialValue + ((현재 레벨 + 1) × GrowthValue))
- **레벨업 버튼**:
  - 재화 아이콘 이미지: 강화에 필요한 재화 아이콘 (골드)
  - 비용 텍스트: 강화에 필요한 비용 표시 (InitialCost × CostGrowthRate^(현재 레벨 - 1))
  - 클릭 시 동작:
    - 재화 충분 여부 확인 (VCurrency.CanUseOrNotify)
    - 최대 레벨 도달 여부 확인 (EnhancementData.MaxLevel)
    - 요구 능력치 레벨 충족 여부 확인 (EnhancementData.RequiredStatName, RequiredStatLevel)
    - 조건 충족 시 강화 레벨 1 증가 (VCharacterEnhancement.AddLevel) 및 재화 차감 (VCurrency.Use)
    - UI 갱신 (재화 변경 이벤트 발생)

### 7.4 강화 아이템 데이터 바인딩

**데이터 소스**
- EnhancementDataAsset: 강화 능력치의 고정 데이터 (최대 레벨, 초기값, 성장값, 비용 등)
- VCharacterEnhancement: 저장된 강화 레벨 데이터

**표시 데이터 계산**
- 현재 능력치 값: InitialValue + (현재 레벨 × GrowthValue)
- 다음 능력치 값: InitialValue + ((현재 레벨 + 1) × GrowthValue)
- 강화 비용: InitialCost × CostGrowthRate^(현재 레벨 - 1)

**UI 갱신 시점**
- 페이지 열릴 때
- 강화 레벨업 성공 시
- 재화 변경 시

### 7.5 성장 페이지 (Growth Page)

**페이지 구조**
- **UITogglePageController**의 두 번째 페이지 (인덱스 1)
- 성장 토글 클릭 시 표시

**성장 아이템 스크롤 뷰 (Character Growth Group)**
- Scroll View 컴포넌트를 사용하여 스크롤 가능한 목록 표시
- 성장할 수 있는 모든 타입(STR, HP, VIT, CRI, LUK, ACC, DODGE)에 대해 하나씩 아이템 생성

**성장 아이템 (UIGrowthItem) 구성 요소**
- **배경 이미지**: 아이템의 배경
- **프레임 이미지**: 아이템의 테두리
- **성장 아이콘 이미지**: 해당 성장 타입의 아이콘
- **성장 타입 텍스트**: 성장 타입 이름 (예: "STR", "HP")
- **최대 레벨 텍스트**: "Max Lv.{최대 레벨}" 형식으로 최대 레벨 표시
- **성장 레벨 텍스트**: "Lv.{현재 레벨}" 형식으로 현재 성장 레벨 표시
- **능력치 값 텍스트**: "{능력치 이름} {현재값} → {다음값}" 형식으로 성장 시 능력치 변화 표시
  - 현재값: 현재 레벨의 능력치 값 (레벨 × StatIncreasePerLevel)
  - 다음값: 다음 레벨의 능력치 값 ((레벨 + 1) × StatIncreasePerLevel)
- **레벨업 버튼** (UIGrowthButton):
  - 프레임 이미지: 버튼의 테두리 (색상으로 활성화 상태 표시)
  - 활성화 색상: SteelBlue (레벨업 가능)
  - 비활성화 색상: IndianRed (레벨업 불가)
  - 클릭/홀드 시 동작:
    - 최대 레벨 도달 여부 확인 (GrowthConfigData.MaxLevel)
    - 능력치 포인트 충분 여부 확인 (VCharacterGrowth.CanConsumeStatPointOrNotify)
    - 조건 충족 시 성장 레벨 1 증가 (VCharacterGrowth.AddLevel) 및 능력치 포인트 1개 소비 (VCharacterGrowth.ConsumeStatPoint)
    - UI 갱신 (GAME_DATA_CHARACTER_GROWTH_LEVEL_CHANGED, GAME_DATA_CHARACTER_GROWTH_STAT_POINT_CHANGED 이벤트 발생)
    - 레벨업 성공 시 능력치 값 텍스트에 펀치 스케일 애니메이션 재생

**성장 페이지 추가 구성 요소**
- **능력치 포인트 텍스트**: "능력치 포인트: {현재 포인트}" 형식으로 표시
- 전역 이벤트 구독:
  - GAME_DATA_CHARACTER_LEVEL_CHANGED: 캐릭터 레벨 변경 시 모든 아이템 갱신
  - GAME_DATA_CHARACTER_GROWTH_LEVEL_CHANGED: 성장 레벨 변경 시 페이지 갱신
  - GAME_DATA_CHARACTER_GROWTH_STAT_POINT_CHANGED: 능력치 포인트 변경 시 포인트 텍스트 갱신

### 7.6 성장 아이템 데이터 바인딩

**데이터 소스**
- GrowthConfigAsset: 성장 능력치의 고정 데이터 (최대 레벨, 레벨당 증가량 등)
- VCharacterGrowth: 저장된 성장 레벨 데이터 및 능력치 포인트

**표시 데이터 계산**
- 현재 능력치 값: 현재 레벨 × StatIncreasePerLevel
- 다음 능력치 값: (현재 레벨 + 1) × StatIncreasePerLevel
- 레벨업 비용: 항상 능력치 포인트 1개 고정

**UI 갱신 시점**
- 페이지 열릴 때
- 성장 레벨업 성공 시
- 능력치 포인트 변경 시
- 캐릭터 레벨 변경 시

### 7.7 UI 계층 구조

```
UIPageGroup
├── Page(Character)
│   ├── Background Image
│   ├── UITogglePageController
│   │   ├── UIToggleGroup
│   │   │   ├── Toggle (Enhancement) - 강화
│   │   │   ├── Toggle (Growth) - 성장
│   │   │   └── Toggle (Promotion) - 승급
│   │   └── UIPageGroup
│   │       ├── Page (Enhancement) - UIEnhancementPage
│   │       ├── Page (Growth) - UIGrowthPage
│   │       └── Page (Promotion) - UIPromotionPage
│   ├── Character Level Group (강화/성장 페이지에서만 표시)
│   │   ├── Character Icon Image
│   │   ├── Level Name Text
│   │   ├── Exp Name Text
│   │   ├── UIGauge
│   │   │   ├── Slider (Front)
│   │   │   ├── Slider (Back)
│   │   │   └── Value Text
│   │   └── LevelUp Button
│   └── Character Enhancement Group (강화 페이지 내용)
│       └── Scroll View
│           └── Viewport
│               └── Content
│                   └── UIEnhancementItem (강화 타입별 생성)
│                       ├── Background Image
│                       ├── Frame Image
│                       ├── Stat Icon Image
│                       ├── Stat Name Text
│                       ├── Level Text
│                       ├── StatValue Text
│                       └── LevelUp Button
│                           ├── Currency Icon Image
│                           └── Text (TMP)
│   └── Character Growth Group (성장 페이지 내용)
│       └── Scroll View
│           └── Viewport
│               └── Content
│                   └── UIGrowthItem (성장 타입별 생성)
│                       ├── Background Image
│                       ├── Frame Image
│                       ├── Stat Icon Image
│                       ├── Growth Type Text
│                       ├── Growth MaxLevel Text
│                       ├── Growth Level Text
│                       ├── Growth Stat Text (현재값 → 다음값 형식)
│                       └── LevelUp Button (UIGrowthButton)
│                           └── Frame Image (색상으로 활성화 상태 표시)
```
