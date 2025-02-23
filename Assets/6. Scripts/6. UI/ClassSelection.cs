using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ClassSelection : MonoBehaviour
{
    [SerializeField] private CharacterClass[] availableClasses;
    [SerializeField] private string nextSceneName = "OverworldMap"; // Scene to load after selection

    [Header("UI Elements")]
    [SerializeField] private GameObject classInfoPanel;
    [SerializeField] private Image classIcon;
    [SerializeField] private TextMeshProUGUI classNameText;
    [SerializeField] private TextMeshProUGUI classDescriptionText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI dexterityText;
    [SerializeField] private TextMeshProUGUI intelligenceText;
    [SerializeField] private Button confirmButton;

    [Header("Selected Class UI")] // ✅ UI to display selected classes
    [SerializeField] private Image[] selectedClassIcons;
    [SerializeField] private TextMeshProUGUI[] selectedClassNames;

    private CharacterClass[] selectedClasses = new CharacterClass[2]; // ✅ Store 3 selected classes
    private int currentSelectionIndex = 0; // ✅ Track which slot is being selected

    private void Start()
    {
        StartCoroutine(CheckForGameManager());

        if (confirmButton != null) confirmButton.gameObject.SetActive(false);
        if (classInfoPanel != null) classInfoPanel.SetActive(false);

        // ✅ Hide selected class UI initially
        for (int i = 0; i < selectedClassIcons.Length; i++)
        {
            selectedClassIcons[i].enabled = false;
            selectedClassNames[i].text = "Empty";
        }
    }

    private IEnumerator CheckForGameManager()
    {
        yield return new WaitForSeconds(0.1f);

        if (GameManager.Instance == null)
        {
            Debug.LogError("[ClassSelection] ❌ No GameManager found! Returning to Overworld...");
            SceneManager.LoadScene("OverworldMap");
        }
    }

    public void SelectClass(int index)
    {
        if (index < 0 || index >= availableClasses.Length)
        {
            Debug.LogError("[ClassSelection] ❌ Invalid class index selected!");
            return;
        }

        if (currentSelectionIndex >= 2)
        {
            Debug.LogWarning("[ClassSelection] ⚠️ All 3 classes have already been selected.");
            return;
        }

        selectedClasses[currentSelectionIndex] = availableClasses[index];
        UpdateSelectedClassUI(currentSelectionIndex, selectedClasses[currentSelectionIndex]);
        DisplayClassInfo(selectedClasses[currentSelectionIndex], currentSelectionIndex);
        currentSelectionIndex++;

        if (currentSelectionIndex == 2)
        {
            confirmButton.gameObject.SetActive(true); // ✅ Enable confirm button once all selections are made
        }
    }

    private void DisplayClassInfo(CharacterClass characterClass, int slot)
    {
        if (classInfoPanel != null) classInfoPanel.SetActive(true);

        if (classNameText != null) classNameText.text = $"Class {slot + 1}: {characterClass.className}";
        if (classIcon != null) classIcon.sprite = characterClass.classIcon;
        if (classDescriptionText != null) classDescriptionText.text = characterClass.classDescription;
        if (healthText != null) healthText.text = $"Health: {characterClass.baseHealth}";
        if (strengthText != null) strengthText.text = $"Strength: {characterClass.strength}";
        if (dexterityText != null) dexterityText.text = $"Dexterity: {characterClass.dexterity}";
        if (intelligenceText != null) intelligenceText.text = $"Intelligence: {characterClass.intelligence}";
    }

    private void UpdateSelectedClassUI(int slot, CharacterClass characterClass)
    {
        if (selectedClassIcons[slot] != null)
        {
            selectedClassIcons[slot].enabled = true;
            selectedClassIcons[slot].sprite = characterClass.classIcon;
        }

        if (selectedClassNames[slot] != null)
        {
            selectedClassNames[slot].text = characterClass.className;
        }
    }

    public void ConfirmSelection()
    {
        if (selectedClasses[0] == null || selectedClasses[1] == null) // || selectedClasses[2] == null)
        {
            Debug.LogError("[ClassSelection] ❌ Not all classes selected!");
            return;
        }

        if (GameManager.Instance != null)
        {   
            for (int i = 0; i < 2; i++)
            {
                GameManager.Instance.SetPlayerClass(i, selectedClasses[i]);
            }

            Debug.Log("[ClassSelection] ✅ All 3 classes selected. Transitioning to overworld...");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("[ClassSelection] ❌ GameManager is missing!");
        }
    }

    // Assign these methods to UI buttons in the Inspector
    public void SelectWarrior() => SelectClass(0);
    public void SelectMage() => SelectClass(1);
    public void SelectRogue() => SelectClass(2);
}
