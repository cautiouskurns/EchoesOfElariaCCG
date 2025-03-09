using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

// This class helps create a fallback dialogue for error handling
public class CreateFallbackDialogue : MonoBehaviour
{
    [MenuItem("Tools/Create Fallback Dialogue")]
    public static void CreateDialogue()
    {
        // Create a new dialogue data asset
        DialogueData fallbackDialogue = ScriptableObject.CreateInstance<DialogueData>();
        fallbackDialogue.name = "FallbackDialogue";
        
        // Set required properties based on your DialogueData structure
        // Example: fallbackDialogue.dialogueLines = new string[] { "This is fallback text." };
        
        // Save the asset
        string path = "Assets/Resources/FallbackDialogue.asset";
        AssetDatabase.CreateAsset(fallbackDialogue, path);
        AssetDatabase.SaveAssets();
        
        // Show in project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = fallbackDialogue;
        
        Debug.Log($"Created fallback dialogue asset at {path}");
    }
}
#endif
