using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    
    public enum TurnState { PlayerTurn, EnemyTurn }
    public TurnState CurrentTurn { get; private set; } = TurnState.PlayerTurn;
    
    public event Action<TurnState> OnTurnChanged;

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
        Debug.Log("[TurnManager] 🔄 Player turn ended");
        CurrentTurn = TurnState.EnemyTurn;
        OnTurnChanged?.Invoke(CurrentTurn);
        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        Debug.Log("[TurnManager] 👿 Enemy turn started");
        // TODO: Implement enemy AI here
        EndEnemyTurn(); // For now, immediately end enemy turn
    }

    private void EndEnemyTurn()
    {
        Debug.Log("[TurnManager] Enemy turn ended");
        CurrentTurn = TurnState.PlayerTurn;
        OnTurnChanged?.Invoke(CurrentTurn);
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        Debug.Log("[TurnManager] 👑 Player turn started");
        APManager.Instance.ResetAP();
    }
}
