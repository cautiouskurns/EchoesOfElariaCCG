using UnityEngine;

public class CardExecution : MonoBehaviour
{
    private CardBehavior cardBehavior;

    private void Awake()
    {
        cardBehavior = GetComponent<CardBehavior>();
        if (cardBehavior == null)
        {
            Debug.LogError("[CardExecution] ❌ CardBehavior component is missing!");
        }
    }

    public void PlayCard(IEffectTarget target)
    {
        if (TurnManager.Instance.CurrentTurn != TurnManager.TurnState.PlayerTurn)
        {
            Debug.LogWarning("[CardExecution] ❌ Cannot play cards during enemy turn!");
            return;
        }

        if (cardBehavior == null || cardBehavior.CardData == null)
        {
            Debug.LogError("[CardExecution] ❌ CardBehavior or CardData is missing.");
            return;
        }

        // Get card cost
        int cost = cardBehavior.CardData.Cost;
        
        // Check if enough AP is available
        if (!APManager.Instance.SpendAP(cost))
        {
            Debug.LogWarning($"[CardExecution] ❌ Not enough AP to play {cardBehavior.CardData.CardName}");
            return;
        }

        // Get selected character
        BaseCharacter sourceCharacter = BaseCharacter.GetSelectedCharacter();
        if (sourceCharacter == null)
        {
            Debug.LogWarning("[CardExecution] ❌ No character selected to play card!");
            return;
        }

        // Calculate effect value with class bonus
        int baseValue = cardBehavior.CardData.EffectValue;
        float multiplier = 1.0f;
        
        Debug.Log($"[CardExecution] Character Class: {sourceCharacter.Stats.CharacterClass}, Card Preferred Class: {cardBehavior.CardData.PreferredClass}");
        
        if (sourceCharacter.Stats.CharacterClass == cardBehavior.CardData.PreferredClass)
        {
            multiplier = cardBehavior.CardData.ClassBonus;
            Debug.Log($"[CardExecution] ⚔️ Class bonus of {multiplier}x applied!");
        }

        int finalValue = Mathf.RoundToInt(baseValue * multiplier);
        Debug.Log($"[CardExecution] Damage calculation: {baseValue} × {multiplier} = {finalValue}");

        // Apply the card effect
        CardEffect effect = cardBehavior.CardData.CardEffect;
        if (effect == null)
        {
            Debug.LogError($"[CardExecution] ❌ No effect found for card {cardBehavior.CardData.CardName}.");
            return;
        }

        Debug.Log($"[CardExecution] 🎯 {sourceCharacter.Name} ({sourceCharacter.Stats.CharacterClass}) played {cardBehavior.CardData.CardName} for {finalValue} damage");
        effect.ApplyEffect(target, finalValue);

        // Remove the card from hand
        Destroy(gameObject);
    }
}

