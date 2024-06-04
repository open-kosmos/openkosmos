using UnityEngine;
using UnityEngine.UIElements;

namespace Kosmos.UI.Loading
{
    public class LoadingScreenUiController : MonoBehaviour
    {
        [SerializeField] private UIDocument _loadingScreenUiDoc;
        
        private const string PROGRESS_LOADING = "progress_loading";
        
        private ProgressBar _progressBar;
        
        private void Awake()
        {
            _progressBar = _loadingScreenUiDoc.rootVisualElement.Q<ProgressBar>(PROGRESS_LOADING);
        }
        
        public void SetProgress(float progress)
        {
            _progressBar.value = progress;
        }
    }
}