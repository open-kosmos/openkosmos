using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Arkship.UI.MainMenu
{
    public class MainMenuUiController : MonoBehaviour
    {
        [SerializeField] private UIDocument _mainMenuUiDoc;
        
        private const string SCENE_LOADING = "Loading";
        
        private const string BTN_NEWGAME = "btn_newgame";
        
        private void Awake()
        {
            _mainMenuUiDoc.rootVisualElement.Q<Button>(BTN_NEWGAME).clicked += OnStartGameClicked;
        }
        
        private void OnStartGameClicked()
        {
            Debug.Log($"[MainMenuController] Start game clicked!");
            
            var loadingScene = SceneManager.GetSceneByName("Loading");
            if (!loadingScene.isLoaded)
            {
                SceneManager.LoadScene(SCENE_LOADING, LoadSceneMode.Additive);
            }
        }
    }
}