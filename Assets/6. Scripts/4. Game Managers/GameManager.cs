using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private string overworldSceneName = "OverworldMap"; // Default overworld scene
    private string lastScene;
    public CharacterClass[] selectedClasses = new CharacterClass[2]; // ✅ Store all 2 classes
    private DialogueData currentLoreDialogue;

    [HideInInspector] public BattleType CurrentBattleType { get; private set; }
    [HideInInspector] public EnemyClass[] CurrentEnemies { get; private set; }

    public bool BaseNodeVisited { get; set; } = false;

    [Header("Map Persistence")]
    // Store the state of all map nodes
    private Dictionary<string, MapNodeData> mapNodeStates = new Dictionary<string, MapNodeData>();
    
    // Track current node for when returning to map
    public string CurrentNodeId { get; set; }
    
    // Track if map has been generated
    public bool MapGenerated { get; set; }

    // Add scene tracking for returning to map
    private bool returningToMap = false;
    public bool IsReturningToMap { get { return returningToMap; } }

    // Add an event system for map regeneration
    public delegate void MapEventHandler();
    public event MapEventHandler OnReturnToMap;

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
        
        // Check if we're returning to the map scene
        if (scene.name == overworldSceneName && returningToMap)
        {
            Debug.Log("[GameManager] Returning to map - triggering map restoration");
            
            // Allow the map to initialize first, then notify
            Invoke("NotifyMapRestoration", 0.1f);
        }
    }
    
    private void NotifyMapRestoration()
    {
        // Trigger map regeneration event
        OnReturnToMap?.Invoke();
        returningToMap = false;
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


    
    // Modified method to include battle type and enemies
    public void StartBattle(string battleScene, string returnScene, BattleType battleType, EnemyClass[] enemies = null)
    {
        overworldSceneName = returnScene;
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
            
            // Set flag that we're returning to the map
            returningToMap = true;
            
            // Load the map scene
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
            
            // Set flag if returning to map
            if (lastScene == overworldSceneName)
                returningToMap = true;
                
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            Debug.LogError("[GameManager] ❌ No previous scene stored, returning to overworld.");
            returningToMap = true;
            SceneManager.LoadScene(overworldSceneName);
        }
    }

    // Save a node's state when interacting with it
    public void SaveNodeState(string nodeId, NodeType nodeType, bool visited, 
        int pathIndex, int nodeIndex, List<string> connectedNodeIds)
    {
        mapNodeStates[nodeId] = new MapNodeData
        {
            NodeId = nodeId,
            NodeType = nodeType,
            Visited = visited,
            PathIndex = pathIndex,
            NodeIndex = nodeIndex,
            ConnectedNodeIds = connectedNodeIds
        };
    }
    
    // Get a node's state when recreating the map
    public MapNodeData GetNodeState(string nodeId)
    {
        if (mapNodeStates.ContainsKey(nodeId))
        {
            return mapNodeStates[nodeId];
        }
        return null;
    }
    
    // Check if a node has been visited
    public bool IsNodeVisited(string nodeId)
    {
        if (mapNodeStates.ContainsKey(nodeId))
        {
            return mapNodeStates[nodeId].Visited;
        }
        return false;
    }
    
    // Clear map data (when starting a new run)
    public void ClearMapData()
    {
        mapNodeStates.Clear();
        MapGenerated = false;
        CurrentNodeId = null;
    }

    // Get all node data for map reconstruction
    public List<MapNodeData> GetAllNodeData()
    {
        return new List<MapNodeData>(mapNodeStates.Values);
    }
}

// Serializable struct to store node data
[System.Serializable]
public class MapNodeData
{
    public string NodeId;
    public NodeType NodeType;
    public bool Visited;
    public int PathIndex;
    public int NodeIndex;
    public List<string> ConnectedNodeIds;
}
