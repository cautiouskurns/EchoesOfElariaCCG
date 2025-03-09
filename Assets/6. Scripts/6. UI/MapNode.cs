using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MapNode : MonoBehaviour
{
    // Add unique identifier for this node
    [SerializeField] private string nodeId;

    public NodeType NodeType { get; set; }
    public string BattleSceneName { get; set; }
    public string EventSceneName { get; set; }
    public bool HasBeenVisited { get; private set; }
    public EnemyClass[] EnemyEncounter { get; set; }
    public DialogueData LoreEvent { get; set; }
    public int PathIndex { get; set; }
    public int NodeIndex { get; set; }
    [SerializeField] private List<MapNode> connectedNodes = new List<MapNode>();

    private void Awake()
    {
        // Generate unique ID if not already assigned
        if (string.IsNullOrEmpty(nodeId))
        {
            nodeId = System.Guid.NewGuid().ToString();
        }
    }

    public string GetNodeId()
    {
        return nodeId;
    }

    public void SetNodeId(string id)
    {
        nodeId = id;
    }

    public void OnNodeClicked()
    {
        Debug.Log($"[MapNode] Node clicked: {NodeType}");
        
        if (!IsAccessible())
        {
            Debug.Log("[MapNode] Node is not accessible");
            return;
        }
        
        Debug.Log($"[MapNode] Processing click for node type: {NodeType}");
        
        // Mark as visited BEFORE loading the new scene
        SetVisited(true);
        
        // If this is the base camp node, save that information
        if (NodeType == NodeType.BaseCamp)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.BaseNodeVisited = true;
        }

        // Inform the MapPersistenceManager we're leaving the map
        if (MapPersistenceManager.Instance != null)
        {
            MapPersistenceManager.Instance.SetNodeVisited(nodeId, true);
            MapPersistenceManager.Instance.LeaveMapScene();
        }

        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        switch (NodeType)
        {
            case NodeType.BaseCamp:
                Debug.Log("[MapNode] Loading character selection scene");
                SceneManager.LoadScene("CampNode");  // Load your character selection scene
                break;

            case NodeType.StandardBattle:
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetOverworldScene(currentSceneName); // Store current scene name
                    GameManager.Instance.StartBattle(
                        BattleSceneName, 
                        currentSceneName, 
                        BattleType.Standard,
                        EnemyEncounter);
                }
                else
                    SceneManager.LoadScene(BattleSceneName);
                break;
                
            case NodeType.EliteBattle:
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetOverworldScene(currentSceneName); // Store current scene name
                    GameManager.Instance.StartBattle(
                        BattleSceneName, 
                        currentSceneName,
                        BattleType.Elite,
                        EnemyEncounter);
                }
                else
                    SceneManager.LoadScene(BattleSceneName);
                break;
                
            case NodeType.LoreEvent:
                // Add null check for LoreEvent
                if (LoreEvent != null)
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.SetOverworldScene(currentSceneName); // Store current scene name
                        GameManager.Instance.StartLoreEvent(LoreEvent, EventSceneName);
                    }
                    else if (!string.IsNullOrEmpty(EventSceneName))
                        SceneManager.LoadScene(EventSceneName);
                }
                else
                {
                    Debug.LogError($"[MapNode] LoreEvent is null for node {nodeId}! Cannot start lore event. Attempting fallback...");
                    // Try to get a fallback event
                    if (GameManager.Instance != null)
                    {
                        DialogueData fallbackEvent = GameManager.Instance.GetFallbackLoreEvent();
                        if (fallbackEvent != null)
                        {
                            GameManager.Instance.SetOverworldScene(currentSceneName); // Store current scene name
                            GameManager.Instance.StartLoreEvent(fallbackEvent, EventSceneName);
                        }
                        else if (!string.IsNullOrEmpty(EventSceneName))
                        {
                            // Just load the scene directly as last resort
                            SceneManager.LoadScene(EventSceneName);
                        }
                    }
                    else if (!string.IsNullOrEmpty(EventSceneName))
                    {
                        // Fallback to just loading the scene
                        SceneManager.LoadScene(EventSceneName);
                    }
                }
                break;
                
            default:
                Debug.LogWarning($"[MapNode] No action defined for node type {NodeType}");
                break;
        }
        
        HasBeenVisited = true;
    }
    
    // In MapNode.cs
    public bool IsAccessible()
    {
        // Base camp is always accessible
        if (NodeType == NodeType.BaseCamp)
            return true;
        
        // Check if base camp has been visited at the game level
        if (GameManager.Instance != null && !GameManager.Instance.BaseNodeVisited)
        {
            // If base camp hasn't been visited, no other nodes should be accessible
            return false;
        }
        
        // Already visited nodes are accessible
        if (HasBeenVisited)
            return true;
        
        // Connected to a visited node
        foreach (MapNode node in connectedNodes)
        {
            if (node != null && node.HasBeenVisited)
            {
                return true;
            }
        }
        
        return false;
    }


    public void SetVisited(bool visited)
    {
        HasBeenVisited = visited;
    }
    
    public List<MapNode> GetConnectedNodes()
    {
        return connectedNodes;
    }
    
    public void AddConnection(MapNode targetNode)
    {
        if (!connectedNodes.Contains(targetNode))
        {
            connectedNodes.Add(targetNode);
        }
    }
}