# 스테이지 스크롤링 시스템

## 개요
방치형 게임의 전형적인 스크롤링 메커니즘을 구현합니다.
플레이어는 고정된 위치에 있고, 몬스터와 배경이 스크롤되어 마치 앞으로 나아가는 것 같은 시각적 효과를 제공합니다.

## 구현 완료 항목

### 스크롤 컨트롤러
- [x] StageScrollController 클래스 구현
- [x] 스크롤 컨테이너 관리 (몬스터 및 스테이지 요소 이동)
- [x] 스크롤 속도 제어 (ScrollSpeed) - 현재 미사용 (포인트 이동 방식 사용)
- [x] 스크롤 방향 설정 (기본: 왼쪽)
- [x] 스크롤 중지 기능 (StopScrolling)
- [x] StageSystem과의 통합
- [x] Tween 기반 부드러운 포인트 이동 구현

### 배경 레이어 시스템
- [x] 기본 배경 레이어 스크롤링
- [x] 패럴랙스 스크롤 지원 (레이어별 다른 속도)
- [x] InfiniteBackgroundLayer 클래스 구현
- [x] 무한 배경 스크롤링 (배경 조각 재배치)

### 무한 배경 스크롤링
- [x] 배경 조각 2개 이상을 나란히 배치
- [x] 배경 너비 자동 계산 (SpriteRenderer, RectTransform, Renderer 지원)
- [x] 화면 밖으로 나간 배경을 반대편으로 이동
- [x] 왼쪽/오른쪽 스크롤 방향 모두 지원
- [x] 카메라 기준 배경 재배치 로직
- [x] 카메라 정보 캐싱으로 성능 최적화
- [x] 배경 재배치 로직 리팩토링 (중복 제거, 헬퍼 메서드 분리)

### 몬스터 스폰 시스템 연동
- [x] MonsterCharacterSpawner가 스크롤 컨테이너 하위에 스폰
- [x] 몬스터가 스크롤 컨테이너와 함께 이동
- [x] 스크롤 컨테이너 참조 전달 메커니즘

### 스테이지 시스템 통합
- [x] StageSystem에 StageScrollController 필드 추가
- [x] 스테이지 초기화 시 스크롤 컨트롤러 참조
- [x] 스테이지 정리 시 스크롤 중지

### 몬스터 사망 시 스크롤 이동 (스폰 위치 기반)
#### Phase 1: 기본 구조 추가
- [x] **[High]** StageScrollController에 스폰 위치 설정 필드 추가 (`_firstSpawnPosition`, `_spawnPositionInterval`)
- [x] **[High]** 현재 타겟 포인트 인덱스 변수 추가 (`_currentTargetPointIndex`)
- [x] **[High]** 최대 포인트 인덱스 설정 (`_maxPointIndex`, `SetMaxPointIndex()`)
- [x] **[High]** `GetSpawnPosition(int index)`: 인덱스 기반 스폰 위치 계산 메서드 구현
- [x] **[High]** `MoveToNextPoint()`: 다음 포인트로 이동하는 메서드 구현
- [x] **[High]** `MoveToPoint(int index)`: 특정 포인트로 이동하는 메서드 구현
- [x] **[High]** `ResetToFirstPoint()`: 첫 번째 포인트로 리셋하는 메서드 구현
- [x] **[High]** 부드러운 이동 구현 (DOTween 사용, Ease.OutQuad)
- [x] **[High]** 배경 레이어 재배치 (`RepositionBackgroundLayer()`)
- [x] **[High]** Tween 관리 및 정리 로직 구현 (`KillAllMoveTweens()`)

#### Phase 2: 몬스터 사망 이벤트 연동
- [x] **[High]** StageSystem의 `OnMonsterDeath()`에서 개별 몬스터 사망 시 스크롤 이동 호출
- [x] **[High]** 모든 몬스터 사망 체크는 유지 (다음 웨이브 진행용)
- [x] **[High]** 보스 모드에서는 스크롤 이동 비활성화 (`_isBossMode` 체크)
- [x] **[High]** 인덱스 범위 체크 (최대 포인트 인덱스 초과 방지)
- [x] **[High]** 스크롤 컨테이너 x값 업데이트 (y, z는 유지)

