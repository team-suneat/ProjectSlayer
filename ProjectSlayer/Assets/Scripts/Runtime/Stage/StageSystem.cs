using System.Collections;
using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat.Stage
{
    public class StageSystem : XBehaviour
    {
        public StageNames Name;
        public string NameString;

        [SerializeField]
        private MonsterCharacterSpawner _monsterSpawner;

        [SerializeField]
        private StageScrollController _scrollController;

        [SerializeField]
        private float _nextWaveDelay = 3f;

        private StageAsset _currentStageAsset;
        private AreaAsset _currentAreaAsset;

        private bool _isBossMode = false;
        private int _originalWave = 0;
        private Coroutine _bossModeTimerCoroutine;
        private Coroutine _stageFlowCoroutine;
        private Coroutine _nextWaveDelayCoroutine;
        private Coroutine _waveResetFadeCoroutine;
        private Coroutine _enterBossModeFadeCoroutine;
        private Coroutine _exitBossModeFadeCoroutine;
        private const float BOSS_MODE_TIME_LIMIT = 30f;

        public override void AutoSetting()
        {
            base.AutoSetting();
            NameString = Name.ToString();
        }

        public override void AutoNaming()
        {
            SetGameObjectName(NameString);
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

        public void Initialize()
        {
            LoadStageData();
            InitializeMonster();
            RegisterCurrentStage();
            RegisterGlobalEvents();

            if (_scrollController != null)
            {
                _scrollController.SetMaxPointIndex(_currentStageAsset.GetStageMonsterCount());
                _scrollController.StopScrolling();
            }

            Log.Info(LogTags.Stage, "스테이지 초기화 완료: {0}", Name);
            _stageFlowCoroutine = StartCoroutine(StartStageFlow());
        }

        public void CleanupStage()
        {
            StopBossModeTimer();
            StopStageFlow();
            StopNextWaveDelay();
            StopWaveResetFade();
            StopEnterBossModeFade();
            StopExitBossModeFade();
            UnregisterGlobalEvents();
            _monsterSpawner?.CleanupAllMonsters();
            _scrollController?.StopScrolling();

            _isBossMode = false;
        }

        private void LoadStageData()
        {
            _currentStageAsset = ScriptableDataManager.Instance.FindStage(Name);
            if (_currentStageAsset == null)
            {
                Log.Warning(LogTags.Stage, "스테이지 에셋을 찾을 수 없습니다: {0}", Name);
                return;
            }

            _currentAreaAsset = ScriptableDataManager.Instance.FindArea(_currentStageAsset.AreaName);
            if (_currentAreaAsset == null)
            {
                Log.Warning(LogTags.Stage, "지역 에셋을 찾을 수 없습니다: {0}", _currentStageAsset.AreaName);
                return;
            }
        }

        private void InitializeMonster()
        {
            if (_monsterSpawner == null)
            {
                Log.Warning(LogTags.Stage, "MonsterCharacterSpawner가 설정되지 않았습니다.");
                return;
            }

            Transform scrollContainer = _scrollController != null ? _scrollController.ScrollContainer : null;
            _monsterSpawner.Initialize(_currentStageAsset, _currentAreaAsset, scrollContainer, _scrollController);
        }

        private void RegisterCurrentStage()
        {
            if (GameApp.Instance?.gameManager == null)
            {
                return;
            }

            GameApp.Instance.gameManager.CurrentStageSystem = this;
        }

        private IEnumerator StartStageFlow()
        {
            yield return WaitForStageTitleNotice();

            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage == null)
            {
                GlobalEvent<StageNames>.Send(GlobalEventType.STAGE_SPAWNED, Name);
                yield break;
            }

            StartFirstWave(profile);
            GlobalEvent<StageNames>.Send(GlobalEventType.STAGE_SPAWNED, Name);
        }

        private IEnumerator WaitForStageTitleNotice()
        {
            UIStageTitleNotice stageNotice = SpawnStageTitleNotice();
            if (stageNotice == null)
            {
                yield break;
            }

            bool isCompleted = false;
            stageNotice.OnCompleted += () => isCompleted = true;

            while (!isCompleted)
            {
                yield return null;
            }
        }

        private void StartFirstWave(Data.Game.VProfile profile)
        {
            if (_currentStageAsset == null)
            {
                Log.Warning(LogTags.Stage, "스테이지 에셋이 없어 첫 웨이브를 시작할 수 없습니다.");
                return;
            }

            int currentWave = profile.Stage.CurrentWave;
            if (currentWave < 0 || currentWave >= _currentStageAsset.WaveCount)
            {
                currentWave = 0;
                profile.Stage.ResetCurrentWave();
            }

            // 첫 웨이브 시작 시 스크롤 리셋
            _scrollController?.ResetToFirstPoint();

            if (_monsterSpawner != null)
            {
                _monsterSpawner.SpawnWave(currentWave);
                SetPlayerTargetToFirstMonster();
            }

            UIManager.Instance.HUDManager.StageProgressGauge.SetProgress(currentWave + 1, _currentStageAsset.WaveCount);
        }

        private void RegisterGlobalEvents()
        {
            GlobalEvent<Character>.Register(GlobalEventType.MONSTER_CHARACTER_DEATH, OnMonsterDeath);
            GlobalEvent<Character>.Register(GlobalEventType.BOSS_CHARACTER_DEATH, OnBossDeath);
            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_DEATH, OnPlayerDeath);
        }

        private void UnregisterGlobalEvents()
        {
            GlobalEvent<Character>.Unregister(GlobalEventType.MONSTER_CHARACTER_DEATH, OnMonsterDeath);
            GlobalEvent<Character>.Unregister(GlobalEventType.BOSS_CHARACTER_DEATH, OnBossDeath);
            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_DEATH, OnPlayerDeath);
        }

        private void OnMonsterDeath(Character character)
        {
            if (_isBossMode)
            {
                return;
            }

            // 개별 몬스터 사망 시 스크롤 이동
            _scrollController?.MoveToNextPoint();

            // 모든 몬스터 사망 체크 (기존 로직 유지)
            if (_monsterSpawner == null || !_monsterSpawner.IsAllMonstersDefeated)
            {
                return;
            }

            StopNextWaveDelay();
            _nextWaveDelayCoroutine = StartCoroutine(StartNextWaveWithDelay());
        }

        // OnBossDeath() 수정 - 보스만 확인하도록
        private void OnBossDeath(Character character)
        {
            if (!_isBossMode)
            {
                return;
            }

            // 보스만 확인 (일반 몬스터는 무시)
            if (character == null || !character.IsBoss)
            {
                return;
            }

            // 보스가 죽었는지 확인
            if (_monsterSpawner?.SpawnedMonsters == null)
            {
                return;
            }

            // 스폰된 몬스터 중 보스만 확인
            bool isBossDefeated = true;
            for (int i = 0; i < _monsterSpawner.SpawnedMonsters.Count; i++)
            {
                MonsterCharacter monster = _monsterSpawner.SpawnedMonsters[i];
                if (monster != null && monster.IsAlive && monster.IsBoss)
                {
                    isBossDefeated = false;
                    break;
                }
            }

            if (!isBossDefeated)
            {
                return;
            }

            Log.Info(LogTags.Stage, "보스 처치 완료. 다음 스테이지로 진행합니다.");
            ExitBossMode(true);
        }

        private void OnPlayerDeath()
        {
            if (!_isBossMode)
            {
                return;
            }

            Log.Info(LogTags.Stage, "플레이어 사망. 보스 모드 종료.");
            ExitBossMode();
        }

        private IEnumerator StartNextWaveWithDelay()
        {
            yield return new WaitForSeconds(_nextWaveDelay);

            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage == null || _currentStageAsset == null)
            {
                _nextWaveDelayCoroutine = null;
                yield break;
            }

            int nextWaveIndex = profile.Stage.CurrentWave + 1;
            if (nextWaveIndex >= _currentStageAsset.WaveCount)
            {
                StopWaveResetFade();
                _waveResetFadeCoroutine = StartCoroutine(StartWaveResetWithFade());
            }
            else
            {
                StartNextWave();
            }

            _nextWaveDelayCoroutine = null;
        }

        private IEnumerator StartWaveResetWithFade()
        {
            Log.Info(LogTags.Stage, "모든 웨이브 완료: {0}. 첫 웨이브부터 반복합니다.", Name);

            if (UIManager.Instance?.ScreenFader != null)
            {
                // 페이드 인/아웃 효과 적용
                UIManager.Instance.ScreenFader.FadeInOut(Color.black, 0.5f, 0, 0.3f);

                // 페이드 인 완료 대기 (페이드 인 시간 + 유지 시간)
                yield return new WaitForSeconds(0.5f + 0.3f);

                // 웨이브 리셋 및 첫 웨이브 시작
                Data.Game.VProfile profile = GetSelectedProfile();
                if (profile?.Stage != null)
                {
                    profile.Stage.CurrentWave = 0;
                    UIManager.Instance.HUDManager.StageProgressGauge.SetProgress(1, _currentStageAsset.WaveCount);

                    // 웨이브 리셋 시 스크롤 리셋
                    _scrollController?.ResetToFirstPoint();

                    if (_monsterSpawner != null)
                    {
                        _monsterSpawner.SpawnWave(0);
                        SetPlayerTargetToFirstMonster();
                    }
                }

                // 페이드 아웃 완료 대기
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                // ScreenFader가 없을 경우 일반 처리
                Data.Game.VProfile profile = GetSelectedProfile();
                if (profile?.Stage != null)
                {
                    profile.Stage.CurrentWave = 0;
                    UIManager.Instance.HUDManager.StageProgressGauge.SetProgress(1, _currentStageAsset.WaveCount);

                    // 웨이브 리셋 시 스크롤 리셋
                    _scrollController?.ResetToFirstPoint();

                    if (_monsterSpawner != null)
                    {
                        _monsterSpawner.SpawnWave(0);
                        SetPlayerTargetToFirstMonster();
                    }
                }
            }

            _waveResetFadeCoroutine = null;
        }

        private void StartNextWave()
        {
            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage == null || _currentStageAsset == null)
            {
                return;
            }

            int nextWaveIndex = profile.Stage.CurrentWave + 1;
            profile.Stage.CurrentWave = nextWaveIndex;

            UIManager.Instance.HUDManager.StageProgressGauge.SetProgress(nextWaveIndex + 1, _currentStageAsset.WaveCount);
            if (_monsterSpawner != null)
            {
                _monsterSpawner.SpawnWave(profile.Stage.CurrentWave);
                SetPlayerTargetToFirstMonster();
            }
        }

        private void SetPlayerTargetToFirstMonster()
        {
            if (_monsterSpawner?.SpawnedMonsters == null || _monsterSpawner.SpawnedMonsters.Count == 0)
            {
                Log.Warning(LogTags.Stage, "스폰된 몬스터가 없어 플레이어 타겟을 설정할 수 없습니다.");
                return;
            }

            PlayerCharacter player = CharacterManager.Instance.Player;
            if (player == null)
            {
                Log.Warning(LogTags.Stage, "플레이어가 없어 타겟을 설정할 수 없습니다.");
                return;
            }

            MonsterCharacter firstMonster = _monsterSpawner.SpawnedMonsters[0];
            if (firstMonster == null || !firstMonster.IsAlive)
            {
                Log.Warning(LogTags.Stage, "첫 번째 몬스터가 유효하지 않아 타겟을 설정할 수 없습니다.");
                return;
            }

            player.SetTarget(firstMonster);
            Log.Info(LogTags.Stage, "플레이어 타겟을 첫 번째 몬스터로 설정했습니다: {0}", firstMonster.Name.ToLogString());
        }

        private UIStageTitleNotice SpawnStageTitleNotice()
        {
            return ResourcesManager.SpawnStageTitleNotice(Name);
        }

        private Data.Game.VProfile GetSelectedProfile()
        {
            return GameApp.GetSelectedProfile();
        }

        public void EnterBossMode()
        {
            if (_isBossMode)
            {
                Log.Warning(LogTags.Stage, "이미 보스 모드입니다.");
                return;
            }

            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage == null || _currentStageAsset == null)
            {
                Log.Warning(LogTags.Stage, "프로필 또는 스테이지 에셋이 없어 보스 모드에 진입할 수 없습니다.");
                return;
            }

            // 보스 모드 진입 전 진행 중인 코루틴 중지
            StopNextWaveDelay();
            StopWaveResetFade();

            _isBossMode = true;
            _originalWave = profile.Stage.CurrentWave;
            profile.Stage.ResetCurrentWave();

            // 모든 몬스터 정리 (보스 모드 플래그 설정 후)
            _monsterSpawner?.CleanupAllMonsters();

            StopEnterBossModeFade();
            _enterBossModeFadeCoroutine = StartCoroutine(EnterBossModeWithFade());
        }

        private IEnumerator EnterBossModeWithFade()
        {
            if (UIManager.Instance?.ScreenFader != null)
            {
                UIManager.Instance.ScreenFader.FadeInOut(Color.black, 0.5f, 0, 0.3f);
                yield return new WaitForSeconds(0.5f + 0.3f);
            }

            SpawnBoss();
            StartBossModeTimer();

            if (UIManager.Instance?.ScreenFader != null)
            {
                yield return new WaitForSeconds(0.5f);
            }

            Log.Info(LogTags.Stage, "보스 모드 진입. 원래 웨이브: {0}", _originalWave);
            _enterBossModeFadeCoroutine = null;
        }

        private void SpawnBoss()
        {
            if (_currentStageAsset == null || _currentAreaAsset == null)
            {
                Log.Error(LogTags.Stage, "스테이지 또는 지역 에셋이 설정되지 않았습니다.");
                return;
            }

            int bossIndex = _currentStageAsset.BossMonsterIndex;
            if (bossIndex < 0 || bossIndex >= _currentAreaAsset.BossMonsters.Length)
            {
                Log.Error(LogTags.Stage, "보스 몬스터 인덱스가 유효하지 않습니다: {0}", bossIndex);
                return;
            }

            CharacterNames bossName = _currentAreaAsset.BossMonsters[bossIndex];
            if (bossName == CharacterNames.None)
            {
                Log.Error(LogTags.Stage, "보스 몬스터 이름이 유효하지 않습니다.");
                return;
            }

            Vector3 spawnPosition = transform.position;

            MonsterCharacter boss = _monsterSpawner?.SpawnMonster(bossName, spawnPosition);
            if (boss != null)
            {
                SetPlayerTargetToFirstMonster();
                Log.Info(LogTags.Stage, "보스 스폰 완료: {0}", bossName);
            }
        }

        private void ExitBossMode(bool isBossDefeated = false)
        {
            if (!_isBossMode)
            {
                return;
            }

            StopBossModeTimer();
            _isBossMode = false;

            _monsterSpawner?.CleanupAllMonsters();

            StopExitBossModeFade();
            _exitBossModeFadeCoroutine = StartCoroutine(ExitBossModeWithFade(isBossDefeated));
        }

        private IEnumerator ExitBossModeWithFade(bool isBossDefeated = false)
        {
            if (UIManager.Instance?.ScreenFader != null)
            {
                UIManager.Instance.ScreenFader.FadeInOut(Color.black, 0.5f, 0, 0.3f);
                yield return new WaitForSeconds(0.5f + 0.3f);
            }

            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage != null && _currentStageAsset != null)
            {
                if (isBossDefeated)
                {
                    // 보스 처치 시: 스테이지 1 증가, 웨이브 0으로 초기화
                    StageNames nextStage = (StageNames)((int)profile.Stage.CurrentStage + 1);
                    if (nextStage != StageNames.None)
                    {
                        profile.Stage.SelectStage(nextStage);
                        profile.Stage.UpdateMaxReachedStage(nextStage);
                    }
                    profile.Stage.ResetCurrentWave();

                    Log.Info(LogTags.Stage, "보스 처치 완료. 스테이지 증가: {0}, 웨이브 초기화", nextStage);
                }
                else
                {
                    // 보스 처치 실패 시: 원래 웨이브로 복귀
                    profile.Stage.CurrentWave = _originalWave;
                }

                if (_monsterSpawner != null)
                {
                    int waveToSpawn = isBossDefeated ? 0 : _originalWave;
                    _monsterSpawner.SpawnWave(waveToSpawn);

                    // 플레이어가 살아있을 때만 타겟 설정
                    PlayerCharacter player = CharacterManager.Instance.Player;
                    if (player != null && player.IsAlive)
                    {
                        SetPlayerTargetToFirstMonster();
                    }

                    UIManager.Instance.HUDManager.StageProgressGauge.SetProgress(waveToSpawn + 1, _currentStageAsset.WaveCount);
                }
            }

            if (UIManager.Instance?.ScreenFader != null)
            {
                yield return new WaitForSeconds(0.5f);
            }

            if (isBossDefeated)
            {
                Log.Info(LogTags.Stage, "보스 모드 종료. 다음 스테이지로 진행");
            }
            else
            {
                Log.Info(LogTags.Stage, "보스 모드 종료. 웨이브 {0}로 복귀", _originalWave);
            }

            _exitBossModeFadeCoroutine = null;
        }

        private void StartBossModeTimer()
        {
            StopBossModeTimer();
            _bossModeTimerCoroutine = StartCoroutine(BossModeTimerCoroutine());
        }

        private void StopBossModeTimer()
        {
            if (_bossModeTimerCoroutine != null)
            {
                StopCoroutine(_bossModeTimerCoroutine);
                _bossModeTimerCoroutine = null;
            }

            Log.Progress(LogTags.Stage, "보스 모드 타이머 종료");
        }

        private void StopStageFlow()
        {
            if (_stageFlowCoroutine != null)
            {
                StopCoroutine(_stageFlowCoroutine);
                _stageFlowCoroutine = null;
            }

            Log.Progress(LogTags.Stage, "스테이지 흐름 종료");
        }

        private void StopNextWaveDelay()
        {
            if (_nextWaveDelayCoroutine != null)
            {
                StopCoroutine(_nextWaveDelayCoroutine);
                _nextWaveDelayCoroutine = null;
            }

            Log.Progress(LogTags.Stage, "웨이브 딜레이 종료");
        }

        private void StopWaveResetFade()
        {
            if (_waveResetFadeCoroutine != null)
            {
                StopCoroutine(_waveResetFadeCoroutine);
                _waveResetFadeCoroutine = null;
            }

            Log.Progress(LogTags.Stage, "웨이브 리셋 페이드 종료");
        }

        private void StopEnterBossModeFade()
        {
            if (_enterBossModeFadeCoroutine != null)
            {
                StopCoroutine(_enterBossModeFadeCoroutine);
                _enterBossModeFadeCoroutine = null;
            }

            Log.Progress(LogTags.Stage, "보스 모드 진입 페이드 종료");
        }

        private void StopExitBossModeFade()
        {
            if (_exitBossModeFadeCoroutine != null)
            {
                StopCoroutine(_exitBossModeFadeCoroutine);
                _exitBossModeFadeCoroutine = null;
            }

            Log.Progress(LogTags.Stage, "보스 모드 종료 페이드 종종료");
        }

        private IEnumerator BossModeTimerCoroutine()
        {
            yield return new WaitForSeconds(BOSS_MODE_TIME_LIMIT);

            if (_isBossMode)
            {
                Log.Info(LogTags.Stage, "보스 모드 시간 제한 도달. 원래 스테이지로 복귀합니다.");
                ExitBossMode();
            }
        }
    }
}