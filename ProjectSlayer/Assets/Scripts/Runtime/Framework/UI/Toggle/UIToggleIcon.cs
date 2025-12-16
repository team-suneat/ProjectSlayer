using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat
{
    public class UIToggleIcon : XBehaviour
    {
        [Title("#UIToggleIcon")]
        [SerializeField] private Image _iconImage;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _iconImage ??= this.FindComponent<Image>("Toggle Icon Image");
        }

        public void Activate()
        {
            if (_iconImage != null)
            {
                _iconImage.SetActive(true);
            }
        }

        public void Deactivate()
        {
            if (_iconImage != null)
            {
                _iconImage.SetActive(false);
            }
        }

        public void SetIcon(Sprite sprite)
        {
            AutoGetComponents();

            if (_iconImage == null)
            {
                return;
            }

            _iconImage.sprite = sprite;
            _iconImage.enabled = sprite != null;
        }

        public void SetGrayScale(bool isGray)
        {
            AutoGetComponents();

            SetGrayScale(_iconImage, isGray);
        }

        private void SetGrayScale(Image image, bool isGray)
        {
            if (image == null)
            {
                return;
            }

            image.color = isGray ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white;
        }
    }
}