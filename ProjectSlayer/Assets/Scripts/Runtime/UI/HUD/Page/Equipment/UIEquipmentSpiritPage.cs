using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 장비 정령 탭 페이지 - 정령 장비 UI (추후 확장)
    public class UIEquipmentSpiritPage : UIPage
    {
        [Title("#UIEquipmentSpiritPage")]

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnShow()
        {
            base.OnShow();

            Refresh();
        }

        public void Refresh()
        {
        }
    }
}
