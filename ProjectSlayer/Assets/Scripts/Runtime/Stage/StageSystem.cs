using System.Collections;
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
        private float _nextWaveDelay = 3f;

        private StageAsset _currentStageAsset;
        private AreaAsset _currentAreaAsset;

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

            Log.Info(LogTags.Stage, "스테이지 초기화 완료: {0}", Name);
            StartCoroutine(StartStageFlow());
        }

        public void CleanupStage()
        {
            UnregisterGlobalEvents();
            _monsterSpawner?.CleanupAllMonsters();
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

            _monsterSpawner.Initialize(_currentStageAsset, _currentAreaAsset);
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
            GlobalEvent<Character>.Register(GlobalEventType.BOSS_CHARACTER_DEATH, OnMonsterDeath);
        }

        private void UnregisterGlobalEvents()
        {
            GlobalEvent<Character>.Unregister(GlobalEventType.MONSTER_CHARACTER_DEATH, OnMonsterDeath);
            GlobalEvent<Character>.Unregister(GlobalEventType.BOSS_CHARACTER_DEATH, OnMonsterDeath);
        }

        private void OnMonsterDeath(Character character)
        {
            if (_monsterSpawner == null || !_monsterSpawner.IsAllMonstersDefeated)
            {
                return;
            }

            StartCoroutine(StartNextWaveWithDelay());
        }

        private IEnumerator StartNextWaveWithDelay()
        {
            yield return new WaitForSeconds(_nextWaveDelay);

            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage == null || _currentStageAsset == null)
            {
                yield break;
            }

            int nextWaveIndex = profile.Stage.CurrentWave + 1;
            if (nextWaveIndex >= _currentStageAsset.WaveCount)
            {
                yield return StartWaveResetWithFade();
            }
            else
            {
                StartNextWave();
            }
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

                    if (_monsterSpawner != null)
                    {
                        _monsterSpawner.SpawnWave(0);
                        SetPlayerTargetToFirstMonster();
                    }
                }
            }
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
    }
}