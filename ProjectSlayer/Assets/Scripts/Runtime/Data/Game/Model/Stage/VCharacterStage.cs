using System;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterStage
    {
        [NonSerialized]
        public StageNames CurrentStage;
        public string CurrentStageString;

        [NonSerialized]
        public AreaNames CurrentArea;
        public string CurrentAreaString;

        [NonSerialized]
        public StageNames MaxReachedStage;
        public string MaxReachedStageString;

        [NonSerialized]
        public AreaNames MaxReachedArea;
        public string MaxReachedAreaString;

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref CurrentStage, CurrentStageString);
            EnumEx.ConvertTo(ref CurrentArea, CurrentAreaString);
            EnumEx.ConvertTo(ref MaxReachedStage, MaxReachedStageString);
            EnumEx.ConvertTo(ref MaxReachedArea, MaxReachedAreaString);
        }

        public void ClearIngameData()
        {
        }

        public void SelectStage(StageNames stageName)
        {
            CurrentStage = stageName;
            CurrentStageString = stageName.ToString();
        }

        public void SelectArea(AreaNames areaName)
        {
            CurrentArea = areaName;
            CurrentAreaString = areaName.ToString();
        }

        public void UpdateMaxReachedStage(StageNames stageName)
        {
            if (stageName > MaxReachedStage)
            {
                MaxReachedStage = stageName;
                MaxReachedStageString = stageName.ToString();
            }
        }

        public void UpdateMaxReachedArea(AreaNames areaName)
        {
            if (areaName > MaxReachedArea)
            {
                MaxReachedArea = areaName;
                MaxReachedAreaString = areaName.ToString();
            }
        }

        public static VCharacterStage CreateDefault()
        {
            return new VCharacterStage()
            {
                CurrentStage = StageNames.Area01_StartForest1,
                CurrentStageString = StageNames.Area01_StartForest1.ToString(),
                CurrentArea = AreaNames.StartForest,
                CurrentAreaString = AreaNames.StartForest.ToString(),
                MaxReachedStage = StageNames.Area01_StartForest1,
                MaxReachedStageString = StageNames.Area01_StartForest1.ToString(),
                MaxReachedArea = AreaNames.StartForest,
                MaxReachedAreaString = AreaNames.StartForest.ToString(),
            };
        }
    }
}