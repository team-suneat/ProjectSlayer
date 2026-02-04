using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UIEquipmentItemPopupButton : MonoBehaviour
    {
        private UIButton _button;
        private UIEquipmentSlotItem _slotItem;

        private void Awake()
        {
            _button = GetComponent<UIButton>();
            _slotItem = this.FindFirstParentComponent<UIEquipmentSlotItem>();
        }

        private void OnEnable()
        {
            if (_button != null)
            {
                _button.RegisterClickSuccessEvent(OnClick);
            }
        }

        private void OnDisable()
        {
            if (_button != null)
            {
                _button.UnregisterClickSuccessEvent(OnClick);
            }
        }

        private void OnClick()
        {
            if (_slotItem == null)
            {
                return;
            }

            if (UIManager.Instance.PopupManager.BlockSpawnPopup)
            {
                return;
            }

            UIPopup popup = UIManager.Instance.PopupManager.SpawnCenterPopup(UIPopupNames.EquipmentItem, null);
            if (popup != null)
            {
                UIEquipmentItemPopup equipmentItemPopup = popup as UIEquipmentItemPopup;
                equipmentItemPopup.Setup(_slotItem.ItemName, _slotItem.ItemType);
            }
        }
    }
}