using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

// This script will specifically handle map persistence across scene loads
public class MapPersistenceManager : MonoBehaviour
{
    private static bool instantiated = false;

    public static MapPersistenceManager Instance
    {
        get
        {
            if (_instance == null && !instantiated)
            {
                instantiated = true;
                GameObject obj = new GameObject("MapPersistenceManager");
                _instance = obj.AddComponent<MapPersistenceManager>();
                DontDestroyOnLoad(obj);
                Debug.Log("[MapPersistenceManager] Auto-created instance");
            }
            return _instance;
        }
        private set { _instance = value; }
    }
    private static MapPersistenceManager _instance;
    
    // The complete serialized state of the map
    private SerializedMapData mapData = new SerializedMapData();
    
    // Track when we need to restore the map
    private bool shouldRestoreMap = false;
    private bool firstTimeLoad = true;
    
    // Store the name of the map scene for returning
    [SerializeField] private string mapSceneName = "OverworldMap";
    
    // Event to notify subscribers when the map should be restored
    public delegate void MapRestoreEventHandler();
    public event MapRestoreEventHandler OnRestoreMapRequest;

    private void Awake()
    {
        // Singleton setup
        if (_instance == null)
        {
            _instance = this;
            instantiated = true;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[MapPersistenceManager] Instance created and set to persist");
        }
        else if (_instance != this)
        {
            Debug.Log("[MapPersistenceManager] Duplicate instance destroyed");
            Destroy(gameObject);
            return;
        }
        
        // Clear out any old data
        ResetMapData();
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    // Handle scene transitions
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[MapPersistenceManager] Scene loaded: {scene.name}, Should restore: {shouldRestoreMap}");
        
        // If we're loading the map scene and need to restore the map
        if (scene.name == mapSceneName && !firstTimeLoad)
        {
            // Give the scene a moment to initialize
            Invoke("TriggerMapRestore", 0.2f);
        }
        
        firstTimeLoad = false;
    }
    
    // Call this method to trigger map restoration
    private void TriggerMapRestore()
    {
        Debug.Log("[MapPersistenceManager] Triggering map restore event");
        OnRestoreMapRequest?.Invoke();
    }
    
    // Call this when leaving the map scene
    public void LeaveMapScene()
    {
        Debug.Log("[MapPersistenceManager] Leaving map scene, map will be restored when returning");
        shouldRestoreMap = true;
    }
    
    // Clear the stored map data
    public void ResetMapData()
    {
        mapData = new SerializedMapData();
        mapData.nodeData = new List<SerializedNodeData>();
        mapData.isGenerated = false;
        shouldRestoreMap = false;
    }
    
    // Save the entire map structure
    public void SaveMapStructure(List<SerializedNodeData> nodes)
    {
        // Log difficulties before saving to verify they're correct
        foreach (var node in nodes)
        {
            Debug.Log($"[MapPersistenceManager] Saving node {node.id} with difficulty: {node.nodeDifficulty}");
        }
        
        mapData.nodeData = nodes;
        mapData.isGenerated = true;
        
        Debug.Log($"[MapPersistenceManager] Saved map with {nodes.Count} nodes");
    }
    
    // Check if we have a stored map
    public bool HasSavedMap()
    {
        return mapData.isGenerated && mapData.nodeData != null && mapData.nodeData.Count > 0;
    }
    
    // Get the stored map data
    public List<SerializedNodeData> GetMapData()
    {
        return mapData.nodeData;
    }
    
    // Update a single node's visited state
    public void SetNodeVisited(string nodeId, bool visited)
    {
        if (!HasSavedMap()) return;
        
        var node = mapData.nodeData.FirstOrDefault(n => n.id == nodeId);
        if (node != null)
        {
            node.visited = visited;
            Debug.Log($"[MapPersistenceManager] Set node {nodeId} visited state to {visited}");
            
            // Update node visuals if in active scene
            UpdateNodeVisualsInScene(nodeId);
        }
    }
    
    // Update visual appearance of any MapNode with the given ID in the active scene
    private void UpdateNodeVisualsInScene(string nodeId)
    {
        MapNode[] allNodes = GameObject.FindObjectsByType<MapNode>(FindObjectsSortMode.None);
        foreach (MapNode node in allNodes)
        {
            if (node.GetNodeId() == nodeId)
            {
                node.UpdateNodeAppearance();
            }
            else
            {
                // Other nodes may have changed accessibility based on this node
                node.RefreshAccessibility();
            }
        }
    }
}

// Class for serializing the entire map
[System.Serializable]
public class SerializedMapData
{
    public bool isGenerated;
    public List<SerializedNodeData> nodeData;
}

// Class for serializing a single node
[System.Serializable]
public class SerializedNodeData
{
    public string id;
    public NodeType nodeType;
    public float xPos;
    public float yPos;
    public int pathIndex;
    public int nodeIndex;
    public bool visited;
    public List<string> connectedNodeIds;
    public string battleSceneName;
    public string eventSceneName;
    public NodeDifficulty nodeDifficulty;

}
