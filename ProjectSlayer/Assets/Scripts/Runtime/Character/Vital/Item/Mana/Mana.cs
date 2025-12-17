using UnityEngine;

namespace TeamSuneat
{
    [System.Serializable]
    public class Mana : VitalResource
    {
        public override VitalResourceTypes Type => VitalResourceTypes.Mana;

        public override void LoadCurrentValue()
        {
            Current = Max;

            LogInfo("캐릭터의 마나를 초기화합니다. {0}/{1}", Current, Max);
        }

        public override bool AddCurrentValue(int value)
        {
            if (base.AddCurrentValue(value))
            {
                Vital.RefreshResourceGauge();
                return true;
            }
            return false;
        }

        public override bool UseCurrentValue(int value)
        {
            if (base.UseCurrentValue(value))
            {
                Vital.RefreshResourceGauge();
                return true;
            }
            return false;
        }

        protected override void OnAddCurrentValue(int value)
        {
            base.OnAddCurrentValue(value);
            Vital.RefreshResourceGauge();
        }
    }
}