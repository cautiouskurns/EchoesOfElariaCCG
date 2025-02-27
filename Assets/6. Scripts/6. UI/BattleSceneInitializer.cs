using UnityEngine;
using System.Collections.Generic;

public class BattleInitializer : MonoBehaviour
{
    [SerializeField] private Transform[] playerSpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;
    
    [SerializeField] private EnemyClass[] standardEnemiesPool;
    [SerializeField] private EnemyClass[] eliteEnemiesPool;
    [SerializeField] private EnemyClass bossEnemy;
    
    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[BattleInitializer] GameManager not found!");
            return;
        }
        
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
                    SpawnRandomEnemies(standardEnemiesPool, 1, 3); // 1-3 standard enemies
                    break;
                    
                case BattleType.Elite:
                    SpawnRandomEnemies(eliteEnemiesPool, 1, 2); // 1-2 elite enemies
                    break;
                    
                case BattleType.Boss:
                    SpawnBoss();
                    break;
            }
        }
    }
    
    private void SpawnSpecificEnemies(EnemyClass[] enemiesToSpawn)
    {
        for (int i = 0; i < Mathf.Min(enemiesToSpawn.Length, enemySpawnPoints.Length); i++)
        {
            SpawnEnemyAtPosition(enemiesToSpawn[i], enemySpawnPoints[i].position);
        }
    }
    
    private void SpawnRandomEnemies(EnemyClass[] pool, int min, int max)
    {
        if (pool == null || pool.Length == 0)
        {
            Debug.LogError("[BattleInitializer] Enemy pool is empty!");
            return;
        }
        
        int enemyCount = Random.Range(min, max + 1);
        enemyCount = Mathf.Min(enemyCount, enemySpawnPoints.Length);
        
        for (int i = 0; i < enemyCount; i++)
        {
            EnemyClass randomEnemy = pool[Random.Range(0, pool.Length)];
            SpawnEnemyAtPosition(randomEnemy, enemySpawnPoints[i].position);
        }
    }
    
    private void SpawnBoss()
    {
        if (bossEnemy == null)
        {
            Debug.LogError("[BattleInitializer] No boss enemy assigned!");
            return;
        }
        
        // Spawn in center position
        SpawnEnemyAtPosition(bossEnemy, enemySpawnPoints[0].position);
    }
    
    private void SpawnEnemyAtPosition(EnemyClass enemyClass, Vector3 position)
    {
        GameObject enemyObj = Instantiate(enemyClass.enemyPrefab, position, Quaternion.identity);
        
        if (enemyObj.TryGetComponent<EnemyUnit>(out var enemy))
        {
            enemy.InitializeFromClass(enemyClass);
            Debug.Log($"[BattleInitializer] Spawned {enemyClass.enemyName} at {position}");
        }
        else
        {
            Debug.LogError($"[BattleInitializer] Enemy prefab for {enemyClass.enemyName} is missing EnemyUnit component!");
        }
    }
}