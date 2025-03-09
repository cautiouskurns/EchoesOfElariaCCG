using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapNode : MonoBehaviour
{
    // Unique identifier for this node
    [SerializeField] private string nodeId;
    
    public NodeType NodeType { get; set; }
    public int PathIndex { get; set; }
    public int NodeIndex { get; set; }
    
    // Scene names to load
    public string BattleSceneName { get; set; }
    public string EventSceneName { get; set; }
    
    // Data for each node type
    public EnemyClass[] EnemyEncounter { get; set; }
    public DialogueData LoreEvent { get; set; }

    // Visual components
    [SerializeField] private GameObject visitedIndicator;
    [SerializeField] private GameObject availableIndicator;
    
    // Connection data
    private List<MapNode> connectedNodes = new List<MapNode>();
    private bool visited = false;
    
    private void Awake()
    {
        // Generate unique ID if not already set
        if (string.IsNullOrEmpty(nodeId))
        {
            nodeId = System.Guid.NewGuid().ToString();
        }
    }
    
    private void Start()
    {
        // Load state from GameManager if available
        LoadState();
        UpdateVisuals();
    }
    
    public string GetNodeId()
    {
        return nodeId;
    }
    
    public void SetNodeId(string id)
    {
        nodeId = id;
    }
    
    public void AddConnection(MapNode node)
    {
        if (!connectedNodes.Contains(node))
        {
            connectedNodes.Add(node);
        }
    }
    
    public List<MapNode> GetConnections()
    {
        return connectedNodes;
    }
    
    public void OnNodeClicked()
    {
        if (!IsAccessible()) return;
        
        Debug.Log($"[MapNode] Node clicked! Type: {NodeType}, ID: {nodeId}");
        
        // Set visited status
        SetVisited(true);
        
        // Inform the persistence manager we're leaving the map
        if (MapPersistenceManager.Instance != null)
        {
            MapPersistenceManager.Instance.SetNodeVisited(nodeId, true);
            MapPersistenceManager.Instance.LeaveMapScene();
        }
        
        // Store current node in GameManager for returning to the map
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CurrentNodeId = nodeId;
            SaveAllConnections(); // Save all connections before leaving
            SetVisited(true); // Mark as visited
        }
        
        // Load appropriate scene based on node type
        switch (NodeType)
        {
            case NodeType.BaseCamp:
                SceneManager.LoadScene("CampNode");
                break;
                
            case NodeType.StandardBattle:
            case NodeType.EliteBattle:
                if (!string.IsNullOrEmpty(BattleSceneName))
                {
                    SceneManager.LoadScene(BattleSceneName);
                }
                break;
                
            case NodeType.LoreEvent:
                if (!string.IsNullOrEmpty(EventSceneName))
                {
                    SceneManager.LoadScene(EventSceneName);
                }
                break;
        }
    }
    
    public bool IsAccessible()
    {
        // Base camp is always accessible
        if (NodeType == NodeType.BaseCamp) return true;
        
        // If any connected node is visited, this node is accessible
        foreach (var node in connectedNodes)
        {
            if (node != null && node.IsVisited())
            {
                return true;
            }
        }
        
        return false;
    }
    
    public void SetVisited(bool value)
    {
        visited = value;
        UpdateVisuals();
        SaveState(value);
    }
    
    public bool IsVisited()
    {
        return visited;
    }
    
    private void UpdateVisuals()
    {
        if (visitedIndicator != null)
        {
            visitedIndicator.SetActive(visited);
        }
        
        if (availableIndicator != null)
        {
            availableIndicator.SetActive(IsAccessible() && !visited);
        }
    }
    
    // Save node state to GameManager
    private void SaveState(bool isVisited)
    {
        if (GameManager.Instance != null)
        {
            List<string> connectedIds = new List<string>();
            foreach (var node in connectedNodes)
            {
                if (node != null)
                {
                    connectedIds.Add(node.GetNodeId());
                }
            }
            
            GameManager.Instance.SaveNodeState(nodeId, NodeType, isVisited, PathIndex, NodeIndex, connectedIds);
        }
    }
    
    // Load node state from GameManager
    private void LoadState()
    {
        if (GameManager.Instance != null)
        {
            MapNodeData data = GameManager.Instance.GetNodeState(nodeId);
            if (data != null)
            {
                NodeType = data.NodeType;
                visited = data.Visited;
                PathIndex = data.PathIndex;
                NodeIndex = data.NodeIndex;
            }
        }
    }

    // Add method to save all node connections
    private void SaveAllConnections()
    {
        if (GameManager.Instance == null) return;
        
        // Get IDs of all connected nodes
        List<string> connectedIds = new List<string>();
        foreach (var connectedNode in connectedNodes)
        {
            if (connectedNode != null)
            {
                connectedIds.Add(connectedNode.GetNodeId());
            }
        }
        
        // Save this node's state with its connections
        GameManager.Instance.SaveNodeState(nodeId, NodeType, visited, PathIndex, NodeIndex, connectedIds);
    }
}