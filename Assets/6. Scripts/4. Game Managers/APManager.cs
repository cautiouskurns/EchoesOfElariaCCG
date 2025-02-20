using UnityEngine;
using System;

public class APManager : MonoBehaviour
{
    public static APManager Instance { get; private set; }

    [System.Serializable]
    public class ClassAP
    {
        public int currentAP;
        public int maxAP;
        public bool isActive;
    }

    [SerializeField] private int maxAPPerClass = 3;
    private ClassAP[] classAPPools;
    private int activeClassIndex = 0;

    public event Action<int> OnAPChanged; // 🔹 Event for UI updates

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Initialize AP pools for each class
        classAPPools = new ClassAP[3];  // For 3 classes
        for (int i = 0; i < classAPPools.Length; i++)
        {
            classAPPools[i] = new ClassAP 
            { 
                currentAP = maxAPPerClass,
                maxAP = maxAPPerClass,
                isActive = i == 0  // First class starts active
            };
        }
    }

    private void Start()
    {
        ResetTurnAP();
    }

    public void SwitchActiveClass(int classIndex)
    {
        if (classIndex < 0 || classIndex >= classAPPools.Length)
        {
            Debug.LogError($"[APManager] Invalid class index: {classIndex}");
            return;
        }

        // Save current class AP state
        classAPPools[activeClassIndex].isActive = false;
        
        // Switch to new class
        activeClassIndex = classIndex;
        classAPPools[activeClassIndex].isActive = true;
        
        // Notify UI of AP change for new class
        OnAPChanged?.Invoke(GetCurrentAP());
        
        Debug.Log($"[APManager] Switched to class {classIndex}'s AP pool: {GetCurrentAP()}/{GetMaxAP()}");
    }

    public bool SpendAP(int amount)  // Add this method to match the call in CardExecution
    {
        return UseAP(amount);  // Use existing UseAP implementation
    }

    public bool UseAP(int amount)  // Keep existing method for backward compatibility
    {
        if (amount > classAPPools[activeClassIndex].currentAP)
        {
            Debug.Log($"[APManager] Not enough AP! Required: {amount}, Available: {classAPPools[activeClassIndex].currentAP}");
            return false;
        }

        classAPPools[activeClassIndex].currentAP -= amount;
        OnAPChanged?.Invoke(classAPPools[activeClassIndex].currentAP);  // 🔹 Notify UI
        Debug.Log($"[APManager] Class {activeClassIndex} used {amount} AP. Remaining: {classAPPools[activeClassIndex].currentAP}");
        return true;
    }

    public void ResetAllAP()
    {
        for (int i = 0; i < classAPPools.Length; i++)
        {
            classAPPools[i].currentAP = classAPPools[i].maxAP;
        }
        OnAPChanged?.Invoke(GetCurrentAP());
        Debug.Log("[APManager] Reset all class AP pools");
    }

    public void ResetTurnAP()
    {
        classAPPools[activeClassIndex].currentAP = classAPPools[activeClassIndex].maxAP;
        OnAPChanged?.Invoke(GetCurrentAP());
        Debug.Log($"[APManager] Reset AP for class {activeClassIndex}");
    }

    // Add method to get specific class's AP
    public int GetClassAP(int classIndex)
    {
        if (classIndex >= 0 && classIndex < classAPPools.Length)
        {
            return classAPPools[classIndex].currentAP;
        }
        return 0;
    }

    public int GetCurrentAP() => classAPPools[activeClassIndex].currentAP;
    public int GetMaxAP() => classAPPools[activeClassIndex].maxAP;
}


