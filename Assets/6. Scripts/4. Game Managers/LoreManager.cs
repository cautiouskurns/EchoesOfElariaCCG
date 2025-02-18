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
    [SerializeField] private Button continueButton;

    [Header("Dialogue File Settings")]
    [SerializeField] private string dialogueFileName = "dialogue";
    [SerializeField] private string dialoguePath = "Assets/0. Dialogues/"; // Add explicit path

    private DialogueLoader dialogueLoader;
    private DialogueNode currentNode;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Ensure DialogueLoader exists
        dialogueLoader = GetComponent<DialogueLoader>();
        if (dialogueLoader == null)
        {
            Debug.Log("[LoreManager] Adding missing DialogueLoader component");
            dialogueLoader = gameObject.AddComponent<DialogueLoader>();
        }
    }

    private void Start()
    {
        if (dialogueLoader == null)
        {
            Debug.LogError("[LoreManager] DialogueLoader is null!");
            return;
        }

        string fullPath = System.IO.Path.Combine(dialoguePath, dialogueFileName + ".json");
        Debug.Log($"[LoreManager] Loading dialogue from: {fullPath}");
        dialogueLoader.LoadDialogueFromFile(fullPath);
        StartDialogue("start");
    }

    public void StartDialogue(string nodeId)
    {
        currentNode = dialogueLoader.GetNode(nodeId);
        if (currentNode == null)
        {
            Debug.LogError($"[LoreManager] ❌ Dialogue Node '{nodeId}' Not Found!");
            return;
        }

        dialoguePanel.SetActive(true);
        dialogueText.text = currentNode.text;
        outcomeText.text = "";

        // Clear old choices
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

        // Check if this is an end node (no choices)
        if (currentNode.choices == null || currentNode.choices.Count == 0)
        {
            Debug.Log("[LoreManager] End node reached - showing continue button");
            if (continueButton != null) continueButton.gameObject.SetActive(true);
            return;
        }

        // Add new choices
        foreach (var choice in currentNode.choices)
        {
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            choiceButton.GetComponent<Button>().onClick.AddListener(() => SelectChoice(choice));
        }

        // Hide continue button while choices are available
        if (continueButton != null) continueButton.gameObject.SetActive(false);
    }

    private void SelectChoice(Choice choice)
    {
        if (!string.IsNullOrEmpty(choice.nextId))
        {
            StartDialogue(choice.nextId);
        }
        else
        {
            // If there's no next node, show continue button
            if (continueButton != null) continueButton.gameObject.SetActive(true);
            
            if (currentNode.outcome != null)
            {
                DisplayOutcome(currentNode.outcome);
            }
        }
    }

    private void DisplayOutcome(Outcome outcome)
    {
        outcomeText.text = outcome.outcomeText;
        ApplyOutcome(outcome);

        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

        if (continueButton != null) continueButton.gameObject.SetActive(true);
    }

    private void ApplyOutcome(Outcome outcome)
    {
        if (outcome.energyChange != 0) Debug.Log($"[LoreManager] ⚡ Energy changed by {outcome.energyChange}");
        if (outcome.strengthChange != 0) Debug.Log($"[LoreManager] 💪 Strength changed by {outcome.strengthChange}");
        if (outcome.triggerBattle) GameManager.Instance.StartBattle("BattleScene", "OverworldMap");
    }

    public void Continue()
    {
        GameManager.Instance.ReturnFromLore();
    }
}

