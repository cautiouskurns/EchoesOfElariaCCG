using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

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

    [Header("Node Appearance")]
    [SerializeField] private float accessibleAlpha = 1.0f;  // Full visibility for accessible nodes
    [SerializeField] private float inaccessibleAlpha = 0.7f;  // 40% visibility for inaccessible nodes
    [SerializeField] private float visitedBrightness = 1.2f;  // Slightly brighter for visited nodes
    [SerializeField] private Color visitedTint = new Color(0.7f, 0.9f, 1.0f);  // Light blue tint for visited
    
    // Reference to the node's image component
    private Image nodeImage;

    private void Awake()
    {
        // Generate unique ID if not already assigned
        if (string.IsNullOrEmpty(nodeId))
        {
            nodeId = System.Guid.NewGuid().ToString();
        }
        
        // Cache the image component
        nodeImage = GetComponent<Image>();
    }

    private void Start()
    {
        // Update visuals on start
        UpdateNodeAppearance();
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

    // Update the UpdateNodeAppearance method in MapNode.cs
    public void UpdateNodeAppearance()
    {
        // Get the main image component if not already cached
        if (nodeImage == null)
        {
            nodeImage = GetComponent<Image>();
        }
        
        // Determine the alpha based on accessibility
        float alpha = IsAccessible() ? accessibleAlpha : inaccessibleAlpha;
        
        // Apply to main node image if it exists
        if (nodeImage != null)
        {
            Color newColor = nodeImage.color;
            newColor.a = alpha;
            
            // If visited, apply tint and brightness
            if (HasBeenVisited)
            {
                newColor.r *= visitedTint.r * visitedBrightness;
                newColor.g *= visitedTint.g * visitedBrightness;
                newColor.b *= visitedTint.b * visitedBrightness;
            }
            
            nodeImage.color = newColor;
            Debug.Log($"[MapNode] Updated main image alpha to {alpha} for node {name}");
        }
        
        // Also apply to ALL child images, which may contain the actual visuals
        Image[] childImages = GetComponentsInChildren<Image>(true);
        foreach (Image img in childImages)
        {
            // Skip the main image which we already handled
            if (img == nodeImage) continue;
            
            Color childColor = img.color;
            childColor.a = alpha; // Apply same alpha
            img.color = childColor;
            Debug.Log($"[MapNode] Updated child image alpha to {alpha} for {img.name} in node {name}");
        }
        
        // Make sure any Button component is interactable only if accessible
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = IsAccessible();
        }
    }
    
    private void UpdateChildImages()
    {
        // Get all child images (icons, etc.)
        Image[] childImages = GetComponentsInChildren<Image>();
        foreach (Image img in childImages)
        {
            // Skip the main node image
            if (img == nodeImage) continue;
            
            // Apply same alpha but keep its original color
            Color imgColor = img.color;
            imgColor.a = IsAccessible() ? accessibleAlpha : inaccessibleAlpha;
            img.color = imgColor;
        }
    }

    public void SetVisited(bool visited)
    {
        HasBeenVisited = visited;
        UpdateNodeAppearance();
        
        // Also update the appearance of connected nodes, as their accessibility may have changed
        foreach (MapNode connectedNode in connectedNodes)
        {
            if (connectedNode != null)
            {
                connectedNode.RefreshAccessibility();
            }
        }
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

    // Call this after IsAccessible() may have changed (e.g., when another node becomes visited)
    public void RefreshAccessibility()
    {
        bool wasAccessible = IsAccessible();
        UpdateNodeAppearance();
        
        // Log for debugging
        Debug.Log($"[MapNode] Refreshed node {name} accessibility: {wasAccessible} -> {IsAccessible()}");
    }
}