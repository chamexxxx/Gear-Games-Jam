using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _closeGameButton;

    private void Start()
    {
        _startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        _closeGameButton.onClick.AddListener(OnCloseGameButtonClicked);
    }

    private void OnStartGameButtonClicked()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
    
    private void OnCloseGameButtonClicked()
    {
        QuitGame();
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
