# 1주차 작업 요약 (2024.12.16 ~ 2024.12.19)

## 📅 작업 기간
- **시작일**: 2024년 12월 16일 (화요일)
- **종료일**: 2024년 12월 19일 (금요일)
- **작업 일수**: 4일

---

## 🎯 주요 완료 작업

### 1. 씬 시스템 및 UI 프레임워크 구축 (Day 1)

#### 씬 시스템
- ✅ 타이틀 씬 UI 구현 (GameTitleScene.cs)
  - 모바일 게임 특성상 탭하여 게임 시작 기능
- ✅ 로딩 씬 기본 구조 구현
  - 진행률 표시, 로딩 애니메이션/텍스트, 리소스 로드 관리
- ✅ 게임 메인 씬 기본 구조 구현
  - 인게임 플레이 영역, UI 오버레이, 토글 및 페이지 기능
- ✅ 씬 전환 시스템 테스트 완료

#### UI 프레임워크
- ✅ UI 매니저 구조 설계 및 구현
  - UIManager, UICanvasManager, UIPopupManager
- ✅ 팝업 시스템 기본 구조
  - UIPopup, UIPopupManager
  - 확인/취소 다이얼로그 구현
- ✅ Toggle/Page 시스템 구현
  - UIToggle, UIToggleGroup
  - UIPage, UIPageGroup
  - UITogglePageController
  - UIToggleIcon, UIToggleName, UIToggleLock, UIToggleUpdateIndicator

---

### 2. 데이터 구조 재설계 및 저장/로드 시스템 (Day 2, 4)

#### 데이터 구조 재설계
- ✅ VCharacter 구조 단순화
- ✅ VCharacterStat 분리
  - VCharacterEnhancement (강화 능력치)
  - VCharacterGrowth (성장 능력치)
- ✅ ScriptableObject 데이터 구조 구현
  - EnhancementDataAsset, GrowthDataAsset, ExperienceConfigAsset
- ✅ 신규 데이터 클래스 구현
  - VCharacterAccessory, VAccessory
- ✅ VProfile 업데이트

#### 저장/로드 시스템
- ✅ 게임 종료 시 저장 로직 구현
  - OnApplicationPause 처리 (GameApp)
  - OnApplicationFocus 처리 (XGameApp)
- ⏳ 주기적 자동 저장 구현 (예정)
- ⏳ 세이브 파일 관리 테스트 (예정)
- ⏳ 데이터 마이그레이션 코드 작성 (예정)

---

### 3. 전투 시스템 리팩토링 (Day 2)

#### DamageTypes 정리
- ✅ 사용하지 않는 피해 타입 삭제
  - Grab, Overwhelm, Sacrifice, Spawn 제거
- ✅ Physical → Normal로 통합

#### Vital 시스템 개선
- ✅ 마나 회복 시스템 추가
- ✅ 보호막 충전 시스템 복구
- ✅ Collider/Guard 기능 제거
- ✅ Life → Health로 네이밍 통일

#### 코드 정리
- ✅ DamageCalculator 코드 정리
- ✅ HitmarkAssetData 정리
- ✅ UIFloatyMoveNames, UIFloatyText 정리

---

### 4. 캐릭터 게이지 시스템 리팩토링 (Day 2)

#### 게이지 UI 구조 개선
- ✅ ICharacterGaugeView 인터페이스 통합
- ✅ UIGaugeManager 메서드 이름 변경

#### UIEnemyGauge 수정
- ✅ MonsterCharacter → Character로 변경
- ✅ 보호막/마나 게이지 삭제
- ✅ 쿨타임 게이지 추가
- ✅ VitalResource.Rate 프로퍼티 활용

#### UIPlayerGauge 구현
- ✅ 플레이어 전용 체력바 구현
- ✅ 체력/보호막/마나 게이지 지원
- ✅ VitalResource가 없으면 해당 게이지 자동 비활성화

