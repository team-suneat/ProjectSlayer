using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class StageLoader : XBehaviour
    {
        private StageSystem _currentStageSystem;
        private XScene _parentScene;

        public StageSystem CurrentStageSystem => _currentStageSystem;

        public void Initialize(XScene parentScene)
        {
            _parentScene = parentScene;
        }

        public void CleanupStage()
        {
            if (_currentStageSystem != null)
            {
                _currentStageSystem.CleanupStage();
                Destroy(_currentStageSystem.gameObject);
                _currentStageSystem = null;
            }
        }

        public async void LoadStage()
        {
            try
            {
                await WaitForGameAppInitialized();

                Data.Game.VProfile profileInfo = GameApp.GetSelectedProfile();
                if (profileInfo == null)
                {
                    Log.Error(LogTags.Stage, "프로필 정보가 없습니다.");
                    return;
                }
                if (profileInfo.Stage == null)
                {
                    Log.Error(LogTags.Stage, "스테이지 정보가 없습니다.");
                    return;
                }

                StageNames currentStageName = profileInfo.Stage.CurrentStage;
                Log.Info(LogTags.Stage, "스테이지 로드 시작: {0}", currentStageName);

                StageData stageData = JsonDataManager.FindStageDataClone(currentStageName);
                if (stageData == null)
                {
                    Log.Error(LogTags.Stage, "스테이지 데이터를 찾을 수 없습니다: {0}", currentStageName);
                    return;
                }

                await ResourcesManager.LoadResourcesByLabelAsync<GameObject>(currentStageName.ToString());

                CreateStageSystem(currentStageName);
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Stage, "스테이지 로드 중 오류 발생: {0}", ex.Message);
            }
        }

        protected override void RegisterGlobalEvent()
        {
            GlobalEvent.Register(GlobalEventType.MOVE_TO_STAGE, OnMoveToStage);
            GlobalEvent.Register(GlobalEventType.MOVE_TO_TITLE, OnMoveToTitle);
        }

        protected override void UnregisterGlobalEvent()
        {
            GlobalEvent.Unregister(GlobalEventType.MOVE_TO_STAGE, OnMoveToStage);
            GlobalEvent.Unregister(GlobalEventType.MOVE_TO_TITLE, OnMoveToTitle);
        }

        private void OnMoveToTitle()
        {
            if (_parentScene != null && _parentScene.DetermineChangeScene("GameTitle"))
            {
                CharacterManager.Instance.UnregisterPlayer();
                GameApp.Instance.gameManager.ResetStage();

                _parentScene.ChangeScene("GameTitle");
            }
        }

        private void OnMoveToStage()
        {
            if (_parentScene != null && _parentScene.DetermineChangeScene("GameMain"))
            {
                GameApp.Instance.gameManager.ResetStage();

                _parentScene.ChangeScene("GameMain");
            }
        }

        private async System.Threading.Tasks.Task WaitForGameAppInitialized()
        {
            while (GameApp.Instance == null || !GameApp.Instance.IsInitialized)
            {
                await System.Threading.Tasks.Task.Delay(10);
            }

            Log.Info(LogTags.Stage, "GameApp 초기화 완료 확인");
        }

        private void CreateStageSystem(StageNames stageName)
        {
            _currentStageSystem = ResourcesManager.SpawnStage(stageName, transform);
            if (_currentStageSystem != null)
            {
                _currentStageSystem.Initialize();

                Log.Info(LogTags.Stage, "스테이지 시스템 초기화 완료: {0}", stageName.ToLogString());

                _currentStageSystem.StartStage();

                Log.Info(LogTags.Stage, "스테이지 시스템 생성 완료: {0}", stageName.ToLogString());
            }
            else
            {
                Log.Error($"{stageName.ToLogString()} 스테이지를 생성할 수 없습니다. 플레이어 턴이 시작되지 않습니다.");
            }
        }
    }
}
