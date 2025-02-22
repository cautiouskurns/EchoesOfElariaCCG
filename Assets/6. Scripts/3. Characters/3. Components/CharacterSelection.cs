using UnityEngine;
using System.Collections.Generic;

public class CharacterSelection : MonoBehaviour
{
    private bool isSelected = false;
    public bool IsSelected => isSelected;

    [SerializeField] private int classIndex;

    public void Select()
    {
        isSelected = true;
        Debug.Log($"[CharacterSelection] Selected character: {gameObject.name}");

        HandManager.Instance?.SwitchActiveClass(classIndex);
        APManager.Instance?.SwitchActiveClass(classIndex);
    }

    public void Deselect()
    {
        isSelected = false;
        Debug.Log($"[CharacterSelection] Deselected character.");
    }
}