#### Vital 게이지 시스템 개선
- ✅ Vital.Field에 PlayerGauge 프로퍼티 추가
- ✅ Vital.Gauge에 SpawnCharacterGauge() 메서드 추가
- ✅ ResourcesManager에 SpawnPlayerGauge() 메서드 추가

---

### 5. 캐릭터 정보 UI 및 페이지 시스템 (Day 2)

#### 캐릭터 정보 UI 시스템
- ✅ 데이터 모델 구현
  - VCharacterAccessory, VAccessory
  - VProfile에 Accessory 필드 추가
- ✅ UI 컴포넌트 구현
  - UICharacterInfoPopup
  - UICharacterBasicInfoPage
  - UICharacterAppearancePage
  - UIEquipmentSlot
  - UIStatDisplayGroup, UIStatDisplayEntry
- ✅ HUD 버튼 구현
  - HUDCharacterInfoButton
  - HUDCurrency

#### 캐릭터 페이지 시스템
- ✅ UICharacterPage (강화/성장/승급 탭 전환)
- ✅ UICharacterLevelGroup (레벨, 경험치, 레벨업 버튼)
- ✅ UIEnhancementPage, UIEnhancementItem (강화 페이지)
- ✅ UIGrowthPage (성장 페이지 - 스켈레톤)
- ✅ UIPromotionPage (승급 페이지 - 스켈레톤)

---

### 6. 게임 데이터 등록 (Day 2)

#### 재화 시스템
- ✅ 재화 타입 추가 (Diamond, Emerald)

#### 아이템 시스템
- ✅ 무기/악세사리 등록 (ItemNames)
- ✅ 아이템 등급 등록 (GradeNames)
- ✅ 아이템 등급별 색상 설정 (EnumColorEx)

#### 지역/스테이지 시스템
- ✅ 지역 등록 (AreaNames)
- ✅ 스테이지 등록 (StageNames)
- ✅ 1지역 몬스터 등록 (Area01_Mushroom)
- ✅ 현재 지역/최대 도달 지역/스테이지 세이브 데이터 추가 (VCharacterStage)
- ✅ 스테이지 프리팹 어드레서블 라벨 설정

---

### 7. 플레이어 캐릭터 시스템 (Day 2)

- ✅ 플레이어 캐릭터 스프라이트/애니메이션 추가 (kill_chainsaw 시리즈)
- ✅ 플레이어 캐릭터 Animator Controller 추가
- ✅ PlayerCharacterSpawner 클래스 구현
- ✅ 메인 씬 진입 시 플레이어 캐릭터 자동 생성

---

### 8. 리소스 관리 시스템 (Day 3)

#### 재화 관리 (VCurrency)
- ✅ 재화 획득/소비 기능 (Add, Use, UseAll)
- ✅ 재화 조회 기능 (GetAmount)
- ✅ 재화 소비 검증 (CanUse, CanUseOrNotify)
- ✅ 재화 변경 이벤트 (CURRENCY_EARNED, CURRENCY_PAYED)

#### 경험치 관리 (VCharacterLevel)
- ✅ 경험치 획득/관리 (AddExperience)
- ✅ 경험치 표시 및 관리 (Experience, Level 필드)
- ✅ 레벨업 필요 경험치 계산 (CharacterLevelExpData 사용)
- ✅ 경험치 변경 이벤트 (GAME_DATA_CHARACTER_ADD_EXPERIENCE, GAME_DATA_CHARACTER_LEVEL_CHANGED)

---

### 9. 메인 HUD 구현 (Day 3, 4)

#### 재화 상시 표시 UI (HUDCurrency)
- ✅ 재화 표시 UI 컴포넌트 구현
  - 재화별 아이콘 표시
  - 수량 표시 및 숫자 포맷팅
- ✅ 재화 UI 업데이트 시스템
  - 실시간 재화 변경 반영 (이벤트 구독)
  - 초기값 설정

