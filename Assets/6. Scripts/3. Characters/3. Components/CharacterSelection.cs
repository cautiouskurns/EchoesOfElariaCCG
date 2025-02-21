using UnityEngine;
using System.Collections.Generic;

public class CharacterSelection : MonoBehaviour
{    
    private bool isSelected = false;
    public bool IsSelected => isSelected;
    private static CharacterSelection currentlySelectedCharacter;

    [SerializeField] private int classIndex; // ✅ Store class index for Hand/AP switching

    public void Select()
    {
        if (currentlySelectedCharacter != null && currentlySelectedCharacter != this)
        {
            currentlySelectedCharacter.Deselect();
        }

        if (!IsSelected)
        {
            currentlySelectedCharacter = this;
            isSelected = true;
            Debug.Log($"[CharacterSelection] Selected character: {gameObject.name}");

            // ✅ Switch Hand and AP when selecting a new character
            HandManager.Instance?.SwitchActiveClass(classIndex);
            APManager.Instance?.SwitchActiveClass(classIndex);
        }
    }

    public void Deselect()
    {
        if (IsSelected)
        {
            currentlySelectedCharacter = null;
            isSelected = false;
            Debug.Log($"[CharacterSelection] Deselected character.");
        }
    }

    public static BaseCharacter GetSelectedCharacter()
    {
        return currentlySelectedCharacter != null ? currentlySelectedCharacter.GetComponent<BaseCharacter>() : null;
    }
}
