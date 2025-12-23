using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDStageProgressGauge : XBehaviour
    {
        [SerializeField]
        private UIGauge _gauge;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _gauge ??= GetComponentInChildren<UIGauge>();
        }

        public void SetProgress(float progress)
        {
            if (_gauge == null)
            {
                return;
            }

            _gauge.SetFrontValue(progress);
            _gauge.SetValueText(ValueStringEx.GetPercentString(progress));
        }
    }
}