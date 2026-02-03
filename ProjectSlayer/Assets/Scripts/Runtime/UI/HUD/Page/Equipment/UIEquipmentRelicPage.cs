using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 장비 유물 탭 페이지 - 유물 장비 UI (추후 확장)
    public class UIEquipmentRelicPage : UIPage
    {
        [Title("#UIEquipmentRelicPage")]

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
