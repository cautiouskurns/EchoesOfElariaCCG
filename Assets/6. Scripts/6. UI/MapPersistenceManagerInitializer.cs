using UnityEngine;

// This script ensures MapPersistenceManager exists in the scene
[DefaultExecutionOrder(-10)] // Make sure this runs before other scripts
public class MapPersistenceManagerInitializer : MonoBehaviour
{
    private void Awake()
    {
        // Just reference the Instance property to trigger auto-creation if needed
        var manager = MapPersistenceManager.Instance;
        Debug.Log("[MapPersistenceManagerInitializer] Ensured MapPersistenceManager exists");
    }
}
