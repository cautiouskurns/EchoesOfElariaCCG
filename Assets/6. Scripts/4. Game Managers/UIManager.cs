using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowEndBattleScreen(bool isWin)
    {
        if (isWin)
        {
            winScreen.SetActive(true);
        }
        else
        {
            loseScreen.SetActive(true);
            Invoke(nameof(ReturnToMap), 3f);  // Add delay for lose screen
        }
    }

    public void ReturnToMap()
    {
        Debug.Log("[UIManager] Attempting to return to map...");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToOverworld();
        }
        else
        {
            Debug.LogWarning("[UIManager] GameManager not found, loading Overworld scene directly");
            SceneManager.LoadScene("OverworldMap");
        }
    }
}
