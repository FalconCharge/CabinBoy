using UnityEngine;

public class SceneController : MonoBehaviour
{
    private SceneTransition sceneTransition;

    private void Awake()
    {
        sceneTransition = FindFirstObjectByType<SceneTransition>();
        if (sceneTransition == null)
        {
            Debug.LogError("SceneTransition not found! Make sure it exists in the scene.");
        }
    }
    public void StartGame()
    {
        // Load game + Fade to it
        sceneTransition.FadeToScene("Game");
        Debug.Log("Starting up game scene");


        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenMainMenu()
    {
        // Load the Main Menu scene
        sceneTransition.FadeToScene("MainMenu");
        Debug.Log("Starting up MainMenu Scene");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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

    public void LoadOptions()
    {
        sceneTransition.FadeToScene("Options");
        Debug.Log("Starting up Options Scene");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

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
