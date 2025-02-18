using UnityEngine;

public class LoreNode : MonoBehaviour
{
    [SerializeField] private DialogueData loreDialogue; // ✅ Assign this in Inspector
    [SerializeField] private string loreSceneName = "LoreScene"; // ✅ Set the scene name

    public void OnInteract()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[LoreNode] ❌ GameManager not found!");
            return;
        }

        if (loreDialogue == null)
        {
            Debug.LogError("[LoreNode] ❌ No DialogueData assigned to this node!");
            return;
        }

        if (string.IsNullOrEmpty(loreSceneName))
        {
            Debug.LogError("[LoreNode] ❌ No lore scene name specified!");
            return;
        }

        Debug.Log($"[LoreNode] 📖 Starting lore event with dialogue: {loreDialogue.name}");
        GameManager.Instance.StartLoreEvent(loreDialogue, loreSceneName);
    }
}


