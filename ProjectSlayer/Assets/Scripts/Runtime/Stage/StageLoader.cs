using TeamSuneat.Assets.Scripts.Runtime.Stage;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class StageLoader : MonoBehaviour
    {
        private StageSystem _currentStageSystem;

        public StageSystem CurrentStageSystem => _currentStageSystem;

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

                // 프로필 정보 유효성 체크
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

                // 지역 리소스 로드
                AreaNames currentArea = profileInfo.Stage.CurrentArea;
                int areaIndex = (int)currentArea;
                string label = string.Format(AddressableLabels.AreaFormat, areaIndex);
                await ResourcesManager.LoadResourcesByLabelAsync<GameObject>(label);

                // 스테이지 시스템 생성 전 데이터 체크
                StageNames currentStageName = profileInfo.Stage.CurrentStage;
                Log.Info(LogTags.Stage, "스테이지 로드 시작: {0}", currentStageName);

                // 스테이지 시스템 생성
                CreateStageSystem(currentStageName);
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Stage, "스테이지 로드 중 오류 발생: {0}", ex.Message);
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

                // _currentStageSystem.StartStage();

                Log.Info(LogTags.Stage, "스테이지 시스템 생성 완료: {0}", stageName.ToLogString());
            }
            else
            {
                Log.Error($"{stageName.ToLogString()} 스테이지를 생성할 수 없습니다.");
            }
        }
    }
}