using UnityEngine;

namespace TeamSuneat.Scenes
{
    public class GameMainScene : XScene
    {
        #region Private Fields

        [SerializeField]
        private StageLoader _stageLoader;

        [SerializeField]
        private PlayerCharacterSpawner _playerCharacterSpawner;

        #endregion Private Fields

        #region XScene

        protected override void OnCreateScene()
        {
            RegisterGlobalEvent();
        }

        protected override void OnEnterScene()
        {
            if (_playerCharacterSpawner != null)
            {
                _playerCharacterSpawner.Initialize(this);
                _playerCharacterSpawner.SpawnPlayer();
            }

            if (_stageLoader != null)
            {
                _stageLoader.LoadStage();
            }
        }

        protected override void OnExitScene()
        {
        }

        protected override void OnDestroyScene()
        {
            UnregisterGlobalEvent();
        }

        #endregion XScene

        #region Global Event

        private void RegisterGlobalEvent()
        {
            GlobalEvent.Register(GlobalEventType.MOVE_TO_STAGE, OnMoveToStage);
            GlobalEvent.Register(GlobalEventType.MOVE_TO_TITLE, OnMoveToTitle);
        }

        private void UnregisterGlobalEvent()
        {
            GlobalEvent.Unregister(GlobalEventType.MOVE_TO_STAGE, OnMoveToStage);
            GlobalEvent.Unregister(GlobalEventType.MOVE_TO_TITLE, OnMoveToTitle);
        }

        private void OnMoveToTitle()
        {
            if (DetermineChangeScene("GameTitle"))
            {
                CharacterManager.Instance.UnregisterPlayer();
                GameApp.Instance.gameManager.ResetStage();
                ChangeScene("GameTitle");
            }
        }

        private void OnMoveToStage()
        {
            if (DetermineChangeScene("GameMain"))
            {
                GameApp.Instance.gameManager.ResetStage();
                ChangeScene("GameMain");
            }
        }

        #endregion Global Event

        #region Change Scene

        protected override void CleanupCurrentScene()
        {
            _stageLoader?.CleanupStage();
            _playerCharacterSpawner?.CleanupPlayer();

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