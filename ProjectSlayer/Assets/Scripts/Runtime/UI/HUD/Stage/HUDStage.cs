using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDStage : MonoBehaviour
    {
        [SerializeField]
        private UILocalizedText _indexText;

        [SerializeField]
        private UILocalizedText _nameText;

        private void Start()
        {
            Refresh();
        }

        private void Refresh()
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();
            if (profileInfo == null)
            {
                return;
            }

            int index = profileInfo.Stage.CurrentStage.ToInt();
            StringData data = JsonDataManager.FindStringData("Format_Stage_Index");
            string content = StringGetter.Format(data, index.ToString());
            _indexText.SetText(content);

            string stringKey = profileInfo.Stage.CurrentStage.GetStringKey();
            _nameText.SetStringKey(stringKey);
        }
    }
}