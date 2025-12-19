using System;
using System.Globalization;

namespace TeamSuneat.Data
{
    public partial class VStatistics
    {
        #region Get

        /// <summary>
        /// 총 플레이 시간을 반환합니다.
        /// 게임 시작 시간과 마지막으로 저장한 시간의 차이를 계산합니다.
        /// </summary>
        /// <returns>총 플레이 시간</returns>
        public TimeSpan GetTotalPlayTime()
        {
            TimeSpan totalPlayTime = LastSaveTime - GameStartTime;
            return totalPlayTime;
        }

        /// <summary>
        /// 마지막 저장 시간을 문자열로 반환합니다.
        /// </summary>
        /// <returns>마지막 저장 시간 문자열</returns>
        public string GetLastSaveTimeString()
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            return GameStartTime.ToString("yyyy/MM/dd HH:mm:ss", culture);
        }

        /// <summary>
        /// 총 플레이 시간을 문자열로 반환합니다.
        /// </summary>
        /// <returns>총 플레이 시간 문자열</returns>
        public string GetTotalPlayTimeString()
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            return GetTotalPlayTime().ToString(@"hh\:mm", culture);
        }

        #endregion Get

        /// <summary>
        /// 게임 시작 시간을 등록합니다.
        /// </summary>
        public void RegisterGameStartTime()
        {
            GameStartTime = DateTime.Now;
        }

        /// <summary>
        /// 마지막 저장 시간을 등록합니다.
        /// </summary>
        public void RegisterLastSaveTime()
        {
            LastSaveTime = DateTime.Now;
        }
    }
}