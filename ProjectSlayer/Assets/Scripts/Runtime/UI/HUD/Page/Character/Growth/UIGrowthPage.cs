using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    // 성장 페이지 - 능력치 포인트로 구매하는 성장 능력치 관리
    public class UIGrowthPage : UIPage
    {
        [Title("#UIGrowthPage")]
        [SerializeField] private Transform _contentParent;

        // TODO: 성장 아이템 목록 구현

        protected override void OnShow()
        {
            base.OnShow();

            Refresh();
        }

        public void Refresh()
        {
            // TODO: 성장 아이템 갱신 로직 구현
            Log.Info(LogTags.UI_Page, "성장 페이지가 열렸습니다.");
        }
    }
}