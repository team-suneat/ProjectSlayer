using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat
{
    public class UIToggleUpdateIndicator : XBehaviour
    {
        [Title("#UIToggleUpdateIndicator")]
        [SerializeField] private Image _indicatorImage;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _indicatorImage ??= this.FindComponent<Image>("Toggle Update Image");
        }

        public void SetHasUpdate(bool hasUpdate)
        {
            if (_indicatorImage != null)
            {
                _indicatorImage.SetActive(hasUpdate);
            }
        }
    }
}