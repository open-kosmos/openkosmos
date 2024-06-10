using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kosmos
{
    public class EntryPoint : MonoBehaviour
    {
        private const string CLASS_NAME = nameof(EntryPoint);
        
        private const string SCENE_MAIN_MENU = "MainMenu";
        
        private void Awake()
        {
            Debug.Log("Welcome to Open Kosmos!");

            LoadMods();
            TransitionToMainMenu();
        }
        
        private void LoadMods()
        {
            Debug.Log($"[{CLASS_NAME}] Loading mods...");
        }
        
        private void TransitionToMainMenu()
        {
            Debug.Log($"[{CLASS_NAME}] Transitioning to main menu...");
            
            UnityEngine.SceneManagement.SceneManager.LoadScene(SCENE_MAIN_MENU, LoadSceneMode.Additive);
        }
    }
}