#### Phase 3: 웨이브 전환 처리
- [x] **[High]** `StartFirstWave()`에서 첫 번째 포인트로 스크롤 리셋
- [x] **[High]** `StartNextWave()`는 스크롤 리셋 없이 진행 (연속 웨이브)
- [x] **[High]** 웨이브 리셋 시에도 첫 번째 포인트로 리셋 (`StartWaveResetWithFade()`)
- [x] **[Medium]** 보스 모드 진입 시 스크롤 중지 (`StopScrolling()`)
- [x] **[Medium]** 보스 모드 종료 시 스크롤 재개 및 첫 포인트로 리셋 (웨이브 스폰 시 자동 리셋)

#### Phase 4: 이동 방식 개선
- [x] **[Low]** 부드러운 이동 방식 구현 (DOTween 사용)
- [x] **[Low]** 이동 속도 설정 가능 (`_moveToPointDuration`)
- [x] **[Low]** 이동 중 추가 이동 요청 처리 (기존 Tween 중지 후 새 이동)

#### Phase 5: 예외 처리 및 안정성
- [x] **[High]** 스크롤 컨테이너가 null인 경우 처리
- [x] **[High]** 인덱스가 범위를 벗어나는 경우 처리 (최대 포인트 인덱스 체크)
- [x] **[High]** 배경 레이어 배열 유효성 검사
- [x] **[Medium]** 스크롤 이동 시 로그 출력
- [ ] **[Medium]** 현재 타겟 포인트 인덱스 표시 (디버그용)
- [x] **[Low]** Inspector에서 현재 상태 확인 가능 (Odin Inspector FoldoutGroup 사용)

### 코드 품질 및 리팩토링
- [x] Tween 관리 로직 개선 (`KillAllMoveTweens()`)
- [x] 메서드 분리 및 책임 분리
  - `MoveToPoint()`: 포인트 이동 로직
  - `RepositionBackgroundLayer()`: 배경 레이어 재배치
  - `ResetInfiniteBackgroundLayerPosition()`: 배경 레이어 초기 위치 리셋
- [x] 성능 최적화
  - 카메라 정보 캐싱 (`_cachedCamera`, `_cachedCameraWidth`)
  - 배경 재배치 로직 최적화 (`FindExtremePiecePosition`, `RepositionPiece`)
- [x] Odin Inspector 통합 (FoldoutGroup, SuffixLabel)

## 구현 필요 항목

### 스크롤 제어 개선
- [ ] **[High]** 연속 스크롤 모드 추가 (Update 기반 지속 스크롤)
- [ ] **[Medium]** 스크롤 시작 시점 제어 (웨이브 시작 시 자동 시작)

### 무한 배경 개선
- [ ] **[Medium]** 배경 조각 3개 이상 지원 (더 부드러운 전환)
- [ ] **[Low]** 배경 너비 계산 정확도 개선
- [ ] **[Low]** 배경 재배치 시 부드러운 전환 효과

### 성능 최적화
- [ ] **[Medium]** 스크롤 업데이트 빈도 조절 (60fps 고정이 아닌 필요 시만)
- [ ] **[Medium]** 배경 레이어별 업데이트 우선순위 설정
- [ ] **[Low]** 배경 오브젝트 풀링 (재사용)

### 시각적 효과
- [ ] **[Medium]** 스크롤 속도에 따른 모션 블러 효과
- [ ] **[Low]** 배경 레이어별 색상/밝기 조절
- [ ] **[Low]** 스크롤 방향 전환 시 페이드 효과
- [ ] **[Low]** 배경 레이어별 깊이감 표현 (Z축 오프셋)

