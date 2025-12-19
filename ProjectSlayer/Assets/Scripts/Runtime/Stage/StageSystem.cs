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

        private StageAsset _currentStageAsset;
        private AreaAsset _currentAreaAsset;

        // private StageData _currentStageData;

        public override void AutoSetting()
        {
            base.AutoSetting();

            NameString = Name.ToString();
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

        public override void AutoNaming()
        {
            SetGameObjectName(NameString);
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
            if (_monsterSpawner != null)
            {
                _monsterSpawner.Initialize(_currentStageAsset, _currentAreaAsset);
            }
            else
            {
                Log.Warning(LogTags.Stage, "MonsterCharacterSpawner가 설정되지 않았습니다.");
            }
        }

        private void RegisterCurrentStage()
        {
            if (GameApp.Instance != null && GameApp.Instance.gameManager != null)
            {
                GameApp.Instance.gameManager.CurrentStageSystem = this;
            }
        }

        public void CleanupStage()
        {
            UnregisterGlobalEvents();
            _monsterSpawner?.CleanupAllMonsters();
            // _currentStageData = null;
        }

        private UIStageTitleNotice SpawnStageTitleNotice()
        {
            return ResourcesManager.SpawnStageTitleNotice(Name);
        }

        private IEnumerator StartStageFlow()
        {
            UIStageTitleNotice stageNotice = SpawnStageTitleNotice();
            if (stageNotice != null)
            {
                bool isCompleted = false;
                stageNotice.OnCompleted += () => isCompleted = true;

                while (!isCompleted)
                {
                    yield return null;
                }
            }

            Data.Game.VProfile profile = GameApp.GetSelectedProfile();
            if (profile != null && profile.Stage != null)
            {
                int currentWave = profile.Stage.CurrentWave;
                if (currentWave <= 0)
                {
                    currentWave = 1;
                    profile.Stage.ResetCurrentWave();
                }

                if (_monsterSpawner != null)
                {
                    _monsterSpawner.SpawnWave(currentWave);
                }
            }
        }

        private void RegisterGlobalEvents()
        {
            GlobalEvent.Register(GlobalEventType.MONSTER_CHARACTER_DEATH, OnMonsterDeath);
            GlobalEvent.Register(GlobalEventType.BOSS_CHARACTER_DEATH, OnMonsterDeath);
        }

        private void UnregisterGlobalEvents()
        {
            GlobalEvent.Unregister(GlobalEventType.MONSTER_CHARACTER_DEATH, OnMonsterDeath);
            GlobalEvent.Unregister(GlobalEventType.BOSS_CHARACTER_DEATH, OnMonsterDeath);
        }

        private void OnMonsterDeath()
        {
            if (_monsterSpawner != null && _monsterSpawner.IsAllMonstersDefeated)
            {
                StartNextWave();
            }
        }

        private void StartNextWave()
        {
            Data.Game.VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null || profile.Stage == null)
            {
                return;
            }

            profile.Stage.AddCurrentWave();
            if (_currentStageAsset != null && profile.Stage.CurrentWave > _currentStageAsset.WaveCount)
            {
                Log.Info(LogTags.Stage, "모든 웨이브 완료: {0}", Name);
                return;
            }

            if (_monsterSpawner != null)
            {
                _monsterSpawner.SpawnWave(profile.Stage.CurrentWave);
            }
        }
    }
}