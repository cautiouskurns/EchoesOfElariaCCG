using UnityEngine;

public enum NodeType
{
    StandardBattle,
    EliteBattle,
    LoreEvent,
    ShopEvent,
    RestSite,
    BaseCamp
}

public enum BattleType
{
    Standard,
    Elite,
    Boss
}

[System.Serializable]
public class NodeTypeConfig
{
    public NodeType type;
    public GameObject nodePrefab;
    public int minDistance; // Minimum nodes between same type (except standard)
    public float weight = 1.0f; // Likelihood of appearing
    public int maxPerRun = 999; // Maximum number of this node type per run
    public Sprite nodeSprite; // Add this for UI representation
}