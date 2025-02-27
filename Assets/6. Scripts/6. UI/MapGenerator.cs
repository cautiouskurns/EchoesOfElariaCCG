using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Configuration")]
    [SerializeField] private int runLength = 15; // Total nodes in the run
    [SerializeField] private int pathsCount = 3; // Number of parallel paths
    [SerializeField] private float horizontalSpacing = 150f; // Space between nodes horizontally (UI units)
    [SerializeField] private float verticalSpacing = 200f; // Space between paths vertically (UI units)
    [SerializeField] private float nodeJitterAmount = 20f; // Random offset for natural look (UI units)
    
    [Header("Node Types")]
    [SerializeField] private NodeTypeConfig[] nodeTypes;
    [SerializeField] private GameObject startNodePrefab;
    [SerializeField] private GameObject bossNodePrefab;
    
    [Header("Scene Management")]
    [SerializeField] private string standardBattleScene;
    [SerializeField] private string eliteBattleScene;
    [SerializeField] private string loreEventScene;

    [Header("Base Camp Configuration")]
    [SerializeField] private GameObject baseCampNodePrefab;
    [SerializeField] private string characterSelectionScene = "CampNode";
    [SerializeField] private Vector2 baseCampPosition = new Vector2(50, 300);

    [Header("Enemy Configurations")]
    [SerializeField] private EnemyClass[] standardEnemiesPool;
    [SerializeField] private EnemyClass[] eliteEnemiesPool;
    [SerializeField] private EnemyClass bossEnemy;
    
    [Header("Lore Configurations")]
    [SerializeField] private DialogueData[] loreEventsPool;

    [Header("UI Settings")]
    [SerializeField] private RectTransform mapContainer; // The parent container for the map
    [SerializeField] private LineRenderer connectionPrefab; // For drawing connections
    
    private Dictionary<NodeType, NodeTypeConfig> typeConfigMap = new Dictionary<NodeType, NodeTypeConfig>();
    private List<NodeType>[] paths; // Each path has its own sequence of nodes
    private Canvas parentCanvas;
    
    [Header("Starting Position")]
    [SerializeField] private Vector2 startNodePosition = new Vector2(100, 300); // Customize this in inspector

    private void Awake()
    {
        Debug.Log("[MapGenerator] Awake called");

        // Get the parent canvas
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("[MapGenerator] This script must be on a GameObject that is a child of a Canvas!");
        }

        // If mapContainer isn't set, use this transform
        if (mapContainer == null)
        {
            mapContainer = GetComponent<RectTransform>();
        }

        // Cache node type configs for easy lookup
        foreach (var config in nodeTypes)
        {
            typeConfigMap[config.type] = config;
        }
    }

    private void Start()
    {
        Debug.Log("[MapGenerator] Start called - about to generate map");
        GenerateNewMap();
    }

    public void GenerateNewMap()
    {
        Debug.Log("[MapGenerator] GenerateNewMap called");
        GenerateMap();
    }

    public void GenerateMap()
    {
        Debug.Log("[MapGenerator] Beginning map generation process");

        if (!ValidateReferences())
        {
            Debug.LogError("[MapGenerator] Map generation aborted due to missing references");
            return;
        }

        // Initialize paths
        paths = new List<NodeType>[pathsCount];
        for (int i = 0; i < pathsCount; i++)
        {
            paths[i] = new List<NodeType>();
        }
        
        // Generate paths
        GenerateNodeSequences();
        
        // Place nodes on the map
        PlaceNodesOnMap();
        
        // Connect nodes with lines
        ConnectNodes();
    }
    
    private bool ValidateReferences()
    {
        bool isValid = true;
        
        if (startNodePrefab == null)
        {
            Debug.LogError("[MapGenerator] Start node prefab is missing!");
            isValid = false;
        }
        
        if (bossNodePrefab == null)
        {
            Debug.LogError("[MapGenerator] Boss node prefab is missing!");
            isValid = false;
        }
        
        if (nodeTypes == null || nodeTypes.Length == 0)
        {
            Debug.LogError("[MapGenerator] No node types configured!");
            isValid = false;
        }
        
        foreach (var config in nodeTypes)
        {
            if (config.nodePrefab == null)
            {
                Debug.LogError($"[MapGenerator] Node prefab missing for type {config.type}!");
                isValid = false;
            }
        }

        if (parentCanvas == null)
        {
            Debug.LogError("[MapGenerator] No Canvas found in parents!");
            isValid = false;
        }
        
        return isValid;
    }

    public void TestNodeCreation()
    {
        Debug.Log("[MapGenerator] Testing node creation");
        
        // Clear existing nodes
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        // Get reference to the standard node prefab
        GameObject nodePrefab = null;
        foreach (var config in nodeTypes)
        {
            if (config.type == NodeType.StandardBattle)
            {
                nodePrefab = config.nodePrefab;
                break;
            }
        }
        
        if (nodePrefab == null)
        {
            Debug.LogError("[MapGenerator] No standard battle node prefab found for testing!");
            return;
        }
        
        // Create a simple 3x3 grid of nodes
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Vector3 position = new Vector3(x * 3, y * 2, 0);
                GameObject node = Instantiate(nodePrefab, position, Quaternion.identity, transform);
                
                MapNode mapNode = node.GetComponent<MapNode>();
                if (mapNode != null)
                {
                    mapNode.NodeType = NodeType.StandardBattle;
                    mapNode.BattleSceneName = "BattleScene";
                }
                
                Debug.Log($"[MapGenerator] Created test node at {position}");
            }
        }
    }
    private void ConfigureNodeBehavior(MapNode node)
    {
        // Configure node based on type
        switch (node.NodeType)
        {
            case NodeType.BaseCamp:
            // No need to set scene names since we directly load the scene
                break;

            case NodeType.StandardBattle:
                node.BattleSceneName = standardBattleScene;
                // Assign random enemies from standard pool
                node.EnemyEncounter = GetRandomEnemies(standardEnemiesPool, 1, 3);
                break;
                
            case NodeType.EliteBattle:
                node.BattleSceneName = eliteBattleScene;
                // Assign random elite enemies
                node.EnemyEncounter = GetRandomEnemies(eliteEnemiesPool, 1, 2);
                break;
                
            case NodeType.LoreEvent:
                node.EventSceneName = loreEventScene;
                // Assign random lore event
                node.LoreEvent = GetRandomLoreEvent();
                break;
        }
    }
    
    private EnemyClass[] GetRandomEnemies(EnemyClass[] pool, int min, int max)
    {
        if (pool == null || pool.Length == 0) return new EnemyClass[0];
        
        int count = Random.Range(min, max + 1);
        EnemyClass[] selectedEnemies = new EnemyClass[count];
        
        for (int i = 0; i < count; i++)
        {
            selectedEnemies[i] = pool[Random.Range(0, pool.Length)];
        }
        
        return selectedEnemies;
    }
    
    private DialogueData GetRandomLoreEvent()
    {
        if (loreEventsPool == null || loreEventsPool.Length == 0) return null;
        return loreEventsPool[Random.Range(0, loreEventsPool.Length)];
    }
    
    
    private void GenerateNodeSequences()
    {
        // Track counts of each node type
        Dictionary<NodeType, int> typeCounts = new Dictionary<NodeType, int>();
        Dictionary<NodeType, int> lastSeenPosition = new Dictionary<NodeType, int>();
        
        foreach (var config in nodeTypes)
        {
            typeCounts[config.type] = 0;
            lastSeenPosition[config.type] = -999; // Initialize to far in the past
        }
        
        // Generate each path
        for (int pathIndex = 0; pathIndex < pathsCount; pathIndex++)
        {
            for (int nodeIndex = 0; nodeIndex < runLength; nodeIndex++)
            {
                // Last node in the run should be skipped (reserved for boss)
                if (nodeIndex == runLength - 1) continue;
                
                // Skip if first or last in path (reserved for start/branch nodes)
                if (nodeIndex == 0) 
                {
                    paths[pathIndex].Add(NodeType.StandardBattle);
                    continue;
                }
                
                // Get eligible node types for this position
                List<NodeType> eligibleTypes = new List<NodeType>();
                List<float> typeWeights = new List<float>();
                
                foreach (var config in nodeTypes)
                {
                    // Check if too close to same type
                    bool tooClose = (nodeIndex - lastSeenPosition[config.type] <= config.minDistance);
                    // Check if exceeded max count
                    bool exceededMax = (typeCounts[config.type] >= config.maxPerRun);
                    
                    // Standard battles can be placed anywhere
                    if (config.type == NodeType.StandardBattle || (!tooClose && !exceededMax))
                    {
                        eligibleTypes.Add(config.type);
                        typeWeights.Add(config.weight);
                    }
                }
                
                // Select a node type based on weights
                NodeType selectedType = SelectWeightedRandom(eligibleTypes, typeWeights);
                
                // Update tracking info
                paths[pathIndex].Add(selectedType);
                lastSeenPosition[selectedType] = nodeIndex;
                typeCounts[selectedType]++;
            }
            
            // Last node is always empty (for boss node)
            paths[pathIndex].Add(NodeType.StandardBattle);
        }
    }
    
    private NodeType SelectWeightedRandom(List<NodeType> types, List<float> weights)
    {
        if (types.Count == 0) return NodeType.StandardBattle; // Default
        
        float totalWeight = 0f;
        foreach (float weight in weights)
        {
            totalWeight += weight;
        }
        
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;
        
        for (int i = 0; i < types.Count; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
            {
                return types[i];
            }
        }
        
        return types[types.Count - 1]; // Fallback
    }
    
