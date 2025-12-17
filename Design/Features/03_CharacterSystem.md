# 캐릭터 시스템

> 구현 기능 문서입니다.
> 상세 정보는 `03_CharacterSystem_Details.md`를 참고하세요.

## 1. 기능별 고정적인 데이터

### 1.1 스탯 정의
- [x] 스탯 종류 정의
  - [x] 공격력
  - [x] 체력
  - [x] 체력회복
  - [x] 치명타 확률
  - [x] 치명타 피해
  - [x] 마나
  - [x] 마나회복
  - [x] 명중
  - [x] 회피
  - [x] 추가 골드 획득량
  - [x] 추가 경험치
- [x] 각 스탯의 초기값 정의 (EnhancementData, GrowthData에 포함)
- [x] 각 스탯의 레벨별 성장값 정의 (EnhancementData, GrowthData에 포함)

### 1.2 강화 시스템 데이터
- [x] 골드로 구매하는 강화 능력치 정의
  - [x] 공격력 (MAX LV: 1,900,000)
  - [x] 체력 (MAX LV: 1,900,000)
  - [x] 체력 회복량 (MAX LV: 1,900,000)
  - [x] 치명타 공격력 (MAX LV: 10,000)
  - [x] 치명타 확률 (MAX LV: 1,000)
  - [x] 회심의 일격 (MAX LV: 4,000)
  - [x] 회심의 일격 확률 (MAX LV: 1,000)
- [x] 업그레이드 비용 계산 공식 참조 (EnhancementData에 InitialCost, CostGrowthRate 포함)
- [x] 레벨 제한 시스템 (MAX LV 체크 로직) (EnhancementData에 MaxLevel 포함)
- [x] 요구 능력치 및 요구 레벨 시스템 (EnhancementData에 RequiredStatName, RequiredStatLevel 포함)
- [x] ScriptableObject 데이터 구조 구현
  - [x] EnhancementDataAsset: 단일 에셋으로 모든 강화 능력치 데이터 관리
  - [x] EnhancementData: 개별 강화 능력치 데이터 (초기값, 성장값, 비용 등)
  - [x] ScriptableDataManager 통합
  - [x] Editor 자동 생성 기능 (CharacterSystemAssetCreator)

### 1.3 성장 지식 시스템 데이터
- [ ] 성장 지식 종류 정의
  - [ ] 괴력 (MAX LV: 75단계)
  - [ ] 강철의 육체 (MAX LV: 75단계)
  - [ ] 강인한 심장 (MAX LV: 75단계)
  - [ ] 초인의 힘 (MAX LV: 75단계)
- [ ] 성장 지식 도전 조건 정의
  - [ ] 공격력/체력/체력 회복량 특정 레벨 이상 달성 시 도전 가능
- [ ] 회심의 일격 최대 레벨 확장 규칙
  - [ ] 성장 지식 한 줄 완료 시 회심의 일격 최대 레벨 확장
  - [ ] 초인의 힘 전투를 통한 추가 확장

### 1.4 성장 시스템 데이터
- [x] 능력치 포인트로 구매 가능한 능력치 정의
  - [x] STR (MAX LV: 1,000)
  - [x] HP (MAX LV: 1,000)
  - [x] VIT (MAX LV: 1,000)
  - [x] CRI (MAX LV: 200)
  - [x] LUK (MAX LV: 1,000)
  - [x] ACC (MAX LV: 200)
  - [x] DODGE (MAX LV: 200)
- [x] 레벨업 시 능력치 포인트 3개 획득 규칙 (ExperienceConfigAsset에 StatPointPerLevel 포함)
- [x] 레벨 제한 시스템 (MAX LV 체크 로직) (GrowthData에 MaxLevel 포함)
- [x] ScriptableObject 데이터 구조 구현
  - [x] GrowthDataAsset: 단일 에셋으로 모든 성장 능력치 데이터 관리
    - [x] CharacterGrowthTypes 기반 검색 메서드 추가
      - [x] FindGrowthDataByType(CharacterGrowthTypes) 메서드
      - [x] GetStatNameByGrowthType() 변환 메서드
      - [x] GetGrowthTypeByStatName() 변환 메서드
    - [x] CreateAllStatData에서 GrowthType 자동 설정
    - [x] LogErrorInvalid에 GrowthType 유효성 검사 및 중복 체크
  - [x] GrowthData: 개별 성장 능력치 데이터 (레벨당 증가량, 비용 등)
  - [x] ScriptableDataManager 통합
  - [x] Editor 자동 생성 기능 (CharacterSystemAssetCreator)

