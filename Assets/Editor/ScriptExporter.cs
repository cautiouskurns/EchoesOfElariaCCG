using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class ScriptExporter : EditorWindow
{
    [MenuItem("Tools/Export Scripts to Text File")]
    public static void ExportScripts()
    {
        string projectPath = Application.dataPath;
        string outputPath = Path.Combine(Application.dataPath, "../ScriptExport.txt");
        StringBuilder content = new StringBuilder();

        // Find all .cs files in the Assets folder and its subfolders
        string[] scriptFiles = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);

        foreach (string filePath in scriptFiles)
        {
            content.AppendLine($"// File: {Path.GetFileName(filePath)}");
            content.AppendLine("//===========================================");
            content.AppendLine(File.ReadAllText(filePath));
            content.AppendLine("\n\n");
        }

        // Write to file
        File.WriteAllText(outputPath, content.ToString());
        Debug.Log($"Scripts exported to: {outputPath}");
        
        // Open the folder containing the exported file
        EditorUtility.RevealInFinder(outputPath);
    }
}
