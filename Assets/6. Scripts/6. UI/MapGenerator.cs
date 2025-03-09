using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Configuration")]
    [SerializeField] private int runLength = 15; // Total nodes in the run
    [SerializeField] private int pathsCount = 3; // Number of parallel paths
    [SerializeField] private float horizontalSpacing = 150f; // Space between nodes horizontally (UI units)
    [SerializeField] private float verticalSpacing = 200f; // Space between paths vertically (UI units)
    [SerializeField] private float nodeJitterAmount = 20f; // Random offset for natural look (UI units)

    [Header("Path Direction")]
    [SerializeField] private Vector2 pathDirection = new Vector2(1, 0); // Default horizontal (1,0)
    [SerializeField] [Range(0, 360)] private float pathAngle = 0f; // Alternative angle-based control
    
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
    [SerializeField] private Vector2 baseCampPosition; //= new Vector2(50, 300);

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

    private bool mapGeneratedThisSession = false;

    // Track all created nodes by ID
    private Dictionary<string, MapNode> nodesById = new Dictionary<string, MapNode>();

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

    private void OnEnable()
    {
        // Subscribe to GameManager's return event
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnReturnToMap += OnReturnToMap;
        }

        // Subscribe to the map restore event
        if (MapPersistenceManager.Instance != null)
        {
            MapPersistenceManager.Instance.OnRestoreMapRequest += RestoreMap;
        }
    }
    
    private void OnDisable()
    {
        // Clean up event subscription
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnReturnToMap -= OnReturnToMap;
        }

        // Unsubscribe when disabled
        if (MapPersistenceManager.Instance != null)
        {
            MapPersistenceManager.Instance.OnRestoreMapRequest -= RestoreMap;
        }
    }
    
    private void OnReturnToMap()
    {
        Debug.Log("[MapGenerator] OnReturnToMap event received");
        RestoreMap();
    }

    private void Start()
    {
        Debug.Log("[MapGenerator] Start called");
        
        // Check if we're returning from another scene
        if (GameManager.Instance != null && GameManager.Instance.IsReturningToMap)
        {
            Debug.Log("[MapGenerator] Detected returning to map - waiting for event");
            // Do nothing - we'll handle this via the event
        }
        // If we haven't generated a map yet this session and there's no saved data, generate a new one
        else if (!mapGeneratedThisSession)
        {
            Debug.Log("[MapGenerator] First time creating map this session");
            
            // See if we have saved map data
            if (GameManager.Instance != null && GameManager.Instance.MapGenerated)
            {
                Debug.Log("[MapGenerator] Restoring existing map");
                RestoreMap();
            }
            else
            {
                Debug.Log("[MapGenerator] Generating new map");
                GenerateNewMap();
                
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.MapGenerated = true;
                }
            }
            
            mapGeneratedThisSession = true;
        }

        // Check if we need to restore a map
        if (MapPersistenceManager.Instance != null && MapPersistenceManager.Instance.HasSavedMap())
        {
            Debug.Log("[MapGenerator] Found saved map data, restoring...");
            RestoreMap();
        }
        else
        {
            Debug.Log("[MapGenerator] No saved map found, generating new map");
            GenerateNewMap();
        }
    }

    public void GenerateNewMap()
    {
        Debug.Log("[MapGenerator] GenerateNewMap called");
        
        // Clear any existing nodes
        ClearExistingNodes();
        
        // Generate a new map
        GenerateMap();
        
        // Save the map structure
        SaveMapStructure();
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
                    mapNode.BattleSceneName = "Battle1";
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
        
        // Use pathAngle to determine direction
        float angleInRadians = pathAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;
        
        // Calculate step size based on the path direction
        float stepSize = horizontalSpacing; // Base step size
        
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
            // Calculate the perpendicular offset for this path
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            float pathOffset = (pathIndex - pathsCount/2) * verticalSpacing;
            Vector2 pathStartPos = startNodePosition + (perpendicular * pathOffset);
            
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
                
                // Calculate position along the path direction with jitter
                float xJitter = Random.Range(-nodeJitterAmount, nodeJitterAmount);
                float yJitter = Random.Range(-nodeJitterAmount, nodeJitterAmount);
                
                // Move in the path direction
                Vector2 nodePos = pathStartPos + (direction * nodeIndex * stepSize);
                
                // Add jitter (perpendicular and parallel to the path)
                Vector2 jitter = (perpendicular * xJitter) + (direction * yJitter);
                nodePos += jitter;
                
                // Instantiate node as UI element
                GameObject node = Instantiate(nodePrefab, transform);
                RectTransform rectTransform = node.GetComponent<RectTransform>();
                
                // Position the node using anchoredPosition (UI positioning)
                rectTransform.anchoredPosition = nodePos;
                
                Debug.Log($"[MapGenerator] Placed {nodeType} node at UI position ({nodePos.x}, {nodePos.y})");
                
                // Inside PlaceNodesOnMap, when configuring nodes:
                MapNode mapNode = node.GetComponent<MapNode>();
                if (mapNode != null)
                {
                    mapNode.NodeType = nodeType;
                    mapNode.PathIndex = pathIndex;
                    mapNode.NodeIndex = nodeIndex;
                    ConfigureNodeBehavior(mapNode);
                }
                                
                nodeObjects[pathIndex, nodeIndex] = node;
            }
        }
        
        // Generate connections between paths occasionally
        GeneratePathConnections(nodeObjects);
        ConnectBaseCampToFirstTier(baseCampNode, nodeObjects);

        // After creating all nodes and setting up connections
        SaveMapStructure();
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
        // Get button component
        Button button = node.GetComponent<Button>();
        if (button == null)
        {
            button = node.AddComponent<Button>();
        }
        
        // Special handling for BaseCamp - it has a different structure
        if (nodeType == NodeType.BaseCamp)
        {
            // For base camp, find the image that should be transparent
            Image[] allImages = node.GetComponentsInChildren<Image>(true);
            foreach (Image img in allImages)
            {
                // Make the background transparent but keep other images
                if (img.gameObject == node)
                {
                    img.color = new Color(1, 1, 1, 0); // Fully transparent
                }
            }
            
            button.transition = Selectable.Transition.None; // Use custom visuals
        }
        else
        {
            // For regular nodes, handle normally
            Image image = node.GetComponent<Image>();
            if (image == null)
            {
                image = node.AddComponent<Image>();
            }
            
            // Set up sprite and color
            foreach (var config in nodeTypes)
            {
                if (config.type == nodeType && config.nodeSprite != null)
                {
                    image.sprite = config.nodeSprite;
                    image.color = Color.white; // Reset to fully visible
                    image.preserveAspect = true;
                    break;
                }
            }
            
            button.transition = Selectable.Transition.ColorTint;
        }
        
        // Set up click handler
        button.onClick.RemoveAllListeners();
        MapNode mapNode = node.GetComponent<MapNode>();
        if (mapNode != null)
        {
            button.onClick.AddListener(() => {
                Debug.Log($"[MapGenerator] Button clicked for {nodeType} node");
                mapNode.OnNodeClicked();
            });
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

    // Add this method to restore the map
    private void RestoreMap()
    {
        Debug.Log("[MapGenerator] Restoring map...");
        
        // Try MapPersistenceManager first
        if (MapPersistenceManager.Instance != null && MapPersistenceManager.Instance.HasSavedMap())
        {
            RestoreMapFromPersistenceManager();
        }
        // Then try GameManager as fallback
        else if (GameManager.Instance != null && GameManager.Instance.MapGenerated)
        {
            RestoreMapFromGameManager();
        }
        // If nothing works, generate a new map
        else
        {
            Debug.LogWarning("[MapGenerator] No saved map found in any manager, generating new map");
            GenerateNewMap();
        }
    }

    private void RestoreMapFromPersistenceManager()
    {
        if (MapPersistenceManager.Instance == null || !MapPersistenceManager.Instance.HasSavedMap())
        {
            Debug.LogError("[MapGenerator] Cannot restore map - no saved data");
            return;
        }
        
        // Clear existing nodes
        ClearExistingNodes();
        
        // Get the saved node data
        List<SerializedNodeData> savedNodes = MapPersistenceManager.Instance.GetMapData();
        
        if (savedNodes == null || savedNodes.Count == 0)
        {
            Debug.LogError("[MapGenerator] No nodes in saved data");
            return;
        }
        
        Debug.Log($"[MapGenerator] Restoring {savedNodes.Count} nodes");
        
        // First pass: Create all nodes
        foreach (SerializedNodeData nodeData in savedNodes)
        {
            // Get the appropriate prefab
            GameObject prefab = GetNodePrefab(nodeData.nodeType);
            if (prefab == null)
            {
                Debug.LogError($"[MapGenerator] No prefab found for node type {nodeData.nodeType}");
                continue;
            }
            
            // Create the node
            GameObject nodeObject = Instantiate(prefab, transform);
            RectTransform rt = nodeObject.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(nodeData.xPos, nodeData.yPos);
            
            // Configure the node
            MapNode mapNode = nodeObject.GetComponent<MapNode>();
            if (mapNode != null)
            {
                // Set basic properties
                mapNode.NodeType = nodeData.nodeType;
                mapNode.PathIndex = nodeData.pathIndex;
                mapNode.NodeIndex = nodeData.nodeIndex;
                mapNode.BattleSceneName = nodeData.battleSceneName;
                mapNode.EventSceneName = nodeData.eventSceneName;
                mapNode.SetNodeId(nodeData.id); // Explicitly set the ID to match saved data
                
                // Store reference for connections
                nodesById[nodeData.id] = mapNode;
                
                // Apply visited state
                mapNode.SetVisited(nodeData.visited);
                
                // Configure UI components
                ConfigureUIComponents(nodeObject, nodeData.nodeType);
            }
            
            Debug.Log($"[MapGenerator] Restored node {nodeData.id} at ({nodeData.xPos}, {nodeData.yPos})");
        }
        
        // Second pass: Restore connections
        foreach (SerializedNodeData nodeData in savedNodes)
        {
            if (!nodesById.ContainsKey(nodeData.id)) continue;
            
            MapNode sourceNode = nodesById[nodeData.id];
            
            foreach (string targetId in nodeData.connectedNodeIds)
            {
                if (nodesById.ContainsKey(targetId))
                {
                    MapNode targetNode = nodesById[targetId];
                    sourceNode.AddConnection(targetNode);
                    
                    // Draw a visual line between them
                    DrawConnectionLine(
                        sourceNode.GetComponent<RectTransform>(),
                        targetNode.GetComponent<RectTransform>()
                    );
                    
                    Debug.Log($"[MapGenerator] Restored connection from {nodeData.id} to {targetId}");
                }
            }
        }
        
        Debug.Log("[MapGenerator] Map restoration complete");
    }

    private void RestoreMapFromGameManager()
    {
        Debug.Log("[MapGenerator] Restoring map from GameManager");
        
        // Clear existing nodes
        ClearExistingNodes();
        
        // Get all saved nodes
        List<MapNodeData> savedNodes = GameManager.Instance.GetAllNodeData();
        
        if (savedNodes == null || savedNodes.Count == 0)
        {
            Debug.LogError("[MapGenerator] No nodes found in GameManager");
            GenerateNewMap();
            return;
        }
        
        // Create dictionary for ID lookup
        Dictionary<string, MapNode> createdNodes = new Dictionary<string, MapNode>();
        
        // First phase - create all nodes
        foreach (var nodeData in savedNodes)
        {
            GameObject prefab = GetNodePrefab(nodeData.NodeType);
            if (prefab == null) continue;
            
            Vector2 position = CalculateNodePosition(nodeData.PathIndex, nodeData.NodeIndex);
            
            GameObject nodeObj = Instantiate(prefab, transform);
            RectTransform rt = nodeObj.GetComponent<RectTransform>();
            rt.anchoredPosition = position;
            
            MapNode mapNode = nodeObj.GetComponent<MapNode>();
            if (mapNode != null)
            {
                mapNode.SetNodeId(nodeData.NodeId);
                mapNode.NodeType = nodeData.NodeType;
                mapNode.PathIndex = nodeData.PathIndex;
                mapNode.NodeIndex = nodeData.NodeIndex;
                mapNode.SetVisited(nodeData.Visited);
                
                createdNodes[nodeData.NodeId] = mapNode;
                nodesById[nodeData.NodeId] = mapNode;
                
                ConfigureNodeBehavior(mapNode);
                ConfigureUIComponents(nodeObj, nodeData.NodeType);
            }
        }
        
        // Second phase - recreate connections
        foreach (var nodeData in savedNodes)
        {
            if (!createdNodes.ContainsKey(nodeData.NodeId)) continue;
            
            MapNode sourceNode = createdNodes[nodeData.NodeId];
            
            foreach (string targetId in nodeData.ConnectedNodeIds)
            {
                if (createdNodes.ContainsKey(targetId))
                {
                    MapNode targetNode = createdNodes[targetId];
                    sourceNode.AddConnection(targetNode);
                    
                    DrawConnectionLine(
                        sourceNode.GetComponent<RectTransform>(),
                        targetNode.GetComponent<RectTransform>()
                    );
                }
            }
        }
        
        Debug.Log($"[MapGenerator] Restored {createdNodes.Count} nodes from GameManager");
    }

    // Helper method to get all node data
    private List<MapNodeData> GetAllNodeData()
    {
        List<MapNodeData> result = new List<MapNodeData>();
        
        // This assumes GameManager has a method to get all nodes
        // You'll need to implement this in GameManager
        if (GameManager.Instance != null)
        {
            result = GameManager.Instance.GetAllNodeData();
        }
        
        return result;
    }

    // Helper method to calculate node position
    private Vector2 CalculateNodePosition(int pathIndex, int nodeIndex)
    {
        // Use the same logic as in PlaceNodesOnMap
        float angleInRadians = pathAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);
        
        float pathOffset = (pathIndex - pathsCount/2) * verticalSpacing;
        Vector2 pathStartPos = startNodePosition + (perpendicular * pathOffset);
        
        // Calculate position along the path direction with jitter
        // We can't reproduce exact jitter, so use a deterministic approach
        float xJitter = Mathf.PerlinNoise(pathIndex * 0.3f, nodeIndex * 0.3f) * nodeJitterAmount * 2 - nodeJitterAmount;
        float yJitter = Mathf.PerlinNoise(pathIndex * 0.7f, nodeIndex * 0.7f) * nodeJitterAmount * 2 - nodeJitterAmount;
        
        Vector2 nodePos = pathStartPos + (direction * nodeIndex * horizontalSpacing);
        Vector2 jitter = (perpendicular * xJitter) + (direction * yJitter);
        
        return nodePos + jitter;
    }

    // Helper method to get the prefab for a node type
    private GameObject GetNodePrefab(NodeType type)
    {
        if (type == NodeType.BaseCamp)
        {
            return baseCampNodePrefab;
        }
        
        foreach (var config in nodeTypes)
        {
            if (config.type == type)
            {
                return config.nodePrefab;
            }
        }
        
        return null;
    }

    // Method to save current map structure
    private void SaveMapStructure()
    {
        // Check if MapPersistenceManager exists, if not use GameManager fallback
        if (MapPersistenceManager.Instance == null)
        {
            Debug.LogWarning("[MapGenerator] MapPersistenceManager not found, using GameManager for persistence instead");
            SaveMapToGameManager();
            return;
        }
        
        List<SerializedNodeData> nodeDataList = new List<SerializedNodeData>();
        
        // Go through each child (node) of this object
        foreach (Transform child in transform)
        {
            MapNode node = child.GetComponent<MapNode>();
            if (node != null)
            {
                // Get the node's position in UI space
                RectTransform rt = child.GetComponent<RectTransform>();
                Vector2 position = rt.anchoredPosition;
                
                // Create connection list - FIXED: Changed from GetConnectedNodes to GetConnections
                List<string> connections = new List<string>();
                foreach (MapNode connectedNode in node.GetConnections())
                {
                    if (connectedNode != null)
                    {
                        connections.Add(connectedNode.GetNodeId());
                    }
                }
                
                // Create serialized data
                SerializedNodeData nodeData = new SerializedNodeData
                {
                    id = node.GetNodeId(),
                    nodeType = node.NodeType,
                    xPos = position.x,
                    yPos = position.y,
                    pathIndex = node.PathIndex,
                    nodeIndex = node.NodeIndex,
                    visited = node.IsVisited(),
                    connectedNodeIds = connections,
                    battleSceneName = node.BattleSceneName,
                    eventSceneName = node.EventSceneName
                };
                
                nodeDataList.Add(nodeData);
            }
        }
        
        // Save to the persistence manager
        MapPersistenceManager.Instance.SaveMapStructure(nodeDataList);
        Debug.Log($"[MapGenerator] Saved map structure with {nodeDataList.Count} nodes");
    }

    // Fallback method to save map using GameManager
    private void SaveMapToGameManager()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[MapGenerator] GameManager not found either! Map state will not persist.");
            return;
        }
        
        // Clear existing node states
        GameManager.Instance.ClearMapData();
        GameManager.Instance.MapGenerated = true;
        
        // Save each node's state
        foreach (Transform child in transform)
        {
            MapNode node = child.GetComponent<MapNode>();
            if (node != null)
            {
                List<string> connectedNodeIds = new List<string>();
                foreach (MapNode connectedNode in node.GetConnections())
                {
                    if (connectedNode != null)
                    {
                        connectedNodeIds.Add(connectedNode.GetNodeId());
                    }
                }
                
                GameManager.Instance.SaveNodeState(
                    node.GetNodeId(), 
                    node.NodeType, 
                    node.IsVisited(),
                    node.PathIndex,
                    node.NodeIndex,
                    connectedNodeIds
                );
            }
        }
        
        Debug.Log($"[MapGenerator] Saved map using GameManager fallback");
    }

    private void ClearExistingNodes()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        nodesById.Clear();
    }
}
