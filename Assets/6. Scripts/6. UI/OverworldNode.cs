using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverworldNode : MonoBehaviour
{
    public string battleSceneName; // Assign in Inspector

    private Button nodeButton;

    private void Start()
    {
        nodeButton = GetComponent<Button>();
        if (nodeButton != null)
            nodeButton.onClick.AddListener(StartBattle);
    }

    private void StartBattle()
    {
        SceneManager.LoadScene(battleSceneName);
    }
}