### 디버깅 및 개발 도구
- [ ] **[Medium]** 스크롤 속도 실시간 조절 (Inspector)
- [ ] **[Medium]** 스크롤 상태 표시 (디버그 UI)
- [ ] **[Low]** 스크롤 경로 시각화 (Gizmos)
- [ ] **[Low]** 배경 재배치 이벤트 로깅

## 기술 상세

### StageScrollController
스크롤링 시스템의 핵심 컨트롤러입니다.

**주요 기능:**
- 스크롤 컨테이너 이동 (몬스터 및 스테이지 요소)
- 무한 배경 레이어 스크롤링 (패럴랙스 효과)
- 무한 배경 레이어 관리
- 스크롤 제어 (중지)
- 스폰 위치 기반 포인트 이동 (부드러운 Tween 애니메이션)

**설정 파라미터 (Odin Inspector FoldoutGroup):**

**스크롤 설정:**
- `_scrollSpeed`: 초당 이동 속도 (기본: 2f) - 현재 미사용 (포인트 이동 방식 사용)
- `_scrollDirection`: 스크롤 방향 벡터 (기본: Vector3.left)

**컨테이너:**
- `_scrollContainer`: 스크롤되는 오브젝트들의 부모 Transform

**배경 레이어:**
- `_infiniteBackgroundLayers`: 무한 배경 레이어 배열

**포인트 이동:**
- `_firstSpawnPosition`: 첫 스폰 위치 (Vector3.zero면 초기 컨테이너 위치 사용)
- `_spawnPositionInterval`: 스폰 위치 간격 (기본: GameDefine.MONSTER_SPAWN_POSITION_PADDING)
- `_maxPointIndex`: 최대 포인트 인덱스 (기본: 10)
- `_moveToPointDuration`: 포인트 이동 애니메이션 시간 (기본: 0.5초)

**내부 상태:**
- `_currentTargetPointIndex`: 현재 타겟 포인트 인덱스
- `_initialScrollContainerPosition`: 초기 스크롤 컨테이너 위치
- `_moveTween`: 스크롤 컨테이너 이동 Tween
- `_backgroundTweens`: 배경 레이어 이동 Tween 리스트 (현재 미사용)

**주요 메서드:**
- `StopScrolling()`: 스크롤 중지 (모든 Tween 정리)
- `SetMaxPointIndex(int maxIndex)`: 최대 포인트 인덱스 설정
- `GetSpawnPosition(int index)`: 인덱스 기반 스폰 위치 계산
- `MoveToNextPoint()`: 다음 포인트로 부드럽게 이동
- `MoveToPoint(int index)`: 특정 포인트로 부드럽게 이동
- `ResetToFirstPoint()`: 첫 번째 포인트로 리셋

**성능 최적화:**
- Tween 이동 중에는 Update 기반 스크롤 스킵
- 배경 레이어 재배치 시 즉시 처리 (Tween 없이)

### InfiniteBackgroundLayer
무한 배경 스크롤링을 위한 레이어 클래스입니다.

**주요 기능:**
- 배경 조각 2개 이상을 나란히 배치
- 배경 너비 자동 계산
- 화면 밖 배경을 반대편으로 재배치
- 패럴랙스 속도 지원
- 카메라 정보 캐싱으로 성능 최적화

**설정 파라미터 (Odin Inspector FoldoutGroup):**

**배경 조각:**
- `_backgroundPieces`: 무한 스크롤을 위한 배경 조각 Transform 배열 (최소 2개)
- `_backgroundWidth`: 각 배경 조각의 너비 (0이면 자동 계산)

**스크롤 설정:**
- `_parallaxSpeed`: 패럴랙스 효과 속도 배율 (1.0 = 기본 속도)
- `_scrollDirection`: 배경 스크롤 방향 벡터

**내부 상태:**
- `_isInitialized`: 초기화 완료 여부
- `_cachedCamera`: 캐싱된 카메라 참조
- `_cachedCameraWidth`: 캐싱된 카메라 너비

