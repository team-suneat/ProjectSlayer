using System;
using System.Collections;
using System.Collections.Generic;
using TeamSuneat;
using TeamSuneat.Data;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat.Stage
{
    public class BossModeHandler : XBehaviour
    {
        private MonsterCharacterSpawner _monsterSpawner;
        private StageAsset _currentStageAsset;
        private AreaAsset _currentAreaAsset;
        private Transform _spawnTransform;

        private bool _isBossMode = false;
        private int _originalWave = 0;
        private Coroutine _enterBossModeFadeCoroutine;
        private Coroutine _exitBossModeFadeCoroutine;

        private Action _stopNextWaveDelay;
        private Action _stopWaveResetFade;
        private Action _setPlayerTargetToFirstMonster;
        private Action<int> _onWaveRestore;
        private Action<int, int> _onStageProgressUpdate;

        public bool IsBossMode => _isBossMode;

        public float BossModeRemainingTime
        {
            get
            {
                if (!_isBossMode)
                {
                    return 0f;
                }
                return GameTimerManager.Instance.GetRemainingTime(GameTimerLabels.BossMode);
            }
        }

        public float BossModeElapsedTime
        {
            get
            {
                if (!_isBossMode)
                {
                    return 0f;
                }
                return GameTimerManager.Instance.GetElapsedTime(GameTimerLabels.BossMode);
            }
        }

        public void Initialize(
            MonsterCharacterSpawner monsterSpawner,
            StageAsset stageAsset,
            AreaAsset areaAsset,
            Transform spawnTransform,
            System.Action stopNextWaveDelay,
            System.Action stopWaveResetFade,
            System.Action setPlayerTargetToFirstMonster,
            System.Action<int> onWaveRestore,
            System.Action<int, int> onStageProgressUpdate)
        {
            _monsterSpawner = monsterSpawner;
            _currentStageAsset = stageAsset;
            _currentAreaAsset = areaAsset;
            _spawnTransform = spawnTransform;
            _stopNextWaveDelay = stopNextWaveDelay;
            _stopWaveResetFade = stopWaveResetFade;
            _setPlayerTargetToFirstMonster = setPlayerTargetToFirstMonster;
            _onWaveRestore = onWaveRestore;
            _onStageProgressUpdate = onStageProgressUpdate;
        }

        public void Cleanup()
        {
            StopBossModeTimer();
            StopEnterBossModeFade();
            StopExitBossModeFade();
            _isBossMode = false;
        }

        public void EnterBossMode(int originalWave)
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
            _stopNextWaveDelay?.Invoke();
            _stopWaveResetFade?.Invoke();

            _isBossMode = true;
            _originalWave = originalWave;
            profile.Stage.ResetCurrentWave();

            // 모든 몬스터 정리 (보스 모드 플래그 설정 후)
            CharacterManager.Instance.ClearMonsterAndAlliance();

            StopEnterBossModeFade();
            _enterBossModeFadeCoroutine = StartCoroutine(EnterBossModeWithFade());
        }

        public void ExitBossMode(bool isBossDefeated = false)
        {
            if (!_isBossMode)
            {
                return;
            }

            StopBossModeTimer();
            _isBossMode = false;

            // HUD 그룹 전환 및 게이지 비활성화
            GlobalEvent.Send(GlobalEventType.BOSS_MODE_EXITED);

            CharacterManager.Instance.ClearMonsterAndAlliance();

            StopExitBossModeFade();
            _exitBossModeFadeCoroutine = StartCoroutine(ExitBossModeWithFade(isBossDefeated));
        }

        private IEnumerator EnterBossModeWithFade()
        {
            if (UIManager.Instance?.ScreenFader != null)
            {
                UIManager.Instance.ScreenFader.FadeInOut(Color.black, 0.5f, 0, 0.3f);
                yield return new WaitForSeconds(0.5f + 0.3f);
            }

            // HUD 그룹 전환 및 게이지 활성화
            GlobalEvent.Send(GlobalEventType.BOSS_MODE_ENTERED);

            SpawnBoss();
            StartBossModeTimer();

            if (UIManager.Instance?.ScreenFader != null)
            {
                yield return new WaitForSeconds(0.5f);
            }

            Log.Info(LogTags.Stage, "보스 모드 진입. 원래 웨이브: {0}", _originalWave);
            _enterBossModeFadeCoroutine = null;
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
                        _setPlayerTargetToFirstMonster?.Invoke();
                    }

                    _onStageProgressUpdate?.Invoke(waveToSpawn + 1, _currentStageAsset.WaveCount);
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

            Vector3 spawnPosition = _spawnTransform != null ? _spawnTransform.position : transform.position;
            if (_monsterSpawner != null && _monsterSpawner.SpawnPositionGroup != null)
            {
                List<Vector3> positions = _monsterSpawner.SpawnPositionGroup.GetPositions(spawnPosition, 1);
                if (positions != null && positions.Count > 0)
                {
                    spawnPosition = positions[0];
                }
            }

            MonsterCharacter boss = _monsterSpawner?.SpawnMonster(bossName, spawnPosition);
            if (boss != null)
            {
                _setPlayerTargetToFirstMonster?.Invoke();

                // 보스 체력 게이지 바인딩 (게이지는 이미 활성화됨)
                UIManager.Instance?.HUDManager?.OnBossSpawned(boss);

                Log.Info(LogTags.Stage, "보스 스폰 완료: {0}", bossName);
            }
        }

        private void StartBossModeTimer()
        {
            StopBossModeTimer();
            GameTimerManager.Instance.StartTimer(
                GameTimerLabels.BossMode,
                GameDefine.BOSS_MODE_TIME_LIMIT,
                onExpired: () =>
                {
                    if (_isBossMode)
                    {
                        Log.Info(LogTags.Stage, "보스 모드 시간 제한 도달. 원래 스테이지로 복귀합니다.");
                        ExitBossMode();
                    }
                }
            );
        }

        private void StopBossModeTimer()
        {
            GameTimerManager.Instance.StopTimer(GameTimerLabels.BossMode);
            Log.Progress(LogTags.Stage, "보스 모드 타이머 종료");
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

            Log.Progress(LogTags.Stage, "보스 모드 종료 페이드 종료");
        }

        private Data.Game.VProfile GetSelectedProfile()
        {
            return GameApp.GetSelectedProfile();
        }
    }
}