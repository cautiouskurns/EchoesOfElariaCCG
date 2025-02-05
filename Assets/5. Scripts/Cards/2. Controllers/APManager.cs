using UnityEngine;
using System;

public class APManager : MonoBehaviour
{
    public static APManager Instance { get; private set; }

    [SerializeField] private int maxActionPoints = 10;
    private int currentActionPoints;

    public event Action<int, int> OnAPChanged; // Event to update UI (current, max)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        currentActionPoints = maxActionPoints;
        OnAPChanged?.Invoke(currentActionPoints, maxActionPoints);
    }

    public void UseActionPoints(int amount)
    {
        if (currentActionPoints >= amount)
        {
            currentActionPoints -= amount;
            OnAPChanged?.Invoke(currentActionPoints, maxActionPoints);
            Debug.Log($"[APManager] Used {amount} AP. Remaining: {currentActionPoints}/{maxActionPoints}");
        }
        else
        {
            Debug.LogWarning($"[APManager] Not enough AP! Required: {amount}, Available: {currentActionPoints}");
        }
    }

    public bool HasEnoughAP(int cost) => currentActionPoints >= cost;

    public void RefillAP()
    {
        currentActionPoints = maxActionPoints;
        OnAPChanged?.Invoke(currentActionPoints, maxActionPoints);
        Debug.Log($"[APManager] Refilled AP to {maxActionPoints}");
    }
}
