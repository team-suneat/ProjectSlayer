using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 장비 아이템 팝업 - 무기/악세사리 공용 (강화/합성 페이지 포함)
    public class UIEquipmentItemPopup : UIPopup
    {
        [FoldoutGroup("#UIEquipmentItemPopup")]
        [SerializeField] private UITogglePageController _togglePageController;

        [FoldoutGroup("#UIEquipmentItemPopup")]
        [SerializeField] private UIEquipmentEnhancePage _enhancePage;

        private ItemNames _itemName = ItemNames.None;
        private ItemTypes _equipmentType = ItemTypes.None;

        public override UIPopupNames Name => UIPopupNames.EquipmentItem;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _togglePageController ??= GetComponentInChildren<UITogglePageController>(true);
            _enhancePage ??= GetComponentInChildren<UIEquipmentEnhancePage>(true);
        }

        protected override void Awake()
        {
            base.Awake();
            AutoGetComponents();
        }

        public void Setup(ItemNames itemName, ItemTypes equipmentType)
        {
            _itemName = itemName;
            _equipmentType = equipmentType;

            RefreshTitleText();
            _enhancePage?.Setup(itemName, equipmentType);
        }
    }
}
