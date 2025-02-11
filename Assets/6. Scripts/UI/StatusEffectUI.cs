using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StatusEffectUI : MonoBehaviour
{
    public GameObject statusEffectPrefab;  
    public Transform statusEffectContainer;  

    private Dictionary<StatusType, GameObject> activeStatusIcons = new Dictionary<StatusType, GameObject>();

    public void UpdateStatusEffects(List<StatusEffect> effects)
    {
        // Clear previous status effects
        foreach (Transform child in statusEffectContainer)
        {
            Destroy(child.gameObject);
        }
        activeStatusIcons.Clear();

        // Add new status effects
        foreach (StatusEffect effect in effects)
        {
            if (effect.Duration > 0)
            {
                GameObject newEffectIcon = Instantiate(statusEffectPrefab, statusEffectContainer);
                Image effectImage = newEffectIcon.GetComponent<Image>();
                Text effectText = newEffectIcon.GetComponentInChildren<Text>();

                // ✅ Fix: Use EffectData instead of effectData
                if (effect.EffectData != null)  
                {
                    effectImage.sprite = effect.EffectData.effectIcon;  // ✅ Correct reference
                }
                else
                {
                    Debug.LogWarning($"StatusEffect {effect.Type} has no EffectData assigned!");
                }

                effectText.text = effect.Duration.ToString();

                // ✅ Fix: Use EffectData.statusType instead of effectData.statusType
                activeStatusIcons[effect.EffectData.statusType] = newEffectIcon;
            }
        }
    }
}

