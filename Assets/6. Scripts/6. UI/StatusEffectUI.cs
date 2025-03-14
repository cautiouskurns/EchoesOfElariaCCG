using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StatusEffectUI : MonoBehaviour
{
    public GameObject statusEffectPrefab;  
    public Transform statusEffectContainer;  

    private Dictionary<StatusEffectTypes, GameObject> activeStatusIcons = new Dictionary<StatusEffectTypes, GameObject>();

    public void UpdateStatusEffects(List<ActiveStatusEffect> activeEffects)
    {
        // Debug.Log($"[StatusEffectUI] Updating UI. Total effects: {activeEffects.Count}");

        // ✅ Loop through active effects and update UI
        foreach (var effect in activeEffects)
        {
            if (effect.Duration <= 0)  // ✅ Ignore expired effects
                continue;

            if (activeStatusIcons.TryGetValue(effect.EffectData.StatusType, out GameObject existingIcon))
            {
                // Updated to use TextMeshProUGUI instead of Text
                TextMeshProUGUI effectText = existingIcon.transform.Find("StatusDurationText")?.GetComponent<TextMeshProUGUI>();
                if (effectText != null)
                {
                    effectText.text = effect.Duration.ToString();
                }
                else
                {
                    Debug.LogError("[StatusEffectUI] Could not find TextMeshProUGUI component");
                }
            }
            else
            {
                // ✅ If it's a new effect, create a UI icon for it
                GameObject newEffectIcon = Instantiate(statusEffectPrefab, statusEffectContainer);
                Image effectImage = newEffectIcon.GetComponent<Image>();
                // Updated to use TextMeshProUGUI
                TextMeshProUGUI effectText = newEffectIcon.GetComponentInChildren<TextMeshProUGUI>();

                // ✅ Set the icon if available
                if (effect.EffectData.Icon != null)
                {
                    effectImage.sprite = effect.EffectData.Icon;
                }
                else
                {
                    Debug.LogWarning($"[StatusEffectUI] ❌ No icon assigned for {effect.EffectData.StatusType}");
                }

                // ✅ Set duration text
                effectText.text = effect.Duration.ToString();

                // ✅ Store the effect in the dictionary
                activeStatusIcons[effect.EffectData.StatusType] = newEffectIcon;
            }
        }

        // ✅ Remove expired effects from the UI
        List<StatusEffectTypes> expiredEffects = new List<StatusEffectTypes>();
        foreach (var effect in activeStatusIcons)
        {
            bool isEffectStillActive = activeEffects.Exists(e => e.EffectData.StatusType == effect.Key && e.Duration > 0);
            if (!isEffectStillActive)
            {
                Destroy(effect.Value);
                expiredEffects.Add(effect.Key);
            }
        }

        // ✅ Remove expired effects from dictionary
        foreach (var effectType in expiredEffects)
        {
            activeStatusIcons.Remove(effectType);
        }
    }
}


