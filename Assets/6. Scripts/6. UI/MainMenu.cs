using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("OverworldMap");  // Replace with your actual game scene name
    }

    public void LoadGame()
    {
        Debug.Log("Options Menu Placeholder");
    }

    public void ExitGame()
    {
        Debug.Log("Game Exiting...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;  // Works in the Unity Editor
        #else
            Application.Quit();  // Works in a built executable
        #endif
    }
}
