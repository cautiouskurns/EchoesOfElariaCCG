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

    [Header("UI Settings")]

[SerializeField] private Color lineColor = new Color(0.6f, 0.6f, 0.6f, 0.8f); // Default gray, semi-transparent
[SerializeField] private float lineThickness = 3f;
    
    private Dictionary<NodeType, NodeTypeConfig> typeConfigMap = new Dictionary<NodeType, NodeTypeConfig>();
    private List<NodeType>[] paths; // Each path has its own sequence of nodes
    private Canvas parentCanvas;
    
    [Header("Starting Position")]
    [SerializeField] private Vector2 startNodePosition = new Vector2(100, 300); // Customize this in inspector

    // Add tracking of nodes by ID
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
        // Subscribe to the map restore event
        if (MapPersistenceManager.Instance != null)
        {
            MapPersistenceManager.Instance.OnRestoreMapRequest += RestoreMap;
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe when disabled
        if (MapPersistenceManager.Instance != null)
        {
            MapPersistenceManager.Instance.OnRestoreMapRequest -= RestoreMap;
        }
    }

    private void Start()
    {
        Debug.Log("[MapGenerator] Start called");
        
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
                    mapNode.BattleSceneName = "Battle1";
                }
                
                Debug.Log($"[MapGenerator] Created test node at {position}");
            }
        }
    }
    private void ConfigureNodeBehavior(MapNode node)
    {
        try
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
                    // Assign a random lore event with error handling
                    DialogueData loreEvent = GetRandomLoreEvent();
                    if (loreEvent == null)
                    {
                        Debug.LogWarning($"[MapGenerator] No lore event available for node {node.GetNodeId()}. Creating a fallback.");
                        loreEvent = CreateFallbackLoreEvent();
                    }
                    node.LoreEvent = loreEvent;
                    Debug.Log($"[MapGenerator] Assigned lore event '{(loreEvent != null ? loreEvent.name : "NULL")}' to node {node.GetNodeId()}");
                    break;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[MapGenerator] Error configuring node behavior: {ex.Message}");
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
        if (loreEventsPool == null || loreEventsPool.Length == 0)
        {
            Debug.LogWarning("[MapGenerator] Lore events pool is null or empty!");
            return null;
        }
        
        // Filter out null entries
        System.Collections.Generic.List<DialogueData> validEvents = new System.Collections.Generic.List<DialogueData>();
        foreach (DialogueData evt in loreEventsPool)
        {
            if (evt != null)
                validEvents.Add(evt);
        }
        
        if (validEvents.Count == 0)
        {
            Debug.LogWarning("[MapGenerator] No valid lore events in the pool!");
            return null;
        }
        
        return validEvents[Random.Range(0, validEvents.Count)];
    }

    // Add this method to create a basic fallback lore event
    private DialogueData CreateFallbackLoreEvent()
    {
        try
        {
            // Create a new dialogue data scriptable object
            DialogueData fallbackEvent = ScriptableObject.CreateInstance<DialogueData>();
            fallbackEvent.name = "Fallback Lore Event";
            
            // Set minimum required properties - adjust these based on your DialogueData structure
            // Assuming DialogueData has these properties:
            // fallbackEvent.title = "Mysterious Encounter";
            // fallbackEvent.dialogueText = "You encounter something strange, but can't quite make out what it is.";
            
            Debug.Log("[MapGenerator] Created fallback lore event");
            return fallbackEvent;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[MapGenerator] Failed to create fallback lore event: {ex.Message}");
            return null;
        }
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
            
            // Skip if first in path (reserved for start nodes)
            if (nodeIndex == 0) 
            {
                paths[pathIndex].Add(NodeType.StandardBattle);
                continue;
            }
            
            // Get eligible node types for this position
            List<NodeType> eligibleTypes = new List<NodeType>();
            List<float> typeWeights = new List<float>();
            
            // Debug logging for node selection
            Debug.Log($"[MapGenerator] Selecting node type for path {pathIndex}, position {nodeIndex}");
            
            foreach (var config in nodeTypes)
            {
                // Check if too close to same type
                bool tooClose = (nodeIndex - lastSeenPosition[config.type] <= config.minDistance);
                // Check if exceeded max count
                bool exceededMax = (typeCounts[config.type] >= config.maxPerRun);
                
                // Log eligibility for this type
                Debug.Log($"[MapGenerator] {config.type} eligible? tooClose={tooClose}, exceededMax={exceededMax}, count={typeCounts[config.type]}/{config.maxPerRun}, weight={config.weight}");
                
                // Special case for LoreEvent - ensure we get enough
                if (config.type == NodeType.LoreEvent)
                {
                    // Reduce distance constraint for lore events
                    tooClose = (nodeIndex - lastSeenPosition[config.type] <= Mathf.Min(1, config.minDistance));
                    
                    // Increase weight based on scarcity
                    float adjustedWeight = config.weight;
                    if (typeCounts[NodeType.LoreEvent] < config.maxPerRun / 2)
                    {
                        adjustedWeight *= 1.5f; // Boost weight if we're below half of max count
                    }
                    
                    if (!tooClose && !exceededMax)
                    {
                        eligibleTypes.Add(config.type);
                        typeWeights.Add(adjustedWeight);
                        Debug.Log($"[MapGenerator] Added LoreEvent with adjusted weight {adjustedWeight}");
                    }
                }
                // Standard battles can be placed anywhere
                else if (config.type == NodeType.StandardBattle || (!tooClose && !exceededMax))
                {
                    eligibleTypes.Add(config.type);
                    typeWeights.Add(config.weight);
                }
            }
            
            // Select a node type based on weights
            NodeType selectedType = SelectWeightedRandom(eligibleTypes, typeWeights);
            Debug.Log($"[MapGenerator] Selected {selectedType} for path {pathIndex}, position {nodeIndex}");
            
            // Update tracking info
            paths[pathIndex].Add(selectedType);
            lastSeenPosition[selectedType] = nodeIndex;
            typeCounts[selectedType]++;
            
            // Log running counts
            foreach (var kvp in typeCounts)
            {
                Debug.Log($"[MapGenerator] Current counts: {kvp.Key}={kvp.Value}");
            }
        }
        
        // Last node is always empty (for boss node)
        paths[pathIndex].Add(NodeType.StandardBattle);
    }
    
    // Log final distribution
    Debug.Log("[MapGenerator] Final node type distribution:");
    foreach (var kvp in typeCounts)
    {
        Debug.Log($"[MapGenerator] {kvp.Key}: {kvp.Value}");
    }
}

