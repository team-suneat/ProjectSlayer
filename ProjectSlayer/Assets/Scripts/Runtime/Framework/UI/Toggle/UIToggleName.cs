using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace TeamSuneat
{
    public class UIToggleName : XBehaviour
    {
        [Title("#UIToggleName")]
        [SerializeField] private TextMeshProUGUI _nameText;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _nameText ??= GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetName(string content)
        {
            AutoGetComponents();

            if (_nameText != null)
            {
                _nameText.text = content;
            }
        }

        public void SetGrayScale(bool isGray)
        {
            AutoGetComponents();

            if (_nameText != null)
            {
                _nameText.color = isGray ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white;
            }
        }
    }
}

