using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UIEnemyGauge : MonoBehaviour, IEnemyGaugeView
    {
        [SerializeField] private UIGauge _healthGauge;
        [SerializeField] private UIGauge _shieldGauge;
        [SerializeField] private UIGauge _manaGauge;
        [SerializeField] private UIFollowObject _followObject;
        [SerializeField] private Vector3 _worldOffset;
        [SerializeField] private Vector3 _screenOffset;

        private MonsterCharacter _monster;
        private Vital _vital;
        private bool _despawnMark;

        private void OnDisable()
        {
            Unbind();
        }

        private void Update()
        {
            if (_despawnMark)
            {
                _despawnMark = false;
                Clear();
            }
        }

        public void Bind(MonsterCharacter monster)
        {
            Unbind();

            if (monster == null)
            {
                return;
            }

            Vital vital = monster.MyVital;
            if (vital == null)
            {
                return;
            }

            _monster = monster;
            _vital = vital;

            UIGaugeManager gaugeManager = UIManager.Instance != null ? UIManager.Instance.GaugeManager : null;
            if (gaugeManager != null)
            {
                if (!gaugeManager.RegisterEnemy(_vital, this))
                {
                    _monster = null;
                    _vital = null;
                    return;
                }
            }

            if (_vital.DieEvent != null)
            {
                _vital.DieEvent.AddListener(OnVitalDied);
            }

            if (_vital.Life != null)
            {
                _vital.Life.OnValueChanged += OnHealthChanged;
                SetHealth(_vital.Life.Current, _vital.Life.Max);
            }

            if (_vital.Shield != null)
            {
                _vital.Shield.OnValueChanged += OnShieldChanged;
                SetShield(_vital.Shield.Current, _vital.Shield.Max);
            }

            if (_vital.Mana != null)
            {
                _vital.Mana.OnValueChanged += OnResourceChanged;
                SetResource(_vital.Mana.Current, _vital.Mana.Max);
            }
            else
            {
                SetResource(0, 0);
            }

            Transform anchor = _vital.GaugePoint != null ? _vital.GaugePoint : _monster.transform;
            SetupFollow(anchor);
        }

        public void Unbind()
        {
            UIGaugeManager gaugeManager = UIManager.Instance != null ? UIManager.Instance.GaugeManager : null;
            if (gaugeManager != null && _vital != null)
            {
                _ = gaugeManager.UnregisterEnemy(_vital);
            }

            if (_vital != null && _vital.DieEvent != null)
            {
                _vital.DieEvent.RemoveListener(OnVitalDied);
            }

            if (_vital != null)
            {
                if (_vital.Life != null)
                {
                    _vital.Life.OnValueChanged -= OnHealthChanged;
                }

                if (_vital.Shield != null)
                {
                    _vital.Shield.OnValueChanged -= OnShieldChanged;
                }

                if (_vital.Mana != null)
                {
                    _vital.Mana.OnValueChanged -= OnResourceChanged;
                }
            }

            _monster = null;
            _vital = null;

            _followObject?.StopFollowing();
        }

        public void SetHealth(int current, int max)
        {
            if (_healthGauge == null)
            {
                return;
            }

            float rate = max > 0 ? (float)current / max : 0f;
            _healthGauge.SetValueText(current, max);
            _healthGauge.SetFrontValue(rate);
        }

        public void SetShield(int current, int max)
        {
            if (_shieldGauge == null)
            {
                return;
            }

            float rate = max > 0 ? (float)current / max : 0f;
            _shieldGauge.SetValueText(current, max);
            _shieldGauge.SetFrontValue(rate);
        }

        public void SetResource(int current, int max)
        {
            if (_manaGauge == null)
            {
                return;
            }

            bool hasMana = max > 0;
            _manaGauge.gameObject.SetActive(hasMana);

            if (!hasMana)
            {
                _manaGauge.ResetValueText();
                _manaGauge.ResetFrontValue();
                return;
            }

            float rate = (float)current / max;
            _manaGauge.SetValueText(current, max);
            _manaGauge.SetFrontValue(rate);
        }

        public void Clear()
        {
            _healthGauge?.ResetValueText();
            _healthGauge?.ResetFrontValue();

            _shieldGauge?.ResetValueText();
            _shieldGauge?.ResetFrontValue();

            _manaGauge?.ResetValueText();
            _manaGauge?.ResetFrontValue();

            Unbind();
        }

        public void SetDespawnMark()
        {
            if (_despawnMark)
            {
                return;
            }

            _despawnMark = true;
        }

        private void SetupFollow(Transform anchor)
        {
            if (_followObject == null)
            {
                return;
            }

            _followObject.IsWorldSpaceCanvas = true;
            _followObject.SetWorldOffset(_worldOffset);
            _followObject.ScreenOffset = _screenOffset;
            _followObject.Setup(anchor);
        }

        private void OnHealthChanged(int current, int max)
        {
            SetHealth(current, max);
        }

        private void OnShieldChanged(int current, int max)
        {
            SetShield(current, max);
        }

        private void OnResourceChanged(int current, int max)
        {
            SetResource(current, max);
        }

        private void OnVitalDied()
        {
            Clear();
        }
    }
}