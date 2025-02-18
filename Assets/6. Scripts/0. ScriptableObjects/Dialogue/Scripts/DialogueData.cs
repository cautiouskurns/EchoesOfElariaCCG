using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLoreDialogue", menuName = "Dialogue/LoreDialogue")]
public class DialogueData : ScriptableObject
{
    [TextArea(3, 5)]
    public string dialogueText; // The main dialogue text

    public List<DialogueChoice> choices; // List of choices and outcomes
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText; // The text of the choice
    public DialogueOutcome outcome; // The effect when choosing this option
}

[System.Serializable]
public class DialogueOutcome
{
    [TextArea(2, 3)]
    public string outcomeText; // The text displayed after selecting this choice

    public int energyChange;
    public int luckChange;
    public int strengthChange;
    public bool giveItem;
}

