using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private string overworldSceneName = "OverworldMap"; // Default overworld scene
    private string lastScene;
    public CharacterClass[] selectedClasses = new CharacterClass[3]; // ✅ Store all 3 classes
    private DialogueData currentLoreDialogue;
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

    // ✅ Set the three selected classes
    public void SetPlayerClass(int index, CharacterClass newClass)
    {
        if (index < 0 || index >= selectedClasses.Length)
        {
            Debug.LogError($"[GameManager] ❌ Invalid class index {index}");
            return;
        }

        selectedClasses[index] = newClass;
        Debug.Log($"[GameManager] 🏹 Player {index + 1} class set to: {selectedClasses[index].className}");
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

    // Fix parameter order to match LoreNode's call
    public void StartLoreEvent(DialogueData dialogueData, string loreScene)
    {
        if (dialogueData == null)
        {
            Debug.LogError("[GameManager] Cannot start lore event with null dialogue!");
            return;
        }

        lastScene = SceneManager.GetActiveScene().name;
        currentLoreDialogue = dialogueData;

        Debug.Log($"[GameManager] 📖 Storing dialogue '{dialogueData.name}' and loading scene: {loreScene}");
        SceneManager.LoadScene(loreScene);
    }

    // ✅ Get the stored dialogue data
    public DialogueData GetStoredLoreDialogue()
    {
        return currentLoreDialogue;
    }

    public void ReturnFromLore()
    {
        if (!string.IsNullOrEmpty(lastScene))
        {
            Debug.Log($"[GameManager] 🔄 Returning to: {lastScene}");
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            Debug.LogError("[GameManager] ❌ No previous scene stored, returning to overworld.");
            SceneManager.LoadScene(overworldSceneName);
        }
    }
}
