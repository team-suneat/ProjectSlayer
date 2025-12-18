using System.Collections;
using TeamSuneat.Data;
using TeamSuneat.UserInterface;

namespace TeamSuneat.Assets.Scripts.Runtime.Stage
{
    public class StageSystem : XBehaviour
    {
        public StageNames Name;
        public string NameString;

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
            RegisterCurrentStage();
            Log.Info(LogTags.Stage, "스테이지 초기화 완료: {0}", Name);
            StartCoroutine(StartStageFlow());
        }

        private void LoadStageData()
        {
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
        }
    }
}