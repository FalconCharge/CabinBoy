using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void StartGame()
    {
        // Load the Game scene
        SceneManager.LoadScene("Game"); 
    }

    public void OpenMainMenu()
    {
        // Load the Main Menu scene
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseGame()
    {
        // Freeze the game
        Time.timeScale = 0f;
        // Show the pause UI
    }

    public void ResumeGame()
    {
        // Unfreeze the game
        Time.timeScale = 1f;
        // Hide the pause UI
    }

    public void Exit()
    {
        Application.Quit();

#if UNITY_EDITOR
        // For stopping play mode inside the editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
