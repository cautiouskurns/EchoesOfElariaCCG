using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class EndTurnButton : MonoBehaviour
{
    private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnEndTurnClicked);
    }

    private void Start()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged += UpdateButtonState;
            UpdateButtonState(TurnManager.Instance.CurrentTurn);
        }
        else
        {
            Debug.LogError("[EndTurnButton] ❌ No TurnManager instance found!");
            button.interactable = false;
        }
    }

    private void OnEnable()
    {
        // Moved to Start
    }

    private void OnDisable()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnTurnChanged -= UpdateButtonState;
    }

    private void OnEndTurnClicked()
    {
        TurnManager.Instance.EndPlayerTurn();
    }

    private void UpdateButtonState(TurnManager.TurnState newTurn)
    {
        button.interactable = (newTurn == TurnManager.TurnState.PlayerTurn);
        buttonText.text = newTurn == TurnManager.TurnState.PlayerTurn ? "End Turn" : "Enemy Turn";
    }
}