### 1.5 경험치 필요량 계산 데이터
- [x] 초기 경험치 필요량: 120 (ExperienceConfigAsset에 InitialExperienceRequired 포함)
- [x] 경험치 증가 배율: 1.01 (레벨마다 1.01배씩 증가) (ExperienceConfigAsset에 ExperienceGrowthRate 포함)
- [x] 레벨 n 필요 경험치: 120 × 1.01^(n-1) (ExperienceConfigAsset.GetExperienceRequiredForNextLevel() 메서드)
- [x] 레벨 n 총 경험치: 12000 × (1.01^n - 1) (ExperienceConfigAsset.GetTotalExperienceRequired() 메서드)
- [x] 레벨업 시 능력치 포인트 3개 획득 (ExperienceConfigAsset에 StatPointPerLevel 포함)
- [x] ScriptableObject 데이터 구조 구현
  - [x] ExperienceConfigAsset: 경험치 필요량 계산 설정 관리
  - [x] ScriptableDataManager 통합
  - [x] Editor 자동 생성 기능 (CharacterSystemAssetCreator)

### 1.6 계산 공식 참조
- [x] 스탯 증가 계산 공식 참조 (EnhancementData, GrowthData에 구현)
- [x] 경험치 필요량 계산 공식 참조 (ExperienceConfigAsset에 구현)
- [ ] 능력치 효과 적용 계산 공식 참조 (향후 구현)

## 2. 세이브하는 데이터

### 2.1 캐릭터 기본 정보
- [x] 캐릭터 레벨 저장 (VCharacterLevel.Level)
- [x] 현재 경험치 저장 (VCharacterLevel.Experience)

### 2.2 강화 시스템 데이터
- [x] 강화 능력치 레벨 저장 (VCharacterEnhancement.EnhancementLevels Dictionary<string, int>)
  - [x] 공격력 레벨
  - [x] 체력 레벨
  - [x] 체력 회복량 레벨
  - [x] 치명타 공격력 레벨
  - [x] 치명타 확률 레벨
  - [x] 회심의 일격 레벨
  - [x] 회심의 일격 확률 레벨

### 2.3 성장 지식 시스템 데이터
- [ ] 성장 지식 단계 저장
  - [ ] 괴력 단계
  - [ ] 강철의 육체 단계
  - [ ] 강인한 심장 단계
  - [ ] 초인의 힘 단계
- [ ] 성장 지식 도전 진행 상태 저장

### 2.4 성장 시스템 데이터
- [x] 능력치 포인트 저장 (VCharacterGrowth.StatPoint)
- [x] 능력치 레벨 저장 (VCharacterGrowth.GrowthLevels Dictionary<string, int>)
  - [x] STR 레벨
  - [x] HP 레벨
  - [x] VIT 레벨
  - [x] CRI 레벨
  - [x] LUK 레벨
  - [x] ACC 레벨
  - [x] DODGE 레벨
- [x] CharacterGrowthTypes 기반 레벨 관리 메서드
  - [x] GetLevel(CharacterGrowthTypes) 메서드
  - [x] SetLevel(CharacterGrowthTypes, int) 메서드
  - [x] AddLevel(CharacterGrowthTypes, int) 메서드
  - [x] GrowthTypeToStatName() 변환 메서드

## 3. UI

### 3.1 캐릭터 정보 표시 UI
- [x] HUD에 캐릭터 정보 팝업 열기 버튼
  - [x] 버튼 UI 구현 (HUDCharacterInfoButton.cs)
  - [x] 버튼 클릭 시 팝업 열기 기능
