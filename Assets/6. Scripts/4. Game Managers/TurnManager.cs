using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    
    public enum TurnState { PlayerTurn, EnemyTurn }
    public TurnState CurrentTurn { get; private set; } = TurnState.PlayerTurn;
    
    public event Action<TurnState> OnTurnChanged;

    [SerializeField] private HandManager handManager;  // Assign in inspector

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartPlayerTurn();
    }

    public void EndPlayerTurn()
    {
    // Deselect current character if one exists
    CharacterSelection selectedChar = FindAnyObjectByType<CharacterSelection>();
    if (selectedChar != null)
    {
        selectedChar.Deselect();  // ✅ Now calling Deselect() from CharacterSelection component
    }

        Debug.Log("[TurnManager] 🔄 Player turn ended");
        CurrentTurn = TurnState.EnemyTurn;
        OnTurnChanged?.Invoke(CurrentTurn);
        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        Debug.Log("[TurnManager] 👿 Enemy turn started");

        if (EnemyAIManager.Instance == null)
        {
            Debug.LogError("[TurnManager] ❌ EnemyAIManager instance is NULL! Enemies will NOT act.");
            EndEnemyTurn(); 
            return;
        }

        Debug.Log("[TurnManager] ✅ Calling ExecuteEnemyTurn()");
        EnemyAIManager.Instance.ExecuteEnemyTurn();
    }

    public void EndEnemyTurn()
    {
        Debug.Log("[TurnManager] Enemy turn ended");
        CurrentTurn = TurnState.PlayerTurn;
        OnTurnChanged?.Invoke(CurrentTurn);

        Debug.Log("[TurnManager] Ending turn...");

        // // ✅ Find all characters and process their end-turn status effects
        BaseCharacter[] allCharacters = FindObjectsByType<BaseCharacter>(FindObjectsSortMode.None);
        foreach (BaseCharacter character in allCharacters)
        {
            character.ProcessEndOfTurnEffects();
        }

        Debug.Log("[TurnManager] --- Turn Ended ---");

        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        Debug.Log("[TurnManager] 👑 Player turn started");
        APManager.Instance.ResetAllAP();  // Reset AP for all classes at turn start
        
        if (handManager != null)
        {
            // Draw new cards at the start of turn
            handManager.RefreshHand();
            Debug.Log("[TurnManager] 🎴 Hand refreshed for new turn");
        }
        else
        {
            Debug.LogError("[TurnManager] ❌ HandManager reference missing!");
        }
    }
}
