using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private string overworldSceneName = "OverworldMap"; // Default scene name

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameManager] Initialized and set to persist");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void StartBattle(string battleScene, string overworldScene)
    {
        overworldSceneName = overworldScene;
        Debug.Log($"[GameManager] 🔄 Transitioning to battle scene: {battleScene}");
        SceneManager.LoadScene(battleScene);
    }

    public void ReturnToOverworld()
    {
        if (!string.IsNullOrEmpty(overworldSceneName))
        {
            Debug.Log($"[GameManager] 🔄 Returning to overworld: {overworldSceneName}");
            SceneManager.LoadScene(overworldSceneName);
        }
        else
        {
            Debug.LogError("[GameManager] ❌ No overworld scene stored!");
        }
    }
}