- [x] 캐릭터 정보 팝업
  - [x] 팝업 이름: "캐릭터 정보" (UICharacterInfoPopup.cs)
  - [x] 탭 시스템
    - [x] 기본 정보 탭 (UICharacterBasicInfoPage.cs)
    - [x] 외형 탭 (UICharacterAppearancePage.cs)
    - [x] 탭 클릭 시 해당 페이지 전환 기능 (UITogglePageController 활용)
  - [x] 기본 정보 탭 내용
    - [x] 캐릭터 레벨 텍스트 표시
    - [x] 장착한 무기 슬롯 (UIEquipmentSlot.cs)
      - [x] 무기 이름 표시
      - [x] 무기 아이콘 표시
      - [x] 무기 레벨 표시
    - [x] 장착한 악세사리 슬롯 (UIEquipmentSlot.cs)
      - [x] 악세사리 이름 표시
      - [x] 악세사리 아이콘 표시
      - [x] 악세사리 레벨 표시
    - [x] 능력치 표시 영역 (UIStatDisplayGroup.cs, UIStatDisplayEntry.cs)
      - [x] 능력치 이름 텍스트들 (공격력, 체력, 체력회복, 치명타 확률, 치명타 피해, 마나, 마나회복, 명중, 회피, 추가 골드 획득량, 추가 경험치)
      - [x] 능력치 값 텍스트들
    - [ ] 메인 인포 전환 버튼 (향후 구현)
      - [ ] 버튼 클릭 시 능력치 정보 숨김
      - [ ] 메인 인포 표시
  - [ ] 메인 인포 영역 (향후 구현)
    - [ ] 동료 슬롯들
      - [ ] 아이콘 표시
      - [ ] 레벨 표시
    - [ ] 사용스킬 슬롯들
      - [ ] 아이콘 표시
      - [ ] 레벨 표시
    - [ ] 유물 슬롯들
      - [ ] 아이콘 표시
      - [ ] 레벨 표시
    - [ ] 정령 슬롯들
      - [ ] 아이콘 표시
      - [ ] 레벨 표시
  - [ ] 외형 탭 내용 (향후 구현)
- [ ] 경험치 표시 및 관리
- [ ] 능력치 포인트 표시 및 관리

### 3.2 경험치 및 레벨업 UI
- [ ] 경험치 바 표시
- [ ] 레벨업 처리 UI
  - [ ] 레벨업 시 능력치 포인트 3개 획득 표시
  - [ ] 레벨업 효과 적용 표시
- [ ] 적 처치 시 경험치 획득 표시

### 3.3 강화 시스템 UI
- [ ] 캐릭터 페이지 강화 탭
  - [ ] UITogglePageController를 통한 탭 전환
  - [ ] 강화 / 성장 / 승급 토글
- [ ] 캐릭터 정보 영역 (Character Level Group)
  - [ ] 캐릭터 아이콘 이미지
  - [ ] 캐릭터 레벨 텍스트
  - [ ] 경험치 비율 텍스트
  - [ ] 경험치 게이지 (UIGauge)
  - [ ] 레벨업 버튼 (경험치 100% 시 활성화)
  - [ ] 강화/성장 페이지에서 표시, 승급 페이지에서 숨김
- [ ] 강화 아이템 스크롤 뷰 (Character Enhancement Group)
  - [ ] 모든 강화 타입에 대해 아이템 생성
  - [ ] UIEnhancementItem 컴포넌트
    - [ ] 강화 아이콘 표시
    - [ ] 강화 이름 표시
    - [ ] 현재 레벨 표시
    - [ ] 능력치 값 표시 (현재값 → 레벨업 후 값)
    - [ ] 레벨업 버튼 (재화 아이콘 + 비용 텍스트)
- [ ] 강화 레벨업 처리
  - [ ] 재화 충분 여부 확인
  - [ ] 최대 레벨 도달 여부 확인
  - [ ] 요구 능력치 레벨 충족 여부 확인
  - [ ] 강화 레벨 증가 및 재화 차감

### 3.4 성장 지식 시스템 UI
- [ ] 성장 지식 UI
  - [ ] 각 성장 지식 표시
  - [ ] 현재 단계 표시
  - [ ] MAX LV 표시
  - [ ] 도전 조건 표시
  - [ ] 도전 가능 여부 표시
