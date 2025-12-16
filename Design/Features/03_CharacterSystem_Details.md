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

### 4.3 성장 시스템 데이터 구조
- **GrowthDataAsset**: 단일 ScriptableObject로 모든 성장 능력치 데이터 관리
  - XScriptableObject를 상속받아 ScriptableDataManager에서 관리
  - DataArray: GrowthData 배열로 모든 성장 능력치 데이터 포함
  - FindGrowthData(StatNames): 스탯 이름으로 데이터 검색
  - Editor 기능: "모든 성장 능력치 데이터 자동 생성" 버튼 제공
  - 성장 시스템 능력치만 자동 생성 (Strength, HealthPoint, Vitality, Critical, Luck, AccuracyStat, Dodge)
- **GrowthData**: 개별 성장 능력치 데이터
  - StatName: 능력치 종류
  - MaxLevel: 최대 레벨
  - InitialCost: 능력치 포인트 초기 비용
  - CostGrowthRate: 비용 성장률
  - StatIncreasePerLevel: 레벨당 스탯 증가량

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

**VCharacterEnhancement**
- EnhancementLevels: 강화 능력치별 레벨 저장 (Dictionary<string, int>)
- GetLevel(StatNames): 특정 강화 능력치의 레벨 가져오기
- SetLevel(StatNames, int): 특정 강화 능력치의 레벨 설정
- AddLevel(StatNames, int): 특정 강화 능력치의 레벨 증가
- ClearIngameData(): 인게임 데이터 초기화

**VCharacterGrowth**
- StatPoint: 능력치 포인트
- GrowthLevels: 성장 능력치별 레벨 저장 (Dictionary<string, int>)
- GetLevel(StatNames): 특정 성장 능력치의 레벨 가져오기
- SetLevel(StatNames, int): 특정 성장 능력치의 레벨 설정
- AddLevel(StatNames, int): 특정 성장 능력치의 레벨 증가
- AddStatPoint(int): 능력치 포인트 추가
- ConsumeStatPoint(int): 능력치 포인트 소비
- ClearIngameData(): 인게임 데이터 초기화

### 6.2 데이터 안정성
- Dictionary 키를 string으로 저장하여 enum 순서 변경에 영향받지 않도록 구현
- StatNames enum을 ToString()으로 변환하여 저장
- VCharacterEnhancement, VCharacterGrowth의 메서드는 StatNames를 받아 내부에서 string으로 변환

### 6.3 ScriptableObject 관리
- **ScriptableDataManager**: 모든 ScriptableObject 에셋을 중앙에서 관리
  - EnhancementDataAsset: 단일 에셋으로 강화 시스템 데이터 관리
  - GrowthDataAsset: 단일 에셋으로 성장 시스템 데이터 관리
  - ExperienceConfigAsset: 단일 에셋으로 경험치 설정 관리
  - GetEnhancementDataAsset(): 강화 데이터 에셋 가져오기
  - GetEnhancementData(StatNames): 스탯 이름으로 강화 데이터 가져오기
  - GetGrowthDataAsset(): 성장 데이터 에셋 가져오기
  - GetGrowthData(StatNames): 스탯 이름으로 성장 데이터 가져오기
  - GetExperienceConfigAsset(): 경험치 설정 에셋 가져오기
- **CharacterSystemAssetCreator**: Editor 스크립트로 기본값이 적용된 에셋 자동 생성
  - Unity 메뉴: TeamSuneat → Create Character System Assets
  - 생성 경로: Assets/Addressables/Scriptable/Character
  - EnhancementData.asset, GrowthData.asset, ExperienceConfig.asset 자동 생성

