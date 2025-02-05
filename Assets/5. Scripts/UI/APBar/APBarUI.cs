using UnityEngine;
using UnityEngine.UI;

public class APBarUI : MonoBehaviour
{
    [SerializeField] private Image apFill; // Drag & drop the APFill image here in Inspector
    private CharacterStats characterStats;

    private void Awake()
    {
        characterStats = GetComponentInParent<CharacterStats>();

        if (characterStats != null)
        {
            characterStats.OnActionPointsChanged += UpdateAPBar;
        }
        else
        {
            Debug.LogError("[APBarUI] ❌ CharacterStats not found in parent.");
        }
    }

    private void Start()
    {
        if (characterStats != null)
        {
            UpdateAPBar(characterStats.CurrentActionPoints); // Initialize AP bar
        }
    }

    private void UpdateAPBar(int currentAP)
    {
        if (apFill == null)
        {
            Debug.LogError("[APBarUI] ❌ APFill reference missing.");
            return;
        }

        float fillAmount = (float)currentAP / characterStats.MaxActionPoints;
        apFill.fillAmount = fillAmount;

        Debug.Log($"[APBarUI] ✅ AP Updated: {currentAP}/{characterStats.MaxActionPoints}");
    }

    private void OnDestroy()
    {
        if (characterStats != null)
        {
            characterStats.OnActionPointsChanged -= UpdateAPBar;
        }
    }
}