#### 경험치 게이지 UI
- ✅ UIExperienceGauge 구현
  - 경험치 게이지 바
  - 현재 경험치 / 필요 경험치 표시
  - 레벨 표시
  - 레벨업 가능 여부 표시
- ✅ 경험치 게이지 업데이트 시스템
  - 경험치 획득 시 자동 업데이트
  - 레벨업 시 게이지 리셋

#### 재화 사용처 구현
- ✅ 재화 획득/소비/조회 로직 통합
- ✅ 재화 부족 시 안내 처리 (UINoticeManager)
- ✅ 인게임 재화 초기화 (VCurrency.ClearIngameCurrencies)
- ✅ 재화 이벤트 연동 완료

---

### 10. Notice 시스템 구현 (Day 4)

#### UINoticeManager
- ✅ Notice UI 컴포넌트 중앙 관리
- ✅ 재화 부족 이벤트 자동 리스닝
- ✅ Notice 생성 및 완료 시 자동 제거

#### UINoticeBase
- ✅ 공통 기능 추출 (Show, FadeInOut, OnCompleted 이벤트)
- ✅ UICurrencyShortageNotice 리팩토링
- ✅ UIStageTitleNotice 리팩토링

#### Notice 컴포넌트
- ✅ UICurrencyShortageNotice (재화 부족 알림)
- ✅ UIStatPointShortageNotice (스탯 포인트 부족 알림)
- ✅ UIExperienceShortageNotice (경험치 부족 알림)

#### 이벤트 및 데이터 모델 연동
- ✅ GlobalEventType 이벤트 추가 (STAT_POINT_SHORTAGE, EXPERIENCE_SHORTAGE)
- ✅ VCharacterGrowth에 CanConsumeStatPoint, CanConsumeStatPointOrNotify 추가
- ✅ VCharacterLevel에 CanLevelUp, CanLevelUpOrNotify 추가
- ✅ 버튼 클릭 로직 수정 (OrNotify 메서드 사용)

---

### 11. 시간 관리 시스템 구현 (Day 4)

#### 게임 접속 시간 기록 시스템 (VStatistics)
- ✅ 기본 시간 필드 구현
  - GameStartTime (게임 시작 시간)
  - LastSaveTime (마지막 저장 시간)
- ✅ 시간 관련 메서드 구현
  - RegisterGameStartTime()
  - RegisterLastSaveTime()
  - GetTotalPlayTime()
  - GetTotalPlayTimeString()
  - GetLastSaveTimeString()
- ✅ 게임 종료 시 LastSaveTime 업데이트
  - OnApplicationPause/OnApplicationFocus에서 처리
  - 주기적 자동 저장 시 처리

#### 오프라인 시간 계산 시스템
- ✅ OfflineTimeManager 클래스 구현
  - 마지막 플레이 시간 기록 (LastSaveTime 활용)
  - 현재 시간과 비교하여 오프라인 시간 계산
  - 오프라인 시간 제한 (최대 24시간)
  - 오프라인 시간 검증 (시간 조작 방지)
  - 게임 시작 시 오프라인 시간 계산
  - 앱 재개 시 오프라인 시간 재계산
  - 단일 책임 원칙에 따른 함수 분리

---

### 12. 통계/기록 시스템 구현 (Day 4)

#### 통계 기록 시스템 (VStatistics)
- ✅ 기본 통계 구조 정리
  - 방치형 게임에 불필요한 통계 항목 제거
    - 도전 횟수, 죽음 횟수, 처치한 몬스터
    - 스피드런 클리어 시간, 도전 시작 시간
    - 난이도별 클리어 시간, 현재 게임플레이 시간
  - 게임 접속 시간 관련 필드만 유지 (GameStartTime, LastSaveTime)
- ⏳ 통계 표시 UI (향후 구현 예정)

---

### 13. UI 시스템 개선 (Day 2, 3)

