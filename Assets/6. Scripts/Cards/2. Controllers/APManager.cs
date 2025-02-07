using UnityEngine;
using System;

public class APManager : MonoBehaviour
{
    public static APManager Instance { get; private set; }

    [SerializeField] private int maxAP = 6;  // Set max AP in the Inspector
    private int currentAP;

    public event Action<int> OnAPChanged; // 🔹 Event for UI updates

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ResetAP();
    }

    public void ResetAP()
    {
        currentAP = maxAP;
        OnAPChanged?.Invoke(currentAP);  // 🔹 Notify UI
    }

    public bool SpendAP(int amount)
    {
        if (amount > currentAP)
        {
            Debug.LogWarning("[APManager] ❌ Not enough AP!");
            return false; 
        }

        currentAP -= amount;
        OnAPChanged?.Invoke(currentAP);  // 🔹 Notify UI
        Debug.Log($"[APManager] 🔥 {amount} AP spent. Remaining: {currentAP}");
        return true;
    }

    public int GetCurrentAP() => currentAP;
}


