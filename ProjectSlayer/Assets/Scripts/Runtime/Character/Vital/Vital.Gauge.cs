using TeamSuneat.Setting;
using TeamSuneat.UserInterface;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        public virtual void SetHUD()
        {
            RefreshLifeGauge();
            RefreshResourceGauge();
        }

        /// <summary> 생명력 게이지가 할당되어있다면 갱신합니다. </summary>
        public virtual void RefreshLifeGauge()
        {
            if (Life != null)
            {
                if (EnemyGauge != null)
                {
                    EnemyGauge.SetHealth(Life.Current, Life.Max);
                }
            }
        }

        public virtual void RefreshResourceGauge()
        {
            if (Mana != null)
            {
                if (EnemyGauge != null)
                {
                    EnemyGauge.SetResource(Mana.Current, Mana.Max);
                }
            }
        }

        public virtual void RefreshShieldGauge()
        {
            if (Shield != null)
            {
                if (EnemyGauge != null)
                {
                    EnemyGauge.SetShield(Shield.Current, Shield.Max);
                }
            }
        }

        //

        public void SpawnEnemyGauge()
        {
            if (!GameSetting.Instance.Play.UseMonsterGauge) { return; }
            if (!UseGauge) { return; }
            if (!IsAlive) { return; }
            if (Owner == null) { return; }

            MonsterCharacter monster = Owner as MonsterCharacter;
            if (monster == null) { return; }

            if (UIManager.Instance == null || UIManager.Instance.GaugeManager == null) { return; }
            if (UIManager.Instance.GaugeManager.FindEnemy(this) != null) { return; }

            UIEnemyGauge view = UIManager.Instance.GaugeManager.SpawnEnemyGauge(Owner);
            if (view != null)
            {
                view.Bind(monster);
                Log.Info(LogTags.UI_Gauge, "몬스터 게이지를 생성하여 바이탈에 바인드합니다. {0}, {1}",
                    view.GetHierarchyName(), this.GetHierarchyPath());
            }
        }

        //

        public virtual void ConsumeLifePotion(int healValue, int healValueOverTime, float duration)
        { }
    }
}