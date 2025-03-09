using UnityEngine;

// Add this script to the map scene to ensure the MapPersistenceManager exists
[DefaultExecutionOrder(-100)] // Make sure this runs before other scripts
public class MapPersistenceManagerInitializer : MonoBehaviour
{
    private void Awake()
    {
        // Just reference the instance to trigger auto-creation if needed
        var manager = MapPersistenceManager.Instance;
        Debug.Log("[MapPersistenceManagerInitializer] Ensured MapPersistenceManager exists");
    }
}
