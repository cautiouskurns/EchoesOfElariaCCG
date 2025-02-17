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
        // Find all units in the scene
        enemyUnits.AddRange(FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None));
        playerUnits.AddRange(FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None));

        Debug.Log($"[BattleManager] Found {enemyUnits.Count} enemies and {playerUnits.Count} players");
        StartCoroutine(MonitorBattleOutcome());
    }

    private IEnumerator MonitorBattleOutcome()
    {
        while (true)
        {
            // Remove any destroyed units from the lists
            enemyUnits.RemoveAll(enemy => enemy == null);
            playerUnits.RemoveAll(player => player == null);

            // Check win condition (all enemies defeated)
            if (enemyUnits.Count == 0)
            {
                Debug.Log("[BattleManager] All enemies defeated!");
                StartCoroutine(EndBattle(true));
                yield break;
            }

            // Check lose condition (all players defeated)
            if (playerUnits.Count == 0)
            {
                Debug.Log("[BattleManager] All players defeated!");
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
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowEndBattleScreen(true);
                yield return new WaitForSeconds(2f);
                UIManager.Instance.ReturnToMap();
            }
            else
            {
                Debug.LogWarning("[BattleManager] UIManager not found, loading Overworld directly");
                SceneManager.LoadScene("OverworldMap");
            }
        }
        else
        {
            Debug.Log("[BattleManager] 💀 Defeat!");
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowEndBattleScreen(false);
            }
        }
    }

    // Method to manually check battle state (can be called from other scripts)
    public void CheckBattleState()
    {
        enemyUnits.RemoveAll(enemy => enemy == null);
        playerUnits.RemoveAll(player => player == null);
        
        Debug.Log($"[BattleManager] Battle Status - Enemies: {enemyUnits.Count}, Players: {playerUnits.Count}");
    }
}



