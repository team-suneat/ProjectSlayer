namespace TeamSuneat.UserInterface
{
    public class HUDManager : XBehaviour
    {
        public UICanvasGroupFader HUDCanvasGroupFader;
        public HUDStageProgressGauge StageProgressGauge;
        public bool IsLockHUD { get; set; } = false;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            HUDCanvasGroupFader = GetComponentInChildren<UICanvasGroupFader>();
            StageProgressGauge = GetComponentInChildren<HUDStageProgressGauge>();
        }

        public void LogicUpdate()
        {
        }
    }
}