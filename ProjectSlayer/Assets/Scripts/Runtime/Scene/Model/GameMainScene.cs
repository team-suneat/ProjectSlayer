using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class GameMainScene : XScene
    {
        #region Private Fields

        [SerializeField]
        private StageLoader _stageLoader;

        #endregion Private Fields

        #region XScene

        protected override void OnCreateScene()
        {
        }

        protected override void OnEnterScene()
        {
            if (_stageLoader != null)
            {
                _stageLoader.Initialize(this);
                _stageLoader.LoadStage();
            }
        }

        protected override void OnExitScene()
        {
        }

        protected override void OnDestroyScene()
        {
        }

        #endregion XScene

        #region Change Scene

        protected override void CleanupCurrentScene()
        {
            _stageLoader?.CleanupStage();

            Audio.AudioManager.Instance.Clear();
            UserInterface.UIManager.Instance?.Clear();
            CharacterManager.Instance.ClearMonsterAndAlliance();
            VitalManager.Instance.Clear();            
            VFXManager.ClearNull();            
            GameApp.Instance.SaveGameData();
            base.CleanupCurrentScene();
        }

        #endregion Change Scene
    }
}