**주요 메서드:**
- `Initialize()`: 배경 레이어 초기화 (너비 계산, 배치)
- `ResetPosition()`: 배경 조각들을 초기 위치로 재배치
- `CheckAndReposition()`: 화면 밖 배경 조각 재배치
- `SetScrollDirection(Vector3 direction)`: 스크롤 방향 설정

**배경 너비 자동 계산:**
1. SpriteRenderer의 bounds.size.x
2. RectTransform의 rect.width
3. Renderer의 bounds.size.x
4. 계산 실패 시 기본값 10f 사용

**성능 최적화:**
- 카메라 정보를 캐싱하여 매 프레임 `Camera.main` 호출 방지
- 배경 재배치 로직을 헬퍼 메서드로 분리하여 가독성 및 유지보수성 향상
- `FindExtremePiecePosition()`: 극단 위치 찾기 로직 통합
- `RepositionPiece()`: 배경 조각 재배치 로직 분리

### 스크롤 동작 흐름

```
Awake()
  ├─ 스크롤 컨테이너 자동 설정
  ├─ _backgroundTweens 초기화
  ├─ _initialScrollContainerPosition 저장
  └─ 무한 배경 레이어 초기화
      └─ 각 레이어의 Initialize() 호출
          ├─ 배경 너비 계산
          └─ 배경 조각 나란히 배치

Initialize() [StageSystem]
  ├─ SetMaxPointIndex() 호출 (스테이지 몬스터 수 기반)
  └─ StopScrolling() 호출

MoveToNextPoint()
  ├─ _currentTargetPointIndex 증가
  ├─ 최대 포인트 인덱스 체크
  └─ MoveToPoint(index)
      ├─ KillAllMoveTweens()
      ├─ 목표 위치 계산 (첫 스폰 위치 + 인덱스 * 간격)
      ├─ ScrollContainer Tween 이동 (DOMoveX)
      └─ RepositionBackgroundLayer()
          └─ 각 배경 레이어의 CheckAndReposition() 호출
              ├─ 카메라 정보 캐싱 업데이트
              └─ 화면 밖 배경 재배치
                  ├─ FindExtremePiecePosition()
                  └─ RepositionPiece()

ResetToFirstPoint()
  ├─ _currentTargetPointIndex = 0
  ├─ MoveToPoint(0)
  └─ ResetInfiniteBackgroundLayerPosition()
      └─ 각 배경 레이어의 ResetPosition() 호출
```

### Unity 씬 설정

**씬 구조:**
```
Stage (StageSystem)
├── ScrollContainer (StageScrollController)
│   ├── MonsterSpawner (MonsterCharacterSpawner)
│   │   └── [Monsters will spawn here]
│   └── StageElements
│       └── [기타 스테이지 오브젝트들]
└── InfiniteBackgroundLayers (배열)
    ├── [0] InfiniteBackgroundLayer
    │   ├── Background Pieces (배열)
    │   │   ├── [0] BackgroundPiece1
    │   │   └── [1] BackgroundPiece2
    │   ├── Parallax Speed: 1.0
    │   └── Background Width: 0 (자동)
    └── [1] InfiniteBackgroundLayer (다른 레이어)
```

**Inspector 설정:**

1. **StageSystem**
   - `Scroll Controller`: StageScrollController 할당

2. **StageScrollController**
   - **스크롤 설정**
     - `Scroll Speed`: 스크롤 속도 (현재 미사용)
     - `Scroll Direction`: 스크롤 방향
   - **컨테이너**
     - `Scroll Container`: 몬스터 컨테이너 Transform
   - **배경 레이어**
     - `Infinite Background Layers`: 무한 배경 레이어 배열
   - **포인트 이동**
     - `First Spawn Position`: 첫 스폰 위치 (Vector3.zero면 초기 위치 사용)
     - `Spawn Position Interval`: 스폰 위치 간격
     - `Max Point Index`: 최대 포인트 인덱스
     - `Move To Point Duration`: 포인트 이동 애니메이션 시간

