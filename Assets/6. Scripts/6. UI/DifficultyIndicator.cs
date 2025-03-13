using UnityEngine;
using UnityEngine.UI;

public class DifficultyIndicator : MonoBehaviour
{
    [SerializeField] private GameObject skullIconPrefab; // Drag your skull sprite prefab here
    [SerializeField] private Transform iconContainer;
    [SerializeField] private float iconSpacing = 15f;
    
    private MapNode parentNode;
    
    private void Awake()
    {
        parentNode = GetComponentInParent<MapNode>();
        
        if (iconContainer == null)
        {
            // Create container if it doesn't exist
            iconContainer = new GameObject("SkullContainer").transform;
            iconContainer.SetParent(transform);
            iconContainer.localPosition = Vector3.zero;
        }
    }
    
    private void Start()
    {
        UpdateSkullIcons();
    }
    
    public void UpdateSkullIcons()
    {
        // Clear any existing icons
        foreach (Transform child in iconContainer)
        {
            Destroy(child.gameObject);
        }
        
        if (parentNode == null) return;
        
        // Skip for non-battle nodes
        if (parentNode.NodeType != NodeType.StandardBattle && 
            parentNode.NodeType != NodeType.EliteBattle)
            return;
        
        // Create skulls based on difficulty
        int skullCount = GetSkullCountForDifficulty(parentNode.Difficulty);
        
        for (int i = 0; i < skullCount; i++)
        {
            GameObject skull = Instantiate(skullIconPrefab, iconContainer);
            RectTransform skullRT = skull.GetComponent<RectTransform>();
            
            // Position horizontally
            float xOffset = (i - (skullCount - 1) / 2.0f) * iconSpacing;
            skullRT.anchoredPosition = new Vector2(xOffset, 30f); // Position above the node
        }
    }
    
    private int GetSkullCountForDifficulty(NodeDifficulty difficulty)
    {
        switch (difficulty)
        {
            case NodeDifficulty.Easy: return 1;
            case NodeDifficulty.Medium: return 2;
            case NodeDifficulty.Hard: return 3;
            case NodeDifficulty.Elite: return 4;
            case NodeDifficulty.Boss: return 5;
            default: return 1;
        }
    }
}