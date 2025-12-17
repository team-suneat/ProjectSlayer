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

        [SerializeField]
        [Tooltip("false일 경우 항상 하나의 토글이 켜져있어야 합니다.")]
        private bool _allowSwitchOff = true;

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
            _toggles = this.GetComponentsInDirectChildren<UIToggle>();
        }

        private void Initialize()
        {
            AutoGetComponents();

            if (_toggleGroup == null)
            {
                Debug.LogError($"[UIToggleGroup] ToggleGroup을 찾을 수 없습니다. {gameObject.name}");
                return;
            }

            _toggleGroup.allowSwitchOff = _allowSwitchOff;

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

            // allowSwitchOff가 false일 경우 첫 번째 토글을 기본으로 켜기
            if (!_allowSwitchOff && _toggles.Length > 0 && _toggles[0] != null)
            {
                _toggles[0].SetIsOn(true);
                _currentActiveIndex = 0;
            }
        }

        private void OnToggleValueChanged(int index, bool isOn)
        {
            if (isOn)
            {
                if (_allowSwitchOff && _currentActiveIndex == index)
                {
                    // 같은 토글을 다시 누른 경우 - 토글 닫기 (allowSwitchOff가 true일 때만)
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
                    // allowSwitchOff가 false면 다시 켜기
                    if (!_allowSwitchOff)
                    {
                        _toggles[index].SetIsOn(true);
                        return;
                    }

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

