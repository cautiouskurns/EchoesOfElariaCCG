using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private string overworldSceneName = "OverworldMap"; // Default overworld scene
    public CharacterClass[] SelectedPlayerClasses { get; private set; } = new CharacterClass[3]; // ✅ Store up to 3 selected classes

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // Ensure no parent object
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[GameManager] Created new instance with ID: {gameObject.GetInstanceID()}");
        }
        else
        {
            Debug.Log($"[GameManager] Found existing instance with ID: {Instance.gameObject.GetInstanceID()}, destroying duplicate with ID: {gameObject.GetInstanceID()}");
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Scene loaded: {scene.name}. Instance ID: {gameObject.GetInstanceID()}");
    }

    // ✅ Set player's selected class at a specific index (0, 1, or 2)
    public void SetPlayerClass(int index, CharacterClass newClass)
    {
        if (index < 0 || index >= SelectedPlayerClasses.Length)
        {
            Debug.LogError($"[GameManager] ❌ Invalid player class index: {index}");
            return;
        }

        SelectedPlayerClasses[index] = newClass;
        Debug.Log($"[GameManager] 🏹 Player {index + 1} class set to: {newClass.className}");
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
