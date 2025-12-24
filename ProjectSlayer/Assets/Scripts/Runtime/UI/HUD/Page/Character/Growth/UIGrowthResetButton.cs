using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    public class UIGrowthResetButton : UIButton
    {
        [FoldoutGroup("#UIButton-GrowthReset"), SerializeField]
        private UIGrowthPage _parentPage;

        [FoldoutGroup("#UIButton-GrowthReset"), SerializeField]
        public UnityEvent OnResetSuccess;

        private const int RESET_COST_DIAMOND = 3000;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _parentPage = this.FindFirstParentComponent<UIGrowthPage>();
        }

        public void Refresh()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                SetFrameImageColor(GameColors.IndianRed);
                return;
            }

            bool canReset = CanReset(profile);

            if (canReset)
            {
                SetFrameImageColor(GameColors.SteelBlue);
            }
            else
            {
                SetFrameImageColor(GameColors.IndianRed);
            }
        }

        private bool CanReset(VProfile profile)
        {
            if (profile == null)
            {
                return false;
            }

            // 다이아몬드 보유 여부 확인
            if (!profile.Currency.CanUseOrNotify(CurrencyNames.Diamond, RESET_COST_DIAMOND))
            {
                return false;
            }

            // 리셋할 레벨이 있는지 확인
            int totalConsumed = profile.Growth.GetTotalConsumedStatPoints();
            if (totalConsumed <= 0)
            {
                return false;
            }

            return true;
        }

        protected override void OnButtonClick()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            // 리셋 가능 여부 확인
            if (!CanReset(profile))
            {
                Log.Warning(LogTags.UI_Page, "성장 시스템 리셋 조건을 충족하지 못했습니다.");
                return;
            }

            // 소비한 능력치 포인트 계산
            int totalConsumed = profile.Growth.GetTotalConsumedStatPoints();

            // 다이아몬드 소비
            profile.Currency.Use(CurrencyNames.Diamond, RESET_COST_DIAMOND);

            // 모든 성장 능력치 레벨 초기화 및 소비한 포인트 반환
            int returnedPoints = profile.Growth.ResetGrowthLevels();
            profile.Growth.AddStatPoint(returnedPoints);

            Log.Info(LogTags.UI_Page, "성장 시스템 리셋 완료! 다이아몬드 {0} 소비, 능력치 포인트 {1} 반환",
                RESET_COST_DIAMOND, returnedPoints);

            // UI 갱신
            Refresh();
            _parentPage?.Refresh();

            // 리셋 성공 이벤트 발생
            OnResetSuccess?.Invoke();
        }
    }
}