3. **InfiniteBackgroundLayer** (각 레이어)
   - **배경 조각**
     - `Background Pieces`: 최소 2개의 배경 Transform
     - `Background Width`: 0이면 자동 계산
   - **스크롤 설정**
     - `Parallax Speed`: 패럴랙스 속도
     - `Scroll Direction`: 배경 스크롤 방향

## 사용 예시

### 스크롤 중지
```csharp
// StageSystem에서
_scrollController?.StopScrolling();
```

### 최대 포인트 인덱스 설정
```csharp
// StageSystem 초기화 시
_scrollController?.SetMaxPointIndex(_currentStageAsset.GetStageMonsterCount());
```

### 스폰 위치 계산
```csharp
// MonsterCharacterSpawner에서
Vector3 spawnPos = _scrollController?.GetSpawnPosition(i);
```

### 몬스터 사망 시 스크롤 이동
```csharp
// StageSystem에서
private void OnMonsterDeath(Character character)
{
    if (_isBossMode)
    {
        return; // 보스 모드에서는 스크롤 이동 비활성화
    }
    
    // 몬스터 사망 시 스크롤 이동 (부드러운 Tween 애니메이션)
    _scrollController?.MoveToNextPoint();
    
    // 모든 몬스터 사망 체크 (기존 로직 유지)
    if (_monsterSpawner == null || !_monsterSpawner.IsAllMonstersDefeated)
    {
        return;
    }
    
    // 다음 웨이브 진행...
}
```

### 웨이브 시작 시 스크롤 리셋
```csharp
// 첫 웨이브 시작 시 스크롤 리셋
private void StartFirstWave(Data.Game.VProfile profile)
{
    // 첫 웨이브 시작 시 스크롤 리셋
    _scrollController?.ResetToFirstPoint();
    
    // 몬스터 스폰...
}

// 웨이브 리셋 시 스크롤 리셋
private IEnumerator StartWaveResetWithFade()
{
    // ... 페이드 처리 ...
    
    // 웨이브 리셋 시 스크롤 리셋
    _scrollController?.ResetToFirstPoint();
    
    // 몬스터 스폰...
}
```

## 참고 사항

- 플레이어는 World Space에서 고정 위치 유지
- 몬스터는 스크롤 컨테이너 하위에 스폰되어 함께 이동
- 배경은 레이어별로 다른 속도로 이동하여 깊이감 표현 (패럴랙스 효과)
- 무한 배경은 화면 밖으로 나간 배경을 재사용하여 메모리 효율적
- 카메라는 플레이어 중심으로 고정 (TurnBasedCameraController 사용)
- **몬스터 사망 시 스크롤 이동**: 몬스터가 죽을 때마다 스크롤 컨테이너가 다음 포인트로 부드럽게 이동하여 진행감 제공
- **스크롤 컨테이너 기준점**: `_firstSpawnPosition` 또는 초기 컨테이너 위치를 기준으로 스폰 위치 계산
- **스폰 위치 계산**: `GetSpawnPosition(int index)` 메서드로 인덱스 기반 스폰 위치 계산 (첫 위치 + 인덱스 * 간격)
- **부드러운 이동**: DOTween을 사용하여 부드러운 포인트 이동 애니메이션 제공 (Ease.OutQuad)
- **배경 레이어 재배치**: 포인트 이동 시 배경 레이어를 즉시 재배치하여 무한 스크롤 유지
- **최대 포인트 제한**: `_maxPointIndex`로 최대 포인트 인덱스를 제한하여 범위 초과 방지
- **보스 모드 처리**: 보스 모드에서는 스크롤 이동 비활성화 (`_isBossMode` 체크)
- **성능 최적화**: 카메라 정보 캐싱으로 불필요한 계산 최소화
