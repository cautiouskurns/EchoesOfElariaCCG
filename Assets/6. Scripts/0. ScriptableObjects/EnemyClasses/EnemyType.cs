using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy Type")]
public class EnemyType : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName;
    public GameObject enemyPrefab;

    [Header("Stats")]
    public int maxHealth = 100;
    public int baseDamage = 10;
}
