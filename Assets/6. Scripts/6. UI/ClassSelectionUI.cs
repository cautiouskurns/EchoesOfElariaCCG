using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ClassSelectionUI : MonoBehaviour
{
    [SerializeField] private List<CharacterClass> availableClasses;
    [SerializeField] private Image classIcon;
    [SerializeField] private Text classNameText;
    [SerializeField] private Text statsText;
    [SerializeField] private Button confirmButton;

    private CharacterClass selectedClass;

    public void SelectClass(int index)
    {
        selectedClass = availableClasses[index];
        classIcon.sprite = selectedClass.classIcon;
        classNameText.text = selectedClass.className;
        statsText.text = $"HP: {selectedClass.baseHealth}\nEnergy: {selectedClass.baseEnergy}\nStrength: {selectedClass.strength}";

        confirmButton.interactable = true;
    }

    public void ConfirmSelection()
    {
        GameManager.Instance.SetPlayerClass(0, selectedClass);
        SceneManager.LoadScene("OverworldMap"); // Return to main map
    }
}