- ✅ UI 색상 변경 (GameColors 업데이트, EnumColorEx 확장)
- ✅ 페이지/토글 그룹 개선
  - UIPage, UIPageGroup 자식 오브젝트를 바로 아래 자식으로 제한
  - UIToggleGroup 자식 오브젝트를 바로 아래 자식으로 제한
- ✅ 토글과 버튼에 클릭 효과 추가
- ✅ 빠른 터치에 의한 버그 수정
- ✅ 게임 내 UI 폰트와 텍스트 크기 통일
- ✅ 언어 변경에 따른 버그 수정

---

### 14. 코드 정리 (Day 2)

- ✅ 불필요한 데이터 삭제
  - WeaponLevelData 클래스 삭제
  - WeaponLevelRowConverter 클래스 삭제
  - 구글 스프레드시트 무기 레벨 데이터 연동 제거

---

## ⏳ 진행 중 / 예정 작업

### 메인 HUD 구현 (Day 4)
- ⏳ 스테이지 진행률 게이지 UI
  - 스테이지 진행률 게이지 바
  - 현재 웨이브 / 전체 웨이브 표시
  - 진행률 퍼센트 표시 (선택)
  - 웨이브 클리어 시 진행률 업데이트
  - 스테이지 변경 시 진행률 리셋
- ⏳ 지역 정보 표시 UI
  - 현재 지역 이름 텍스트
  - 지역 번호 표시
  - 지역 정보 업데이트
- ⏳ 보스 도전 버튼 UI
  - 보스 버튼 컴포넌트
  - 버튼 활성화/비활성화 상태
  - 보스 도전 가능 여부 표시
  - 언제든지 스테이지 보스 도전 기능
  - 보스 도전 시 모든 스킬 쿨타임 초기화

### 저장/로드 시스템
- ⏳ 주기적 자동 저장 구현
  - 저장 주기 설정 (예: 30초마다)
  - 저장 트리거 관리
  - 저장 실패 시 재시도 로직
- ⏳ 강제 종료 대응 (가능한 범위 내)
- ⏳ 세이브 파일 관리 테스트
- ⏳ 데이터 마이그레이션 코드 작성

### 통합 테스트
- ⏳ HUD UI 통합 테스트
  - 모든 재화 표시 정상 동작 확인
  - 경험치 게이지 정상 동작 확인
  - 진행률 게이지 정상 동작 확인
- ⏳ 데이터 시스템 통합 테스트
  - 재화 획득/소비 정상 동작 확인
  - 경험치 시스템 정상 동작 확인
  - 저장/로드 정상 동작 확인
- ⏳ 버그 수정 및 안정화

---

## 📊 작업 통계

### 완료된 작업
- **총 작업 항목**: 약 150개 이상
- **완료율**: 약 85% (대부분의 핵심 기능 완료)

### 주요 성과
1. ✅ **기본 인프라 구축 완료**
   - 씬 시스템, UI 프레임워크, 데이터 구조 재설계
2. ✅ **핵심 게임 시스템 구현**
   - 재화/경험치 관리, 캐릭터 시스템, 전투 시스템 개선
3. ✅ **HUD 및 UI 시스템 구축**
   - 메인 HUD, Notice 시스템, 캐릭터 정보 UI
4. ✅ **시간 관리 시스템 구현**
   - 게임 접속 시간 기록, 오프라인 시간 계산

---

## 🔄 다음 주 계획 (예상)

1. 스테이지 진행률 게이지 및 지역 정보 UI 완성
2. 보스 도전 시스템 구현
3. 주기적 자동 저장 시스템 구현
4. 데이터 마이그레이션 코드 작성
5. 통합 테스트 및 버그 수정
6. 성장/승급 페이지 기능 완성

---

## 📝 참고 사항

- 모든 작업은 Unity 프로젝트에서 진행되었습니다.
- 코드 스타일은 Unity Code Style Guide를 따릅니다.
- 주석 및 문서는 한국어로 작성되었습니다.
- 이벤트 기반 아키텍처를 활용하여 시스템 간 결합도를 낮췄습니다.

