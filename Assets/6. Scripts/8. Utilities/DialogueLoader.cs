using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class DialogueNode
{
    public string dialogueId;
    public string text;
    public List<Choice> choices;
    public Outcome outcome;
}

[System.Serializable]
public class Choice
{
    public string choiceText;
    public string nextId;
}

[System.Serializable]
public class Outcome
{
    public string outcomeText; // ✅ This was missing!
    public int energyChange;
    public int strengthChange;
    public bool triggerBattle;
}

[System.Serializable]
public class DialogueTree
{
    public List<DialogueNode> nodes;
}

public class DialogueLoader : MonoBehaviour
{
    private Dictionary<string, DialogueNode> dialogueNodes;

    // Load from Resources folder
    public void LoadDialogueFromResources(string resourcePath)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);
        if (jsonFile == null)
        {
            Debug.LogError($"[DialogueLoader] Failed to load dialogue from Resources: {resourcePath}");
            return;
        }

        LoadDialogueFromJson(jsonFile.text);
    }

    // Load from direct file path
    public void LoadDialogueFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"[DialogueLoader] File not found: {filePath}");
            return;
        }

        string jsonContent = File.ReadAllText(filePath);
        LoadDialogueFromJson(jsonContent);
    }

    // Private method to handle the actual JSON parsing
    private void LoadDialogueFromJson(string jsonContent)
    {
        try
        {
            DialogueTree tree = JsonUtility.FromJson<DialogueTree>(jsonContent);
            dialogueNodes = new Dictionary<string, DialogueNode>();

            foreach (var node in tree.nodes)
            {
                dialogueNodes[node.dialogueId] = node;
            }

            Debug.Log($"[DialogueLoader] Successfully loaded {dialogueNodes.Count} dialogue nodes");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DialogueLoader] Error parsing dialogue JSON: {e.Message}");
        }
    }

    public DialogueNode GetNode(string dialogueId)
    {
        return dialogueNodes.ContainsKey(dialogueId) ? dialogueNodes[dialogueId] : null;
    }
}