private NodeType SelectWeightedRandom(List<NodeType> types, List<float> weights)
{
    if (types.Count == 0) return NodeType.StandardBattle; // Default
    
    // Debug log the options
    for (int i = 0; i < types.Count; i++)
    {
        Debug.Log($"[MapGenerator] Option {i}: {types[i]} with weight {weights[i]}");
    }
    
    float totalWeight = 0f;
    foreach (float weight in weights)
    {
        totalWeight += weight;
    }
    
    float randomValue = Random.Range(0f, totalWeight);
    float cumulativeWeight = 0f;
    
    Debug.Log($"[MapGenerator] Random value: {randomValue} out of total weight {totalWeight}");
    
    for (int i = 0; i < types.Count; i++)
    {
        cumulativeWeight += weights[i];
        Debug.Log($"[MapGenerator] Checking {types[i]}, cumulative weight: {cumulativeWeight}");
        if (randomValue <= cumulativeWeight)
        {
            Debug.Log($"[MapGenerator] Selected {types[i]}");
            return types[i];
        }
    }
    
    Debug.Log($"[MapGenerator] Defaulted to {types[types.Count - 1]}");
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
                
                nodeObjects[pathIndex, nodeIndex] = node;
            }
        }
        
        // Connect nodes within each path
        ConnectPathNodes(nodeObjects);
        
        // Generate connections between paths occasionally
        GeneratePathConnections(nodeObjects);
        
        // Connect base camp to first tier nodes
        ConnectBaseCampToFirstTier(baseCampNode, nodeObjects);

        // After placing all nodes and creating connections
        SaveMapStructure();
    }

    // Add this new method for connecting nodes along the same path
    private void ConnectPathNodes(GameObject[,] nodeObjects)
    {
        // Connect nodes in sequence within each path
        for (int pathIndex = 0; pathIndex < pathsCount; pathIndex++)
        {
            for (int nodeIndex = 1; nodeIndex < runLength - 1; nodeIndex++)
            {
                // Connect current node to next node in path
                GameObject currentNode = nodeObjects[pathIndex, nodeIndex];
                GameObject nextNode = nodeObjects[pathIndex, nodeIndex + 1];
                
                if (currentNode != null && nextNode != null)
                {
                    // Create the connection
                    CreateNodeConnection(currentNode, nextNode, true);
                    
                    // Log the connection
                    Debug.Log($"[MapGenerator] Connected path {pathIndex} node {nodeIndex} to node {nodeIndex + 1}");
                }
            }
        }
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
        // Create a UI line using an Image
        GameObject lineObj = new GameObject("UILine");
        lineObj.transform.SetParent(transform, false);
        
        // Add image component
        Image lineImage = lineObj.AddComponent<Image>();
        lineImage.color = lineColor; // Use the inspector-configurable color
        
        // Create a 1x1 white texture if needed
        if (lineImage.sprite == null)
        {
            // Use Unity's built-in white texture
            lineImage.sprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0, 0, 4, 4),
                new Vector2(0.5f, 0.5f)
            );
        }
        
        // Get rect transform and set proper anchoring
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();
        
        // These settings are critical for proper orientation
        lineRect.anchorMin = new Vector2(0, 0);
        lineRect.anchorMax = new Vector2(0, 0);
        lineRect.pivot = new Vector2(0, 0.5f); // Pivot at left center for correct rotation
        
        // Calculate positions and distances
        Vector2 fromAnchoredPos = from.anchoredPosition;
        Vector2 toAnchoredPos = to.anchoredPosition;
        Vector2 direction = toAnchoredPos - fromAnchoredPos;
        float distance = direction.magnitude;
        
        // Set position to start point
        lineRect.anchoredPosition = fromAnchoredPos;
        
        // Set width/height (width is the distance, height is the thickness)
        lineRect.sizeDelta = new Vector2(distance, lineThickness); // Use the inspector-configurable thickness
        
        // Calculate angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.localEulerAngles = new Vector3(0, 0, angle);
        
        // Ensure lines are drawn behind nodes
        lineRect.SetAsFirstSibling();
        
        Debug.Log($"[MapGenerator] Created UI line from {fromAnchoredPos} to {toAnchoredPos}, distance: {distance}, angle: {angle}");
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
        
        // Special handling for BaseCamp node to fix white background issue
        if (nodeType == NodeType.BaseCamp)
        {
            // Disable default transition visual - we'll handle this differently
            button.transition = Selectable.Transition.None;
            
            // Find ALL Image components on the node and its children
            Image[] allImages = node.GetComponentsInChildren<Image>(true);
            
            Debug.Log($"[MapGenerator] Found {allImages.Length} images in Base Camp node");
            
            foreach (Image img in allImages)
            {
                // Check if this is a background image (usually the one directly on the node)
                if (img.gameObject == node && (img.sprite == null || img.sprite.name == "Background" || img.sprite.name == "UISprite"))
                {
                    // This is likely the background - make it fully transparent
                    img.color = new Color(0, 0, 0, 0);
                    Debug.Log("[MapGenerator] Made base camp background transparent");
                }
                // If it's a child with a proper sprite, keep it visible
                else if (img.sprite != null && img.sprite.name != "Background" && img.sprite.name != "UISprite")
                {
                    // This is likely an icon or meaningful image - keep it visible
                    img.color = Color.white;
                    Debug.Log($"[MapGenerator] Keeping sprite '{img.sprite.name}' visible");
                }
                // Any other images - make transparent
                else
                {
                    img.color = new Color(0, 0, 0, 0);
                    Debug.Log($"[MapGenerator] Made extra image transparent: {img.gameObject.name}");
                }
            }
        }
        else
        {
            // For other nodes, use standard button behavior
            button.transition = Selectable.Transition.ColorTint;
            
            // Make sure it has an Image component (required for Button)
            Image image = node.GetComponent<Image>();
            // if (image == null)
            // {
            //     image = node.AddComponent<Image>();
            //     Debug.LogWarning($"[MapGenerator] Added missing Image component to {nodeType} node");
            // }
            
            // Set node sprite based on type
            foreach (var config in nodeTypes)
            {
                if (config.type == nodeType && config.nodeSprite != null)
                {
                    image.sprite = config.nodeSprite;
                    image.preserveAspect = true;
                    break;
                }
            }
        }
        
        // Set up button click handler for all nodes
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
                // 30% chance to create a diagonal connection
                if (Random.value < 0.3f)
                {
                    // Connect to node diagonally forward-up
                    if (nodeObjects[pathIndex, nodeIndex] != null && 
                        nodeObjects[pathIndex + 1, nodeIndex + 1] != null)
                    {
                        CreateNodeConnection(
                            nodeObjects[pathIndex, nodeIndex], 
                            nodeObjects[pathIndex + 1, nodeIndex + 1],
                            true // Visualize this connection
                        );
                    }
                    
                    // Connect to node diagonally forward-down
                    if (nodeObjects[pathIndex + 1, nodeIndex] != null && 
                        nodeObjects[pathIndex, nodeIndex + 1] != null)
                    {
                        CreateNodeConnection(
                            nodeObjects[pathIndex + 1, nodeIndex], 
                            nodeObjects[pathIndex, nodeIndex + 1],
                            true // Visualize this connection
                        );
                    }
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
    
    private void CreateNodeConnection(GameObject sourceNode, GameObject targetNode, bool visualize = false)
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
            targetMapNode.AddConnection(sourceMapNode); // Make connection bidirectional
            
            // Visualize the connection if requested
            if (visualize)
            {
                DrawConnectionLine(sourceNode.GetComponent<RectTransform>(), 
                                targetNode.GetComponent<RectTransform>());
            }
            
            Debug.Log($"[MapGenerator] Created connection between nodes at {sourceNode.GetComponent<RectTransform>().anchoredPosition} and {targetNode.GetComponent<RectTransform>().anchoredPosition}");
        }
    }

    // Add this method to restore the map
    private void RestoreMap()
    {
        Debug.Log("[MapGenerator] Restoring map...");
        
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
                mapNode.SetNodeId(nodeData.id);
                
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
        
        // When restoring nodes, specifically check LoreEvent nodes and ensure they have events
        foreach (SerializedNodeData nodeData in savedNodes)
        {
            // ...existing node creation code...
            
            // After setting basic node properties:
            if (nodesById.ContainsKey(nodeData.id))
            {
                MapNode mapNode = nodesById[nodeData.id];
                if (mapNode != null && nodeData.nodeType == NodeType.LoreEvent)
                {
                    // Ensure Lore nodes have valid DialogueData
                    if (mapNode.LoreEvent == null)
                    {
                        DialogueData loreEvent = GetRandomLoreEvent();
                        if (loreEvent == null)
                            loreEvent = CreateFallbackLoreEvent();
                            
                        mapNode.LoreEvent = loreEvent;
                        Debug.Log($"[MapGenerator] Fixed missing LoreEvent for restored node {nodeData.id}");
                    }
                }
            }
        }
        
        Debug.Log("[MapGenerator] Map restoration complete");
    }
    
    // Add this method to clear existing nodes
    private void ClearExistingNodes()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        nodesById.Clear();
    }
    
    // Add this method to get the prefab for a node type
    private GameObject GetNodePrefab(NodeType type)
    {
        if (type == NodeType.BaseCamp)
            return baseCampNodePrefab;
        
        // For other types, check configs
        foreach (var config in nodeTypes)
        {
            if (config.type == type)
            {
                return config.nodePrefab;
            }
        }
        
        return null;
    }

    // Add this method to save map structure
    private void SaveMapStructure()
    {
        if (MapPersistenceManager.Instance == null)
        {
            Debug.LogError("[MapGenerator] Cannot save map - no MapPersistenceManager found");
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
                
                // Create connection list
                List<string> connections = new List<string>();
                foreach (MapNode connectedNode in node.GetConnectedNodes())
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
                    visited = node.HasBeenVisited,
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
    
}
