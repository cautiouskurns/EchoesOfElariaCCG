using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private string overworldSceneName = "OverworldMap"; // Default overworld scene
    private string lastScene;
    public CharacterClass[] selectedClasses = new CharacterClass[2]; // Store 2 classes
    private DialogueData currentLoreDialogue;

    [HideInInspector] public BattleType CurrentBattleType { get; private set; }
    [HideInInspector] public EnemyClass[] CurrentEnemies { get; private set; }

    public bool BaseNodeVisited { get; set; } = false;

    // Add a reference to a fallback lore event
    [SerializeField] private DialogueData fallbackLoreEvent;


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

    // Method to explicitly set the overworld scene name
    public void SetOverworldScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            overworldSceneName = sceneName;
            Debug.Log($"[GameManager] Overworld scene set to: {overworldSceneName}");
        }
    }

    // Set the player classes
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
    
    // Modified method to include battle type and enemies
    public void StartBattle(string battleScene, string returnScene, BattleType battleType, EnemyClass[] enemies = null)
    {
        overworldSceneName = returnScene;
        lastScene = SceneManager.GetActiveScene().name; // Store the current scene
        CurrentBattleType = battleType;
        CurrentEnemies = enemies;
        
        Debug.Log($"[GameManager] Starting {battleType} battle in scene: {battleScene}");
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
            Debug.LogError("[GameManager] ❌ No overworld scene stored! Loading default map scene.");
            SceneManager.LoadScene("OverworldMap"); // Hardcoded fallback
        }
    }

    // Fix parameter order to match LoreNode's call
    public void StartLoreEvent(DialogueData dialogueData, string loreScene)
    {
        if (dialogueData == null)
        {
            Debug.LogError("[GameManager] Cannot start lore event with null dialogue! Attempting fallback.");
            
            // Try using fallback dialogue
            if (fallbackLoreEvent != null)
            {
                dialogueData = fallbackLoreEvent;
                Debug.Log("[GameManager] Using fallback lore event");
            }
            else
            {
                Debug.LogError("[GameManager] No fallback dialogue available either!");
                
                // Just load the scene if provided
                if (!string.IsNullOrEmpty(loreScene))
                {
                    lastScene = SceneManager.GetActiveScene().name;
                    SceneManager.LoadScene(loreScene);
                }
                return;
            }
        }

        lastScene = SceneManager.GetActiveScene().name;
        currentLoreDialogue = dialogueData;

        Debug.Log($"[GameManager] 📖 Storing dialogue '{dialogueData.name}' and loading scene: {loreScene}");
        SceneManager.LoadScene(loreScene);
    }

    // Get the stored dialogue data
    public DialogueData GetStoredLoreDialogue()
    {
        return currentLoreDialogue;
    }

    // Provide a fallback lore event
    public DialogueData GetFallbackLoreEvent()
    {
        return fallbackLoreEvent;
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
            ReturnToOverworld(); // Use our ReturnToOverworld method which has fallbacks
        }
    }
}
