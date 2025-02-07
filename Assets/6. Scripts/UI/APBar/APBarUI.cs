using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class APBarUI : MonoBehaviour
{
    [SerializeField] private Image apFillImage;  // 🔹 Reference to the AP bar fill (Image)
    [SerializeField] private TextMeshProUGUI apText; // UI text for displaying AP amount
    private int maxAP;

    private void Start()
    {
        if (APManager.Instance != null)
        {
            maxAP = APManager.Instance.GetCurrentAP();  // Get initial max AP
            APManager.Instance.OnAPChanged += UpdateAPDisplay;
            UpdateAPDisplay(maxAP); // Initialize
        }
        else
        {
            Debug.LogError("[APBarUI] ❌ APManager not found in scene!");
        }
    }

    private void UpdateAPDisplay(int currentAP)
    {
        if (apFillImage != null)
        {
            apFillImage.fillAmount = (float)currentAP / maxAP;  // 🔹 Adjust fill amount
        }
        
        if (apText != null)
        {
            apText.text = $"AP: {currentAP} / {maxAP}"; // 🔹 Update AP text
        }
    }
}
