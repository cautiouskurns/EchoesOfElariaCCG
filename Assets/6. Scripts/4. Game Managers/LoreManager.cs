using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LoreManager : MonoBehaviour
{
    public static LoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI outcomeText;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private Transform choiceContainer;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Button continueButton; // ✅ Button for continuing after outcome

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[LoreManager] ❌ GameManager not found!");
            return;
        }

        DialogueData dialogueData = GameManager.Instance.GetStoredLoreDialogue();
        
        if (dialogueData == null)
        {
            Debug.LogError("[LoreManager] ❌ No dialogue data found in GameManager!");
            return;
        }

        Debug.Log($"[LoreManager] ✅ Loading dialogue: {dialogueData.name}");
        StartLoreEvent(dialogueData);
    }

    public void StartLoreEvent(DialogueData dialogueData)
    {
        if (dialogueData == null)
        {
            Debug.LogError("[LoreManager] ❌ Null DialogueData provided!");
            return;
        }

        if (dialogueText == null)
        {
            Debug.LogError("[LoreManager] ❌ DialogueText component not assigned!");
            return;
        }

        dialoguePanel.SetActive(true); // ✅ Ensure panel is active
        dialogueText.text = dialogueData.dialogueText;
        outcomeText.text = ""; // ✅ Clear previous outcome text

        Debug.Log($"[LoreManager] 🗨️ Showing dialogue: {dialogueData.dialogueText}");

        // ✅ Clear old choices before adding new ones
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

        // ✅ Create new choices
        foreach (var choice in dialogueData.choices)
        {
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            choiceButton.GetComponent<Button>().onClick.AddListener(() => SelectChoice(choice));
        }

        // Hide continue button until a choice is made
        if (continueButton != null) continueButton.gameObject.SetActive(false);
    }

    private void SelectChoice(DialogueChoice choice)
    {
        // Hide choices after selection
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

        // ✅ Display the outcome text
        outcomeText.text = choice.outcome.outcomeText;
        Debug.Log($"[LoreManager] 🏆 Outcome: {choice.outcome.outcomeText}");

        // ✅ Apply effects
        ApplyOutcome(choice.outcome);

        // ✅ Show continue button after choice is made
        if (continueButton != null) continueButton.gameObject.SetActive(true);
    }

    private void ApplyOutcome(DialogueOutcome outcome)
    {
        if (GameManager.Instance != null)
        {
            if (outcome.energyChange != 0)
            {
                Debug.Log($"[LoreManager] ⚡ Energy changed by {outcome.energyChange}");
            }
            if (outcome.luckChange != 0)
            {
                Debug.Log($"[LoreManager] 🍀 Luck changed by {outcome.luckChange}");
            }
            if (outcome.strengthChange != 0)
            {
                Debug.Log($"[LoreManager] 💪 Strength changed by {outcome.strengthChange}");
            }
            if (outcome.giveItem)
            {
                Debug.Log("[LoreManager] 🎁 Player gained an item!");
            }
        }
    }

    // ✅ Continue back to the overworld or previous scene
    public void Continue()
    {
        GameManager.Instance.ReturnFromLore();
    }
}
