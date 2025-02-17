using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

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

        AssignClassStatsToExistingPlayers();
        FindEnemies();
        StartCoroutine(MonitorBattleOutcome());
    }

    /// ✅ Assigns Class Stats to Existing Player Units in Scene
    private void AssignClassStatsToExistingPlayers()
    {
        playerUnits.AddRange(FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None));

        if (playerUnits.Count != GameManager.Instance.selectedClasses.Length)
        {
            Debug.LogWarning($"[BattleManager] ⚠ Expected {GameManager.Instance.selectedClasses.Length} players, but found {playerUnits.Count} in the scene.");
        }

        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (i < GameManager.Instance.selectedClasses.Length)
            {
                CharacterClass selectedClass = GameManager.Instance.selectedClasses[i];

                if (selectedClass != null)
                {
                    playerUnits[i].InitializeFromClass(selectedClass);
                    Debug.Log($"[BattleManager] ✅ Assigned {selectedClass.className} to Player {i + 1}");
                }
                else
                {
                    Debug.LogError($"[BattleManager] ❌ No class selected for Player {i + 1}");
                }
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


