using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arkship.PSC
{
    public class EntryPoint : MonoBehaviour
    {
        private const string SCENE_MAIN_MENU = "MainMenu";
        private void Start()
        {
            Debug.Log("Welcome to Plushy Space Cooperative!");

            LoadMods();
            TransitionToMainMenu();
        }
        
        private void LoadMods()
        {
            Debug.Log("Loading mods...");
        }
        
        private void TransitionToMainMenu()
        {
            Debug.Log("Transitioning to main menu...");
            
            UnityEngine.SceneManagement.SceneManager.LoadScene(SCENE_MAIN_MENU, LoadSceneMode.Additive);
        }
    }
}