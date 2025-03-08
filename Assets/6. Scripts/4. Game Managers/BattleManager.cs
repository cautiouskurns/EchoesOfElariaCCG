using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    
    [Header("Player Configuration")]
    [SerializeField] private Transform[] playerSpawnPoints;
    [SerializeField] private int maxPlayersToSpawn = 2; // Explicitly limit number of players
    
    [Header("Enemy Configuration")]
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private EnemyClass[] standardEnemiesPool;
    [SerializeField] private EnemyClass[] eliteEnemiesPool;
    [SerializeField] private EnemyClass bossEnemy;
    
    private List<EnemyUnit> enemyUnits = new List<EnemyUnit>();
    private List<PlayerUnit> playerUnits = new List<PlayerUnit>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[BattleManager] ❌ GameManager not found! Cannot setup battle.");
            return;
        }

            // Check selected classes
        if (GameManager.Instance.selectedClasses != null)
        {
            Debug.Log($"[BattleManager] Selected classes array length: {GameManager.Instance.selectedClasses.Length}");
            for (int i = 0; i < GameManager.Instance.selectedClasses.Length; i++)
            {
                Debug.Log($"[BattleManager] Class at index {i}: {(GameManager.Instance.selectedClasses[i] != null ? GameManager.Instance.selectedClasses[i].className : "NULL")}");
            }
        }

        // Spawn players first
        SpawnPlayers();
        
        // Then spawn appropriate enemies based on battle type
        InitializeEnemies();
        
        // Start monitoring battle outcome
        StartCoroutine(MonitorBattleOutcome());
    }
    
    private void InitializeEnemies()
    {
        // Get battle information from GameManager
        BattleType battleType = GameManager.Instance.CurrentBattleType;
        EnemyClass[] predefinedEnemies = GameManager.Instance.CurrentEnemies;
        
        // Spawn appropriate enemies
        if (predefinedEnemies != null && predefinedEnemies.Length > 0)
        {
            // Use the specific enemies passed from the map
            SpawnSpecificEnemies(predefinedEnemies);
        }
        else
        {
            // Use random enemies based on battle type
            switch (battleType)
            {
                case BattleType.Standard:
                    SpawnRandomEnemies(standardEnemiesPool, 2, 3); // 2-3 standard enemies
                    break;
                    
                case BattleType.Elite:
                    SpawnRandomEnemies(eliteEnemiesPool, 1, 2); // 1-2 elite enemies
                    break;
                    
                case BattleType.Boss:
                    SpawnBoss();
                    break;
                    
                default:
                    Debug.LogWarning($"[BattleManager] Unhandled battle type: {battleType}. Using standard battle.");
                    SpawnRandomEnemies(standardEnemiesPool, 2, 2);
                    break;
            }
        }
        
        // Make sure we have all enemies in our list
        FindEnemies();
    }

    private void SpawnPlayers()
    {
        // Clear any existing player units from previous battles
        foreach (var unit in playerUnits)
        {
            if (unit != null) Destroy(unit.gameObject);
        }
        playerUnits.Clear();

        CharacterClass[] selectedClasses = GameManager.Instance.selectedClasses;
        
        if (selectedClasses == null || selectedClasses.Length == 0)
        {
            Debug.LogError("[BattleManager] No character classes selected!");
            return;
        }

        // Debug log to check classes array
        Debug.Log($"[BattleManager] Selected classes array length: {selectedClasses.Length}");
        for (int i = 0; i < selectedClasses.Length; i++)
        {
            Debug.Log($"[BattleManager] Class at index {i}: {(selectedClasses[i] != null ? selectedClasses[i].className : "NULL")}");
        }

        // Track used spawn points to avoid duplicates
        HashSet<Transform> usedSpawnPoints = new HashSet<Transform>();
        
        // Only spawn up to the maximum number of players
        int spawnedCount = 0;
        for (int i = 0; i < selectedClasses.Length && spawnedCount < maxPlayersToSpawn; i++)
        {
            // Skip null classes
            if (selectedClasses[i] == null) continue;
            
            // Use the class-specific prefab variant
            GameObject playerPrefab = selectedClasses[i].classPrefab;

            if (playerPrefab == null)
            {
                Debug.LogError($"[BattleManager] ❌ No prefab found for {selectedClasses[i].className}");
                continue;
            }

            // Find an unused spawn point
            Transform spawnPoint = null;
            if (spawnedCount < playerSpawnPoints.Length)
            {
                spawnPoint = playerSpawnPoints[spawnedCount];
                
                // Verify it's not null and not already used
                if (spawnPoint == null || usedSpawnPoints.Contains(spawnPoint))
                {
                    // Create a fallback spawn point
                    spawnPoint = new GameObject($"PlayerSpawn_{spawnedCount}").transform;
                    spawnPoint.position = new Vector3(-3 + spawnedCount * 1.5f, 0, 0);
                    Debug.LogWarning($"[BattleManager] ⚠ Creating fallback spawn point for Player {i+1}");
                }
            }
            else
            {
                // Create additional spawn point
                spawnPoint = new GameObject($"PlayerSpawn_{spawnedCount}").transform;
                spawnPoint.position = new Vector3(-3 + spawnedCount * 1.5f, 0, 0);
                Debug.LogWarning($"[BattleManager] ⚠ Creating additional spawn point for Player {i+1}");
            }

            usedSpawnPoints.Add(spawnPoint);
            
            // Log the spawn details
            Debug.Log($"[BattleManager] Spawning {selectedClasses[i].className} at position {spawnPoint.position}");

            GameObject playerObj = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
            PlayerUnit playerUnit = playerObj.GetComponent<PlayerUnit>();

            if (playerUnit != null)
            {
                playerUnit.InitializeFromClass(selectedClasses[i]);
                playerUnits.Add(playerUnit);
                Debug.Log($"[BattleManager] ✅ Spawned {selectedClasses[i].className} at position {spawnPoint.position}");
                spawnedCount++;
            }
            else
            {
                Debug.LogError($"[BattleManager] ❌ {playerPrefab.name} is missing a PlayerUnit component!");
            }
        }
        
        Debug.Log($"[BattleManager] Spawned {spawnedCount} players (maximum set to {maxPlayersToSpawn})");
    }

    private void SpawnSpecificEnemies(EnemyClass[] enemiesToSpawn)
    {
        enemyUnits.Clear();
        
        // Log all enemy spawn points for debugging
        Debug.Log($"[BattleManager] Enemy spawn points: {enemySpawnPoints.Length}");
        
        // Track used spawn points
        HashSet<Transform> usedSpawnPoints = new HashSet<Transform>();
        
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            if (enemiesToSpawn[i] == null) continue;
            
            Transform spawnPoint = null;
            Vector3 spawnPosition;
            
            if (i < enemySpawnPoints.Length && enemySpawnPoints[i] != null && !usedSpawnPoints.Contains(enemySpawnPoints[i]))
            {
                spawnPoint = enemySpawnPoints[i];
                spawnPosition = spawnPoint.position;
                usedSpawnPoints.Add(spawnPoint);
            }
            else
            {
                // Create a fallback position
                spawnPosition = new Vector3(3 + i * 1.5f, 0, 0);
                Debug.LogWarning($"[BattleManager] Using fallback position for enemy {i}: {spawnPosition}");
            }
            
            SpawnEnemyAtPosition(enemiesToSpawn[i], spawnPosition);
        }
    }
    
    private void SpawnRandomEnemies(EnemyClass[] pool, int min, int max)
    {
        if (pool == null || pool.Length == 0)
        {
            Debug.LogError("[BattleManager] ❌ Enemy pool is empty!");
            return;
        }

        enemyUnits.Clear();

        int enemyCount = Random.Range(min, max + 1);
        enemyCount = Mathf.Clamp(enemyCount, min, enemySpawnPoints.Length);  // Ensure proper range

        Debug.Log($"[BattleManager] Attempting to spawn {enemyCount} enemies.");

        for (int i = 0; i < enemyCount; i++)
        {
            if (i >= enemySpawnPoints.Length) break;

            EnemyClass randomEnemy = pool[Random.Range(0, pool.Length)];
            SpawnEnemyAtPosition(randomEnemy, enemySpawnPoints[i].position);
        }
    }

    private void SpawnBoss()
    {
        if (bossEnemy == null)
        {
            Debug.LogError("[BattleManager] No boss enemy assigned!");
            return;
        }
        
        enemyUnits.Clear();
        
        // Spawn in center position
        SpawnEnemyAtPosition(bossEnemy, enemySpawnPoints[0].position);
    }
    
    private void SpawnEnemyAtPosition(EnemyClass enemyClass, Vector3 position)
    {
        GameObject enemyObj = Instantiate(enemyClass.enemyPrefab, position, Quaternion.identity);
        
        if (enemyObj.TryGetComponent<EnemyUnit>(out var enemy))
        {
            enemy.InitializeFromClass(enemyClass);
            enemyUnits.Add(enemy);
            Debug.Log($"[BattleManager] Spawned {enemyClass.enemyName} at {position}");
        }
        else
        {
            Debug.LogError($"[BattleManager] Enemy prefab for {enemyClass.enemyName} is missing EnemyUnit component!");
        }
    }

    private void FindEnemies()
    {
        // Add any enemies that might not be in our list already
        var allEnemies = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);
        foreach (var enemy in allEnemies)
        {
            if (!enemyUnits.Contains(enemy))
            {
                enemyUnits.Add(enemy);
            }
        }

        Debug.Log($"[BattleManager] Found total of {enemyUnits.Count} enemies and {playerUnits.Count} players");
    }

    private IEnumerator MonitorBattleOutcome()
    {
        yield return new WaitForSeconds(0.5f); // Short delay to ensure everything is initialized
        
        while (true)
        {
            enemyUnits.RemoveAll(enemy => enemy == null);
            playerUnits.RemoveAll(player => player == null);

            if (enemyUnits.Count == 0)
            {
                Debug.Log("[BattleManager] 🏆 All enemies defeated!");
                StartCoroutine(EndBattle(true));
                yield break;
            }

            if (playerUnits.Count == 0)
            {
                Debug.Log("[BattleManager] 💀 All players defeated!");
                StartCoroutine(EndBattle(false));
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator EndBattle(bool isVictory)
    {
        yield return new WaitForSeconds(1f);

        if (isVictory)
        {
            Debug.Log("[BattleManager] 🏆 Victory!");
            UIManager.Instance?.ShowEndBattleScreen(true);
            yield return new WaitForSeconds(2f);
            UIManager.Instance?.ReturnToMap();
        }
        else
        {
            Debug.Log("[BattleManager] 💀 Defeat!");
            UIManager.Instance?.ShowEndBattleScreen(false);
        }
    }

    public void CheckBattleState()
    {
        enemyUnits.RemoveAll(enemy => enemy == null);
        playerUnits.RemoveAll(player => player == null);
        
        Debug.Log($"[BattleManager] Battle Status - Enemies: {enemyUnits.Count}, Players: {playerUnits.Count}");
    }
}
