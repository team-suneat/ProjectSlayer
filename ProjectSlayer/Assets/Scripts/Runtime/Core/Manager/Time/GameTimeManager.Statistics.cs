using TeamSuneat.Data;

namespace TeamSuneat
{
    /// <summary>
    /// GameTimeManager의 통계 저장/로드 기능을 담당하는 파셜 클래스
    /// </summary>
    public partial class GameTimeManager
    {
        /// <summary>
        /// VStatistics에서 게임플레이 시간을 로드합니다.
        /// 현재는 VStatistics에 게임플레이 시간 필드가 없으므로 항상 false를 반환합니다.
        /// </summary>
        /// <returns>로드 성공 여부</returns>
        private bool LoadGameplayTimeFromStatistics()
        {
            // VStatistics에서 게임플레이 시간 필드가 제거되었으므로
            // 항상 0으로 시작
            TotalGameplayTime = 0f;
            return false;
        }

        /// <summary>
        /// 현재 게임플레이 시간을 VStatistics에 저장합니다.
        /// 현재는 VStatistics에 게임플레이 시간 필드가 없으므로 저장하지 않습니다.
        /// </summary>
        private void SaveGameplayTimeToStatistics()
        {
            // VStatistics에서 게임플레이 시간 필드가 제거되었으므로
            // 저장하지 않음 (런타임에만 유지)
            // TotalGameplayTime은 메모리에만 존재하며, 게임 종료 시 초기화됨
        }
    }
}

