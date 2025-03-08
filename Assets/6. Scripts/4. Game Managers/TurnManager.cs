using UnityEngine;
using System;
using System.Collections;

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
        // Start with the enemy turn first
        CurrentTurn = TurnState.EnemyTurn;
        
        // Execute enemy's first turn without showing intent
        if (EnemyAIManager.Instance != null)
        {
            StartCoroutine(FirstEnemyTurn());
        }
        else
        {
            // If no enemy manager, go directly to player turn
            CurrentTurn = TurnState.PlayerTurn;
            StartPlayerTurn();
        }
    }
    
    // Special first enemy turn that happens at game start
    private IEnumerator FirstEnemyTurn()
    {
        Debug.Log("[TurnManager] 👿 First enemy turn");
        
        // Execute immediate actions without prior intent
        yield return StartCoroutine(EnemyAIManager.Instance.ExecuteImmediateEnemyTurn());
        
        // Plan next turn's actions (displays intent)
        EnemyAIManager.Instance.PlanEnemyActions();
        
        // Now switch to player turn
        CurrentTurn = TurnState.PlayerTurn;
        StartPlayerTurn();
    }

    public void EndPlayerTurn()
    {
        // Deselect current character if one exists
        BaseCharacter selectedChar = BaseCharacter.GetSelectedCharacter();
        if (selectedChar != null)
        {
            selectedChar.Deselect();
        }

        Debug.Log("[TurnManager] 🔄 Player turn ended");
        CurrentTurn = TurnState.EnemyTurn;
        OnTurnChanged?.Invoke(CurrentTurn);
        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        Debug.Log("[TurnManager] 👿 Enemy turn started");
        if (EnemyAIManager.Instance != null)
        {
            EnemyAIManager.Instance.ExecuteEnemyTurn();
        }
        else
        {
            Debug.LogError("[TurnManager] ❌ No EnemyAIManager found in scene!");
            EndEnemyTurn();
        }
    }

    public void EndEnemyTurn()
    {
        Debug.Log("[TurnManager] Enemy turn ended, planning next actions");
        
        // Plan enemy actions for next turn
        EnemyAIManager.Instance.PlanEnemyActions();
        CurrentTurn = TurnState.PlayerTurn;
        OnTurnChanged?.Invoke(CurrentTurn);

        Debug.Log("[TurnManager] Ending turn...");

        // ✅ Find all characters and process their end-turn status effects
        BaseCharacter[] allCharacters = FindObjectsByType<BaseCharacter>(FindObjectsSortMode.None);
        foreach (BaseCharacter character in allCharacters)
        {
            //character.EndTurn();
            character.ProcessEndOfTurnEffects();
        }

        Debug.Log("[TurnManager] --- Turn Ended ---");

        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        Debug.Log("[TurnManager] 👑 Player turn started");
        APManager.Instance.ResetAllAP();
        
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
