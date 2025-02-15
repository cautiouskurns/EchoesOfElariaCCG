using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image healthFill;  // Reference to UI Image component
    [SerializeField] private TextMeshProUGUI healthText;  // Add reference to TMP text
    private BaseCharacter character;
    private CharacterStats characterStats;

    private void Awake()
    {
        // Configure Image component
        if (healthFill != null)
        {
            healthFill.type = Image.Type.Filled;
            healthFill.fillMethod = Image.FillMethod.Horizontal;
            healthFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        }

        // Step 1: Find the BaseCharacter (PlayerUnit/EnemyUnit)
        character = GetComponentInParent<BaseCharacter>();
        if (character == null)
        {
            Debug.LogError("[HealthBarUI] ❌ BaseCharacter not found in parent of " + gameObject.name);
            return;
        }

        // Step 2: Try to find CharacterStats directly or in the parent
        characterStats = character.GetComponent<CharacterStats>();
        if (characterStats == null)
        {
            characterStats = GetComponentInParent<CharacterStats>(); // Look further up if needed
        }

        if (characterStats == null)
        {
            Debug.LogError("[HealthBarUI] ❌ CharacterStats not found for " + character.Name);
            return;
        }

        // Subscribe to health changes
        characterStats.OnHealthChanged += UpdateHealthBar;

        // Verify Image setup
        if (healthFill != null)
        {
            if (healthFill.type != Image.Type.Filled)
            {
                Debug.LogError("[HealthBarUI] ❌ Image type must be set to FILLED!");
                return;
            }
        }
    }

    private void Start()
    {
        if (characterStats != null)
        {
            if (healthFill != null)
            {
                UpdateHealthBar(characterStats.CurrentHealth);
            }
            else
            {
                Debug.LogError("[HealthBarUI] ❌ Health fill Image reference is missing!");
            }
        }
    }

    private void UpdateHealthBar(int currentHealth)
    {
        if (healthFill == null)
        {
            Debug.LogError("[HealthBarUI] Health fill Image reference not assigned.");
            return;
        }

        // Update fill amount
        float fillAmount = (float)currentHealth / characterStats.MaxHealth;
        healthFill.fillAmount = fillAmount;

        // Update text display
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{characterStats.MaxHealth}";
        }

    }

    private void OnDestroy()
    {
        if (characterStats != null)
        {
            characterStats.OnHealthChanged -= UpdateHealthBar;
        }
    }
}


