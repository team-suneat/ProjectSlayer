using Sirenix.OdinInspector;
using System.Collections;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TeamSuneat.Scenes
{
    public class GameTitleScene : XScene
    {
        [Title("#Settings")]
        public float DelayTimeForChangeScene;

        [Title("#Component")]
        public Button GameStartButton;

        public UILocalizedText TapText;

        protected override void OnCreateScene()
        {
            RegisterButtonEvent();
            SetInteractableButtons(false);
        }

        protected override void OnEnterScene()
        {
            StartCoroutine(WaitForInitialize());
        }

        protected override void OnExitScene()
        {
        }

        protected override void OnDestroyScene()
        {
        }

        //───────────────────────────────────────────────────────────────────────────

        private IEnumerator WaitForInitialize()
        {
            SetInteractableButtons(false);
            SetTapTextActive(false);

            yield return new WaitUntil(() => GameApp.Instance.IsInitialized && !XScene.IsChangeScene);

            SetInteractableButtons(true);
            SetTapTextActive(true);
        }

        private IEnumerator ProcessChangeScene(UnityAction changeSceneAction)
        {
            yield return new WaitForSeconds(DelayTimeForChangeScene);
            changeSceneAction.Invoke();
        }

        //───────────────────────────────────────────────────────────────────────────

        private void RegisterButtonEvent()
        {
            GameStartButton.onClick.AddListener(OnGameStart);
        }

        private void OnGameStart()
        {
            SetInteractableButtons(false);
            StartChangeMainScene();
        }

        //───────────────────────────────────────────────────────────────────────────

        private void SetInteractableButtons(bool value)
        {
            if (GameStartButton != null)
            {
                GameStartButton.interactable = value;
            }
        }

        private void SetTapTextActive(bool value)
        {
            if (TapText != null)
            {
                TapText.SetActive(value);
            }
        }

        public void StartChangeMainScene()
        {
            StartChangeScene(ChangeMainScene);
        }

        private void StartChangeScene(UnityAction changeSceneAction)
        {
            GameSetting.Instance.Input.BlockUIInput();
            if (DelayTimeForChangeScene > 0)
            {
                StartCoroutine(ProcessChangeScene(changeSceneAction));
            }
            else
            {
                changeSceneAction.Invoke();
            }
        }

        private void ChangeMainScene()
        {
            ChangeToScene("GameMain");
        }

        private void ChangeToScene(string sceneName)
        {
            GameSetting.Instance.Input.UnblockUIInput();

            if (DetermineChangeScene(sceneName))
            {
                ChangeScene(sceneName);
            }
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}