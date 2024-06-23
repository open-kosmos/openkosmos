using Kosmos.UI.Loading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Kosmos.UI.MainMenu
{
    [System.Serializable]
    public struct PrototypeSceneEntry
    {
        public string ButtonId;
        public string SceneName;
    }
    
    public class MainMenuUiController : MonoBehaviour
    {
        [SerializeField] private UIDocument _mainMenuUiDoc;
        [SerializeField] private PrototypeSceneEntry[] _prototypeScenes;
        
        private const string SCENE_MAINMENU = "MainMenu";
        private const string SCENE_LOADING = "Loading";
        private const string SCENE_GAME = "Game";
        
        private void Awake()
        {
            for (int i = 0; i < _prototypeScenes.Length; i++)
            {
                var sceneEntry = _prototypeScenes[i];
                
                var button = _mainMenuUiDoc.rootVisualElement.Q<Button>(sceneEntry.ButtonId);
                button.clicked += async () => await AwaitOnPrototypeSceneClicked(sceneEntry.SceneName);
            }
        }
        
        private async Awaitable AwaitOnPrototypeSceneClicked(string sceneName)
        {
            // Immediately load the loading scene if it's not already loaded
            var asyncLoadingLoad = SceneManager.LoadSceneAsync(SCENE_LOADING, LoadSceneMode.Additive);
            await asyncLoadingLoad;

            var loadingScreenUiController = FindFirstObjectByType<LoadingScreenUiController>();
            loadingScreenUiController.SetProgress(0f);
            
            // Load the prototype scene and unload the main menu scene
            var asyncPrototypeScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            var asyncMenuUnload = SceneManager.UnloadSceneAsync(SCENE_MAINMENU);
            
            if (asyncPrototypeScene == null || 
                asyncMenuUnload == null)
            {
                UnityEngine.Debug.LogError("[MainMenuUiController] Failed to load prototype scene or unload main menu scene!");
                return;
            }
            
            while (!asyncPrototypeScene.isDone)
            {
                loadingScreenUiController.SetProgress(asyncPrototypeScene.progress);
                await Awaitable.NextFrameAsync();
            }
            
            await asyncMenuUnload;
            
            var asyncLoadingUnload = SceneManager.UnloadSceneAsync(SCENE_LOADING);
            
            asyncPrototypeScene.allowSceneActivation = true;
        }
    }
}
