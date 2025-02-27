using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;

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
    [SerializeField] private string dialogueFileName = "Dialogue_WhisperingMonolith";
    [SerializeField] private string dialoguePath = "Assets/0. Dialogues/"; // Add explicit path

    [Header("UI References")]
    [SerializeField] private Image sceneImage;  // Reference to the SceneImage object in Canvas
    [SerializeField] private Sprite dialogueImage;  // The sprite to display
    [SerializeField] private Image dialogueImageDisplay;  // Add this to hold the UI Image component

    [Header("Prefab-Specific Settings")]
    [SerializeField] private string loreSceneId;  // Unique identifier for this scene's lore
    [SerializeField] private DialogueData defaultDialogue;  // Default dialogue for this specific scene
    [SerializeField] private Sprite defaultSceneImage;  // Default scene image for this instance

    [Header("Scroll View Settings")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 20f;

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

        // Use scene-specific dialogue file if provided
        string fileName = $"{dialogueFileName}_{loreSceneId}";
        string fullPath = System.IO.Path.Combine(dialoguePath, fileName + ".json");

        Debug.Log($"[LoreManager] Loading dialogue for scene {loreSceneId} from: {fullPath}");
        
        if (File.Exists(fullPath))
        {
            dialogueLoader.LoadDialogueFromFile(fullPath);
        }
        else
        {
            Debug.Log($"[LoreManager] Using default dialogue for scene {loreSceneId}");
            // Use default dialogue data if JSON not found
            if (defaultDialogue != null)
            {
                currentNode = new DialogueNode
                {
                    dialogueId = "start",
                    text = defaultDialogue.dialogueText,
                    nodeSprite = defaultSceneImage
                    // Set other properties as needed
                };
            }
        }

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

        // Update the scene image
        if (sceneImage != null)
        {
            sceneImage.sprite = dialogueImage;
        }

        // Update the image display with the sprite
        if (dialogueImageDisplay != null)
        {
            if (dialogueImage != null)
            {
                dialogueImageDisplay.sprite = dialogueImage;
                dialogueImageDisplay.gameObject.SetActive(true);
            }
            else
            {
                dialogueImageDisplay.gameObject.SetActive(false);
            }
        }

        // Reset scroll position to top when new dialogue starts
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

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
        // if (outcome.triggerBattle) GameManager.Instance.StartBattle("BattleScene", "OverworldMap");
    }

    public void Continue()
    {
        GameManager.Instance.ReturnFromLore();
    }

    // Optional: Add method for automatic scrolling
    public void ScrollText()
    {
        if (scrollRect != null)
        {
            float newPosition = scrollRect.verticalNormalizedPosition - (scrollSpeed * Time.deltaTime);
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newPosition);
        }
    }
}

