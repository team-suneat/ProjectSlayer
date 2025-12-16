using System.Collections;
using TeamSuneat.Data;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    public class StageSystem : XBehaviour
    {
        public StageNames Name;
        public string NameString;

        [SerializeField]
        [Tooltip("몬스터 개별 생성 간격(초)")]
        private float _monsterSpawnInterval = 0.1f;

        private StageData _currentStageData;
        private int _currentWaveNumber;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();
            NameString = Name.ToString();
        }

        private void OnValidate()
        {
            _ = EnumEx.ConvertTo(ref Name, NameString);
        }

        public override void AutoNaming()
        {
            SetGameObjectName(NameString);
        }

        public void Initialize()
        {
            // StageData 로드
            _currentStageData = JsonDataManager.FindStageDataClone(Name);
            if (_currentStageData == null)
            {
                Log.Error(LogTags.Stage, "스테이지 데이터를 찾을 수 없습니다: {0}", Name);
                return;
            }

            Log.Progress(LogTags.Stage, "1~10웨이브 초기 세팅을 시작합니다.");

            StartCoroutine(SetupInitialWavesCoroutine(Name));

            if (GameApp.Instance != null && GameApp.Instance.gameManager != null)
            {
                GameApp.Instance.gameManager.CurrentStageSystem = this;
            }

            SpawnPlayerCharacter();

            Log.Info(LogTags.Stage, "스테이지 초기화 완료: {0}", Name);

            StartCoroutine(StartStageFlow());
        }

        public void StartStage()
        {
            _currentWaveNumber = 1;
            Log.Info(LogTags.Stage, "스테이지 시작: Wave {0}", _currentWaveNumber);
        }

        public void CleanupStage()
        {
            _currentStageData = null;
            _currentWaveNumber = 0;
        }

        /// <summary>
        /// 1~10웨이브를 초기 세팅합니다.
        /// </summary>
        private IEnumerator SetupInitialWavesCoroutine(StageNames stageName)
        {
            yield break;
        }

        private void SpawnPlayerCharacter()
        {
            PlayerCharacter cachedPlayer = CharacterManager.Instance.Player;
            if (cachedPlayer != null)
            {
                Log.Info(LogTags.CharacterSpawn, "이미 플레이어 캐릭터가 존재하여 새로 생성하지 않습니다.");
                return;
            }

            Vector3 spawnPosition = transform.position;
            PlayerCharacter player = ResourcesManager.SpawnPlayerCharacter(spawnPosition, transform);
            if (player == null)
            {
                Log.Error(LogTags.CharacterSpawn, "플레이어 캐릭터 프리팹 스폰에 실패했습니다.");
                return;
            }

            player.transform.localPosition = Vector3.zero;
            player.transform.localRotation = Quaternion.identity;
            player.transform.localScale = Vector3.one;
            player.Initialize();

            Log.Info(LogTags.CharacterSpawn, "플레이어 캐릭터를 생성했습니다. 위치: {0}", spawnPosition);
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
        }
    }
}