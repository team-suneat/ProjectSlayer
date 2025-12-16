using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat
{
    public class UIToggleGroup : XBehaviour
    {
        [Title("#UIToggleGroup")]
        [SerializeField]
        private ToggleGroup _toggleGroup;

        [SerializeField]
        private UIToggle[] _toggles;

        public int ToggleCount => _toggles.Length;
        public ToggleGroup ToggleGroup => _toggleGroup;

        [Title("#UIToggleGroup-Events")]
        public UnityEvent<int> OnToggleChanged;

        private int _currentActiveIndex = -1;
        private UIToggle[] _toggleComponents => _toggles;

        private void Awake()
        {
            Initialize();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _toggleGroup = GetComponentInParent<ToggleGroup>();
            _toggles = GetComponentsInChildren<UIToggle>();
        }

        private void Initialize()
        {
            AutoGetComponents();

            if (_toggleGroup == null)
            {
                Debug.LogError($"[UIToggleGroup] ToggleGroup을 찾을 수 없습니다. {gameObject.name}");
                return;
            }

            // 같은 토글을 다시 눌러서 닫을 수 있도록 Allow Switch Off 활성화
            _toggleGroup.allowSwitchOff = true;

            for (int i = 0; i < _toggles.Length; i++)
            {
                if (_toggles[i] == null)
                {
                    Debug.LogWarning($"[UIToggleGroup] 인덱스 {i}의 Toggle이 null입니다.");
                    continue;
                }

                // 각 토글의 그룹 설정
                _toggles[i].SetGroup(_toggleGroup);

                int index = i;
                _toggles[i].AddListener((isOn) => OnToggleValueChanged(index, isOn));
            }
        }

        private void OnToggleValueChanged(int index, bool isOn)
        {
            if (isOn)
            {
                if (_currentActiveIndex == index)
                {
                    // 같은 토글을 다시 누른 경우 - 토글 닫기
                    _toggles[index].SetIsOn(false);
                    _currentActiveIndex = -1;
                    OnToggleChanged?.Invoke(-1);
                }
                else
                {
                    // 다른 토글을 누른 경우
                    _currentActiveIndex = index;
                    OnToggleChanged?.Invoke(index);
                }
            }
            else
            {
                // 토글이 꺼진 경우
                if (_currentActiveIndex == index)
                {
                    _currentActiveIndex = -1;
                    OnToggleChanged?.Invoke(-1);
                }
            }
        }

        public void SetToggle(int index, bool isOn)
        {
            if (index < 0 || index >= _toggles.Length)
            {
                Debug.LogWarning($"[UIToggleGroup] 유효하지 않은 인덱스입니다: {index}");
                return;
            }

            if (_toggles[index] != null)
            {
                _toggles[index].SetIsOn(isOn);
            }
        }

        public Toggle GetToggle(int index)
        {
            if (index < 0 || index >= _toggles.Length)
            {
                return null;
            }

            return _toggles[index].Toggle;
        }

        public int GetActiveToggleIndex()
        {
            return _currentActiveIndex;
        }

        public UIToggle GetUIToggle(int index)
        {
            if (index < 0 || index >= _toggleComponents.Length)
            {
                return null;
            }

            return _toggleComponents[index];
        }
    }
}

