using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    [SerializeField] private Transform[] playerSpawnPoints;  // Add spawn points for players
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
            Debug.LogError("[BattleManager] ❌ GameManager not found! Cannot assign class data.");
            return;
        }

        // AssignClassStatsToExistingPlayers();
        SpawnPlayers();
        FindEnemies();
        StartCoroutine(MonitorBattleOutcome());
    }

    // /// ✅ Assigns Class Stats to Existing Player Units in Scene
    // private void AssignClassStatsToExistingPlayers()
    // {
    //     playerUnits.AddRange(FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None));

    //     if (playerUnits.Count != GameManager.Instance.selectedClasses.Length)
    //     {
    //         Debug.LogWarning($"[BattleManager] ⚠ Expected {GameManager.Instance.selectedClasses.Length} players, but found {playerUnits.Count} in the scene.");
    //     }

    //     for (int i = 0; i < playerUnits.Count; i++)
    //     {
    //         if (i < GameManager.Instance.selectedClasses.Length)
    //         {
    //             CharacterClass selectedClass = GameManager.Instance.selectedClasses[i];

    //             if (selectedClass != null)
    //             {
    //                 playerUnits[i].InitializeFromClass(selectedClass);
    //                 Debug.Log($"[BattleManager] ✅ Assigned {selectedClass.className} to Player {i + 1}");
    //             }
    //             else
    //             {
    //                 Debug.LogError($"[BattleManager] ❌ No class selected for Player {i + 1}");
    //             }
    //         }
    //     }
    // }

        /// ✅ Spawns Player Units Based on Their Selected Class
    private void SpawnPlayers()
    {
        // Clear any existing player units from previous battles
        foreach (var unit in playerUnits)
        {
            if (unit != null) Destroy(unit.gameObject);
        }
        playerUnits.Clear();

        CharacterClass[] selectedClasses = GameManager.Instance.selectedClasses;

        for (int i = 0; i < selectedClasses.Length; i++)
        {
            if (selectedClasses[i] == null)
            {
                Debug.LogError($"[BattleManager] ❌ No class assigned for Player {i + 1}");
                continue;
            }

            // Use the class-specific prefab variant
            GameObject playerPrefab = selectedClasses[i].classPrefab;  // Make sure SO has a classPrefab field

            if (playerPrefab == null)
            {
                Debug.LogError($"[BattleManager] ❌ No prefab found for {selectedClasses[i].className}");
                continue;
            }

            Transform spawnPoint = (i < playerSpawnPoints.Length) ? playerSpawnPoints[i] : null;

            if (spawnPoint == null)
            {
                Debug.LogWarning($"[BattleManager] ⚠ No spawn point assigned for Player {i + 1}, using default position.");
                spawnPoint = new GameObject($"PlayerSpawn_{i}").transform;
                spawnPoint.position = new Vector3(i * 2, 0, 0);
            }

            GameObject playerObj = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
            PlayerUnit playerUnit = playerObj.GetComponent<PlayerUnit>();

            if (playerUnit != null)
            {
                playerUnit.InitializeFromClass(selectedClasses[i]);
                playerUnits.Add(playerUnit);
                Debug.Log($"[BattleManager] ✅ Spawned {selectedClasses[i].className} at position {spawnPoint.position}");
            }
            else
            {
                Debug.LogError($"[BattleManager] ❌ {playerPrefab.name} is missing a PlayerUnit component!");
            }
        }
    }

    private void FindEnemies()
    {
        enemyUnits.Clear();
        enemyUnits.AddRange(FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None));

        Debug.Log($"[BattleManager] Found {enemyUnits.Count} enemies and {playerUnits.Count} players");
    }

    private IEnumerator MonitorBattleOutcome()
    {
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


