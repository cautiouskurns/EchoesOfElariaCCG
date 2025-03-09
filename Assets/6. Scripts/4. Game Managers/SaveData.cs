using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapSaveData
{
    public int seed; // For deterministic map generation
    public bool mapGenerated = false;
    public List<NodeSaveData> nodes = new List<NodeSaveData>();
    public int pathsCount;
    public int runLength;
    public float pathAngle;
    public Vector2 startNodePosition;
}

[Serializable]
public class NodeSaveData
{
    public int pathIndex;
    public int nodeIndex;
    public Vector2 position;
    public NodeType nodeType;
    public bool visited;
    public List<NodeConnection> connections = new List<NodeConnection>();
}

[Serializable]
public class NodeConnection
{
    public int targetPathIndex;
    public int targetNodeIndex;
}