private void PlaceNodesOnMap()
    {
        Debug.Log("[MapGenerator] Placing nodes on UI map");
        
        // Get the RectTransform of the map container
        RectTransform containerRect = mapContainer;
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;
        
        // Calculate margins and step sizes based on container dimensions
        float leftMargin = startNodePosition.x;
        float bottomMargin = containerHeight * 0.1f;
        float usableWidth = containerWidth * 0.8f;
        float usableHeight = containerHeight * 0.8f;
        
        float horizontalStep = usableWidth / (runLength - 1);
        float verticalStep = usableHeight / (pathsCount - 1);
        
        Debug.Log($"[MapGenerator] Container size: {containerWidth}x{containerHeight}, " +
                $"Step sizes: {horizontalStep}x{verticalStep}");
        
        // Clear existing nodes
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
    // Place base camp node
        GameObject baseCampNode = Instantiate(baseCampNodePrefab, transform);
        RectTransform baseCampRectTransform = baseCampNode.GetComponent<RectTransform>();
        baseCampRectTransform.anchoredPosition = baseCampPosition;
        
        // Configure the base camp node
        MapNode baseCampMapNode = baseCampNode.GetComponent<MapNode>();
        if (baseCampMapNode != null)
        {
            baseCampMapNode.NodeType = NodeType.BaseCamp;
            
            // Set as visited if we're returning from camp scene
            if (GameManager.Instance != null && GameManager.Instance.BaseNodeVisited)
            {
                baseCampMapNode.SetVisited(true);
            }
            else
            {
                baseCampMapNode.SetVisited(true); // Always accessible initially
            }
            
            ConfigureUIComponents(baseCampNode, NodeType.BaseCamp);
        }

        // Track node game objects
        GameObject[,] nodeObjects = new GameObject[pathsCount, runLength];
        nodeObjects[pathsCount/2, 0] = baseCampNode; // Store start node in middle path
        
        // Place path nodes
        for (int pathIndex = 0; pathIndex < pathsCount; pathIndex++)
        {
            for (int nodeIndex = 0; nodeIndex < runLength; nodeIndex++)
            {
                // Skip first node (we already placed the start node)
                if (nodeIndex == 0) continue;
                
                NodeType nodeType = paths[pathIndex][nodeIndex];
                GameObject nodePrefab;
                
                // Last node is the boss
                if (nodeIndex == runLength - 1)
                {
                    nodePrefab = bossNodePrefab;
                }
                else
                {
                    nodePrefab = typeConfigMap[nodeType].nodePrefab;
                }
                
                // Calculate position with some jitter for natural look
                float xJitter = Random.Range(-nodeJitterAmount, nodeJitterAmount);
                float yJitter = Random.Range(-nodeJitterAmount, nodeJitterAmount);
                
                float xPos = leftMargin + (nodeIndex * horizontalStep) + xJitter;
                float yPos = bottomMargin + (pathIndex * verticalStep) + yJitter;
                
                // Instantiate node as UI element
                GameObject node = Instantiate(nodePrefab, transform);
                RectTransform rectTransform = node.GetComponent<RectTransform>();
                
                // Position the node using anchoredPosition (UI positioning)
                rectTransform.anchoredPosition = new Vector2(xPos, yPos);
                
                Debug.Log($"[MapGenerator] Placed {nodeType} node at UI position ({xPos}, {yPos})");
                
                // Configure node component
                MapNode mapNode = node.GetComponent<MapNode>();
                if (mapNode != null)
                {
                    mapNode.NodeType = nodeType;
                    ConfigureNodeBehavior(mapNode);
                }
                else
                {
                    Debug.LogError($"[MapGenerator] Node prefab is missing MapNode component!");
                }
                
                // Make sure UI components are properly set up
                //ConfigureUIComponents(node, nodeType);
                
                nodeObjects[pathIndex, nodeIndex] = node;
            }
        }
        
        // Generate connections between paths occasionally
        GeneratePathConnections(nodeObjects);
        ConnectBaseCampToFirstTier(baseCampNode, nodeObjects);

    }
    
    private void ConnectBaseCampToFirstTier(GameObject baseCampNode, GameObject[,] nodeObjects)
    {
        MapNode baseCampMapNode = baseCampNode.GetComponent<MapNode>();
        if (baseCampMapNode == null) return;
        
        // Connect to first node in each path
        for (int pathIndex = 0; pathIndex < pathsCount; pathIndex++)
        {
            if (nodeObjects[pathIndex, 1] != null) // Connect to second column (index 1)
            {
                MapNode targetNode = nodeObjects[pathIndex, 1].GetComponent<MapNode>();
                if (targetNode != null)
                {
                    baseCampMapNode.AddConnection(targetNode);
                    targetNode.AddConnection(baseCampMapNode); // Bidirectional
                    
                    Debug.Log($"[MapGenerator] Connected base camp to node at {nodeObjects[pathIndex, 1].GetComponent<RectTransform>().anchoredPosition}");
                    
                    // Visualize the connection
                    DrawConnectionLine(baseCampNode.GetComponent<RectTransform>(), 
                                    nodeObjects[pathIndex, 1].GetComponent<RectTransform>());
                }
            }
        }
    }

    private void DrawConnectionLine(RectTransform from, RectTransform to)
    {
        if (connectionPrefab == null) return;
        
        GameObject connectionObj = Instantiate(connectionPrefab.gameObject, transform);
        connectionObj.transform.SetAsFirstSibling(); // Put connections behind nodes
        
        LineRenderer line = connectionObj.GetComponent<LineRenderer>();
        if (line != null)
        {
            // Convert UI positions to world space
            Vector3 startPos = from.TransformPoint(from.rect.center);
            Vector3 endPos = to.TransformPoint(to.rect.center);
            
            line.positionCount = 2;
            line.SetPosition(0, startPos);
            line.SetPosition(1, endPos);
        }
    }

    // New method to configure UI components
    private void ConfigureUIComponents(GameObject node, NodeType nodeType)
    {
        // Make sure the node has a Button component
        Button button = node.GetComponent<Button>();
        if (button == null)
        {
            button = node.AddComponent<Button>();
            Debug.LogWarning($"[MapGenerator] Added missing Button component to {nodeType} node");
        }
        
        // Set transition mode to color tint to provide visual feedback
        button.transition = Selectable.Transition.ColorTint;
        
        // Make sure it has an Image component (required for Button)
        Image image = node.GetComponent<Image>();
        if (image == null)
        {
            image = node.AddComponent<Image>();
            Debug.LogWarning($"[MapGenerator] Added missing Image component to {nodeType} node");
        }
        
        // Set up the button to call the MapNode's OnNodeClicked method
        button.onClick.RemoveAllListeners();
        MapNode mapNode = node.GetComponent<MapNode>();
        if (mapNode != null)
        {
            button.onClick.AddListener(() => {
                Debug.Log($"[MapGenerator] Button clicked for {nodeType} node");
                mapNode.OnNodeClicked();
            });
        }
        
        // You could set different sprites based on node type
        foreach (var config in nodeTypes)
        {
            if (config.type == nodeType && config.nodeSprite != null)
            {
                image.sprite = config.nodeSprite;
                break;
            }
        }
    }

    private void GeneratePathConnections(GameObject[,] nodeObjects)
    {
        // Occasionally create connections between paths
        for (int nodeIndex = 1; nodeIndex < runLength - 1; nodeIndex++)
        {
            for (int pathIndex = 0; pathIndex < pathsCount - 1; pathIndex++)
            {
                // 30% chance to create a connection
                if (Random.value < 0.3f)
                {
                    CreateNodeConnection(
                        nodeObjects[pathIndex, nodeIndex], 
                        nodeObjects[pathIndex + 1, nodeIndex + 1]
                    );
                    
                    CreateNodeConnection(
                        nodeObjects[pathIndex + 1, nodeIndex], 
                        nodeObjects[pathIndex, nodeIndex + 1]
                    );
                }
            }
        }
    }
    
    private void ConnectNodes()
    {
        // For UI, you might use a different approach for connections
        // This could use UI Lines or Image components stretched between nodes
        Debug.Log("[MapGenerator] Node connections are not yet implemented for UI");
    }
    
    private void CreateNodeConnection(GameObject sourceNode, GameObject targetNode)
    {
        if (sourceNode == null || targetNode == null)
        {
            Debug.LogWarning("[MapGenerator] Cannot create connection - null nodes");
            return;
        }
        
        // Get the MapNode components
        MapNode sourceMapNode = sourceNode.GetComponent<MapNode>();
        MapNode targetMapNode = targetNode.GetComponent<MapNode>();
        
        if (sourceMapNode != null && targetMapNode != null)
        {
            // Add the connection in the node's data
            sourceMapNode.AddConnection(targetMapNode);
            
            Debug.Log($"[MapGenerator] Created connection between nodes at {sourceNode.GetComponent<RectTransform>().anchoredPosition} and {targetNode.GetComponent<RectTransform>().anchoredPosition}");
        }
    }
    
}
