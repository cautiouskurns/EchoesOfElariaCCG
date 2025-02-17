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

    private CharacterClass selectedClass;

    private void Start()
    {
        StartCoroutine(CheckForGameManager());

        // Hide confirm button until class is selected
        if (confirmButton != null) confirmButton.gameObject.SetActive(false);
        if (classInfoPanel != null) classInfoPanel.SetActive(false);
    }

    private IEnumerator CheckForGameManager()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to ensure scene is fully loaded

        if (GameManager.Instance == null)
        {
            Debug.LogError("[ClassSelection] No GameManager found! Please ensure GameManager exists in the Overworld and is marked DontDestroyOnLoad");
            
            // Optionally return to Overworld
            SceneManager.LoadScene("OverworldMap");
        }
        else
        {
            Debug.Log($"[ClassSelection] Found GameManager with ID: {GameManager.Instance.gameObject.GetInstanceID()}");
        }
    }

    public void SelectClass(int index)
    {
        if (index < 0 || index >= availableClasses.Length)
        {
            Debug.LogError("[ClassSelection] Invalid class index selected!");
            return;
        }

        selectedClass = availableClasses[index];
        DisplayClassInfo(selectedClass);
    }

    private void DisplayClassInfo(CharacterClass characterClass)
    {
        if (classInfoPanel != null) classInfoPanel.SetActive(true);
        if (confirmButton != null) confirmButton.gameObject.SetActive(true);

        if (classNameText != null) classNameText.text = characterClass.className;
        if (classIcon != null) classIcon.sprite = characterClass.classIcon;
        if (classDescriptionText != null) classDescriptionText.text = characterClass.classDescription;
        if (healthText != null) healthText.text = $"Health: {characterClass.baseHealth}";
        if (strengthText != null) strengthText.text = $"Strength: {characterClass.strength}";
        if (dexterityText != null) dexterityText.text = $"Dexterity: {characterClass.dexterity}";
        if (intelligenceText != null) intelligenceText.text = $"Intelligence: {characterClass.intelligence}";
    }

    public void ConfirmSelection()
    {
        if (selectedClass == null)
        {
            Debug.LogError("[ClassSelection] No class selected!");
            return;
        }

        if (GameManager.Instance != null)
        {   
            GameManager.Instance.SetPlayerClass(selectedClass);
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }

    // Assign these methods to UI buttons in the Inspector
    public void SelectWarrior() => SelectClass(0);
    public void SelectMage() => SelectClass(1);
    public void SelectRogue() => SelectClass(2);
}
