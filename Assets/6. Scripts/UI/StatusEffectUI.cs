using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StatusEffectUI : MonoBehaviour
{
    public GameObject statusEffectPrefab;  
    public Transform statusEffectContainer;  

    // private Dictionary<StatusType, GameObject> activeStatusIcons = new Dictionary<StatusType, GameObject>();
    private List<GameObject> activeStatusIcons = new List<GameObject>();


    public void UpdateStatusEffects(List<StatusEffect> effects)
    {
        Debug.Log($"[StatusEffectUI] Updating UI. Total effects: {effects.Count}");

        // ✅ Remove old icons
        foreach (GameObject icon in activeStatusIcons)
        {
            Destroy(icon);
        }
        activeStatusIcons.Clear();

        // ✅ Add new icons for each effect
        foreach (StatusEffect effect in effects)
        {
            if (effect.Duration > 0)
            {
                Debug.Log($"[StatusEffectUI] Adding effect: {effect.Type} - Duration: {effect.Duration}");

                GameObject newEffectIcon = Instantiate(statusEffectPrefab, statusEffectContainer);
                Image effectImage = newEffectIcon.GetComponent<Image>();
                Text effectText = newEffectIcon.GetComponentInChildren<Text>();

                if (effect.EffectData != null)  
                {
                    effectImage.sprite = effect.EffectData.effectIcon;
                }
                else
                {
                    Debug.LogWarning($"[StatusEffectUI] ❌ EffectData is null for {effect.Type}");
                }

                effectText.text = effect.Duration.ToString();
                activeStatusIcons.Add(newEffectIcon);  // ✅ Store multiple effect icons
            }
        }
    }

}

