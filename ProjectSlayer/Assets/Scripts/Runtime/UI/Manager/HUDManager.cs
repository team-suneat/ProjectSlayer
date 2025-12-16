namespace TeamSuneat.UserInterface
{
    public class HUDManager : XBehaviour
    {
        public UICanvasGroupFader HUDCanvasGroupFader;

        public bool IsLockHUD { get; set; } = false;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            HUDCanvasGroupFader = GetComponentInChildren<UICanvasGroupFader>();
        }

        public void LogicUpdate()
        {
        }
    }
}