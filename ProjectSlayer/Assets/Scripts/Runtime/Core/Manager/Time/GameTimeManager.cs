using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    /// <summary>
    /// 게임 타임스케일 관리를 담당하는 싱글톤 클래스
    /// 슬로우 모션 등 연출 효과를 위한 타임스케일 조절을 지원합니다.
    /// </summary>
    public class GameTimeManager : Singleton<GameTimeManager>
    {
        #region Private Fields

        private float _factor = 1.0f; // 100%

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// 시간 스케일 팩터를 설정합니다.
        /// </summary>
        /// <param name="factor">시간 스케일 팩터</param>
        /// <param name="useSetScale">Time.timeScale을 설정할지 여부</param>
        public void SetFactor(float factor, bool useSetScale = true)
        {
            _factor = factor;

            if (useSetScale)
            {
                Time.timeScale = _factor;
                Log.Info(LogTags.Time, "시간 스케일을 {0}로 설정합니다.", ValueStringEx.GetPercentString(Time.timeScale));
            }
        }

        /// <summary>
        /// 슬로우 모션을 활성화합니다.
        /// </summary>
        /// <param name="duration">지속 시간</param>
        /// <param name="factor">슬로우 모션 팩터</param>
        /// <param name="onCompleted">완료 시 호출될 콜백</param>
        /// <returns>슬로우 모션 코루틴</returns>
        public IEnumerator ActivateSlowMotion(float duration, float factor, UnityAction onCompleted)
        {
            Time.timeScale = factor;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = _factor;

            onCompleted?.Invoke();
        }

        #endregion Public Methods
    }
}