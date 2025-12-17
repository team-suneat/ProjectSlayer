namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 성장 시스템 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Growth Get Methods

        /// <summary>
        /// 성장 시스템 데이터 에셋을 가져옵니다.
        /// </summary>
        public GrowthDataAsset GetGrowthDataAsset()
        {
            return _growthDataAsset;
        }

        /// <summary>
        /// 스탯 이름으로 성장 데이터를 가져옵니다.
        /// </summary>
        public GrowthData GetGrowthData(StatNames statName)
        {
            if (_growthDataAsset == null)
            {
                return null;
            }

            return _growthDataAsset.FindGrowthData(statName);
        }

        #endregion Growth Get Methods

        #region Growth Load Methods

        // LoadGrowthDataSync는 ScriptableDataManager.Load.cs에 정의됨

        #endregion Growth Load Methods

        #region Growth Refresh Methods

        /// <summary>
        /// 성장 시스템 데이터 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllGrowth()
        {
            _growthDataAsset?.Refresh();
        }

        #endregion Growth Refresh Methods
    }
}