- [ ] 성장 지식 획득 UI (전투를 통해 획득)
- [ ] 회심의 일격 최대 레벨 확장 표시

### 3.5 성장 시스템 UI
- [ ] 능력치 포인트로 구매 가능한 능력치 UI
  - [ ] 각 능력치 표시
  - [ ] 현재 레벨 표시
  - [ ] MAX LV 표시
  - [ ] 능력치 포인트 소비 표시
  - [ ] 업그레이드 버튼
- [ ] 능력치 효과 적용 표시

### 3.6 캐릭터 관리 UI
- [ ] 캐릭터 선택/변경 UI (선택)
- [ ] 캐릭터 스탯 비교 UI (선택)

## 4. 인게임

### 4.1 스탯 계산 및 적용
- [ ] 스탯 레벨 기반 계산
  - [ ] 고정 데이터에서 초기값 불러오기
  - [ ] 고정 데이터에서 레벨별 성장값 불러오기
  - [ ] 스탯 값 = 초기값 + (레벨 × 성장값) 계산
- [ ] 강화 시스템 스탯 적용
- [ ] 성장 시스템 스탯 적용
- [ ] 최종 스탯 계산 및 적용

### 4.2 경험치 획득 및 레벨업
- [ ] 적 처치 시 경험치 획득
- [ ] 추가 경험치 보너스 적용
- [ ] 경험치 누적
- [ ] 레벨업 조건 체크
  - [ ] 현재 레벨의 총 경험치 필요량 계산: 12000 × (1.01^현재레벨 - 1)
  - [ ] 다음 레벨의 총 경험치 필요량 계산: 12000 × (1.01^다음레벨 - 1)
  - [ ] 현재 경험치가 다음 레벨 필요량 이상인지 체크
- [ ] 레벨업 처리
  - [ ] 능력치 포인트 3개 획득
  - [ ] 스탯 증가 적용

### 4.3 체력 관리
- [ ] 현재 체력 관리
- [ ] 최대 체력 계산
- [ ] 체력 회복 처리
- [ ] 체력 회복량 적용

### 4.4 마나 관리
- [ ] 현재 마나 관리
- [ ] 최대 마나 계산
- [ ] 마나 회복 처리
- [ ] 마나 회복량 적용

### 4.5 실제 피해량 계산
- [ ] 스탯 합산 처리
  - [ ] 기본 공격력 계산 (레벨 기반)
  - [ ] 장비 공격력 합산
  - [ ] 강화 공격력 합산
  - [ ] 총 공격력 = 기본 공격력 + 장비 공격력 + 강화 공격력
- [ ] 치명타 데미지 계산
  - [ ] 치명타 공격력 % 확인
  - [ ] 치명타 확률 % 확인
  - [ ] 최종 치명타 데미지 = 총 공격력 × (1 + 치명타 공격력 % × 치명타 확률 %)
- [ ] 총 데미지 계산
  - [ ] 평타/스킬 계수 확인
  - [ ] 치명타 데미지와 평타 데미지 차이 계산
  - [ ] 회심의 일격 보너스 계산
  - [ ] 총 데미지 = (평타/스킬 계수) × 총 공격력 × (1 + (치명타 데미지 - 평타 데미지) × 치명타 확률) + 회심의 일격 보너스

### 4.6 치명타 시스템
- [ ] 치명타 확률 계산
- [ ] 치명타 발생 판정
- [ ] 치명타 피해 계산
- [ ] 회심의 일격 확률 계산
- [ ] 회심의 일격 발생 판정
- [ ] 회심의 일격 피해 계산

### 4.7 명중 및 회피 시스템
- [ ] 명중률 계산
- [ ] 회피율 계산
- [ ] 공격 명중 판정
- [ ] 공격 회피 판정

### 4.8 골드 및 경험치 보너스
- [ ] 추가 골드 획득량 적용
- [ ] 추가 경험치 보너스 적용

### 4.9 성장 지식 시스템
- [ ] 성장 지식 도전 조건 체크
- [ ] 성장 지식 획득 처리 (전투를 통해)
- [ ] 성장 지식 효과 적용
- [ ] 회심의 일격 최대 레벨 확장 처리
