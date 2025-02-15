using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StatusEffectUI : MonoBehaviour
{
    public GameObject statusEffectPrefab;  
    public Transform statusEffectContainer;  

    private List<GameObject> activeStatusIcons = new List<GameObject>();

    public void UpdateStatusEffects(List<BaseStatusEffect> effects)
    {
        Debug.Log($"[StatusEffectUI] Updating UI. Total effects: {effects.Count}");

        // ✅ Remove old icons
        foreach (GameObject icon in activeStatusIcons)
        {
            Destroy(icon);
        }
        activeStatusIcons.Clear();

        // ✅ Add new icons for each effect
        foreach (BaseStatusEffect effect in effects)
        {
            if (effect.MaxDuration > 0)  // ✅ Use `MaxDuration` instead of `Duration`
            {
                Debug.Log($"[StatusEffectUI] Adding effect: {effect.StatusType} - Duration: {effect.MaxDuration}");

                GameObject newEffectIcon = Instantiate(statusEffectPrefab, statusEffectContainer);
                Image effectImage = newEffectIcon.GetComponent<Image>();
                Text effectText = newEffectIcon.GetComponentInChildren<Text>();

                // ✅ Set the status effect icon if available
                if (effect.Icon != null)
                {
                    effectImage.sprite = effect.Icon;
                }
                else
                {
                    Debug.LogWarning($"[StatusEffectUI] ❌ No icon assigned for {effect.StatusType}");
                }

                // ✅ Display duration as text
                effectText.text = effect.MaxDuration.ToString();

                // ✅ Store active effect icons
                activeStatusIcons.Add(newEffectIcon);
            }
        }
    }
}

