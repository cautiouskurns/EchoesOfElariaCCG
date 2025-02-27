using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MapNode : MonoBehaviour
{
    public NodeType NodeType { get; set; }
    public string BattleSceneName { get; set; }
    public string EventSceneName { get; set; }
    public bool HasBeenVisited { get; private set; }
    public EnemyClass[] EnemyEncounter { get; set; }
    public DialogueData LoreEvent { get; set; }
    
    [SerializeField] private List<MapNode> connectedNodes = new List<MapNode>();
    
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
        HasBeenVisited = true;
        
        // If this is the base camp node, save that information
        if (NodeType == NodeType.BaseCamp)
        {
            GameManager.Instance.BaseNodeVisited = true;
        }

        switch (NodeType)
        {
            case NodeType.BaseCamp:
                Debug.Log("[MapNode] Loading character selection scene");
                SceneManager.LoadScene("CampNode");  // Load your character selection scene
                break;

            case NodeType.StandardBattle:
                GameManager.Instance.StartBattle(
                    BattleSceneName, 
                    SceneManager.GetActiveScene().name, 
                    BattleType.Standard,
                    EnemyEncounter);
                break;
                
            case NodeType.EliteBattle:
                GameManager.Instance.StartBattle(
                    BattleSceneName, 
                    SceneManager.GetActiveScene().name,
                    BattleType.Elite,
                    EnemyEncounter);
                break;
                
            case NodeType.LoreEvent:
                GameManager.Instance.StartLoreEvent(LoreEvent, EventSceneName);
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
        
        // Already visited nodes are accessible
        if (HasBeenVisited)
            return true;
        
        // Connected to a visited node
        foreach (MapNode node in connectedNodes)
        {
            if (node != null && node.HasBeenVisited)
            {
                Debug.Log($"[MapNode] Node {NodeType} is accessible via connection to visited node {node.NodeType}");
                return true;
            }
        }
        
        if (connectedNodes.Count > 0)
        {
            Debug.Log($"[MapNode] Node {NodeType} has {connectedNodes.Count} connections but none are visited");
        }
        else
        {
            Debug.Log($"[MapNode] Node {NodeType} has no connections");
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