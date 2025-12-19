using UnityEngine;

namespace TeamSuneat
{
    public partial class GameTimeManager
    {
        #region Public Methods

        public void StartGameplayTracking()
        {
            SetState(GameTimeState.Tracking);
            Log.Info(LogTags.Time, "(Manager) 게임플레이 시간 추적을 시작합니다. 현재 시간: {0:F2}초", TotalGameplayTime);
        }

        public void InitializeGameplayTracking(bool isChallengeStarted)
        {
            if (isChallengeStarted)
            {
                if (LoadGameplayTimeFromStatistics())
                {
                    SetState(GameTimeState.Tracking);
                    Log.Info(LogTags.Time, "(Manager) 게임 재시작 시 게임플레이 시간 추적을 복원합니다. 복원된 시간: {0:F2}초", TotalGameplayTime);
                    return;
                }
            }

            TotalGameplayTime = -1f;
            SetState(GameTimeState.None);
            Log.Warning(LogTags.Time, "(Manager) 도전이 시작되지 않았으므로 게임플레이 시간 추적을 시작하지 않습니다.");
        }

        public void StopGameplayTracking()
        {
            SaveGameplayTimeToStatistics();
            SetState(GameTimeState.None);
            Log.Info(LogTags.Time, "(Manager) 게임플레이 시간 추적을 중지합니다. 총 시간: {0:F2}초", TotalGameplayTime);
        }

        public void ResetGameplayTime()
        {
            TotalGameplayTime = -1f;
            SetState(GameTimeState.None);
            Log.Info(LogTags.Time, "(Manager) 게임플레이 시간을 리셋합니다.");
        }

        public void UpdateTimeTracking()
        {
            CheckPauseState();

            if (IsTracking && TotalGameplayTime >= 0)
            {
                float deltaTime = Time.deltaTime;
                TotalGameplayTime += deltaTime;
                UpdateLogTimeTracking();
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void InitializeTimeIfNeeded()
        {
            if (TotalGameplayTime < 0)
            {
                TotalGameplayTime = 0f;
                Log.Info(LogTags.Time, "(Manager) 게임플레이 시간을 0으로 초기화했습니다.");
            }
        }

        private void UpdateLogTimeTracking()
        {
#if UNITY_EDITOR
            if (CurrentState == GameTimeState.InCombat && Time.time - _lastLogTime > LOG_INTERVAL)
            {
                Log.Info(LogTags.Time, "(Manager) 전투 시간 증가: {0:F2}초", TotalGameplayTime);
                _lastLogTime = Time.time;
            }
#endif
        }

        #endregion Private Methods
    }
}