using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(CharacterCombat))]
public class PlayerUnit : BaseCharacter
{
    [SerializeField] private GameObject selectionSprite;  // Assign in inspector

    private CharacterStats stats;
    private CharacterCombat combat;
    
    protected override void Awake()
    {
        base.Awake();
        Name = "Player";

        stats = GetComponent<CharacterStats>();  
        combat = GetComponent<CharacterCombat>();

        if (selectionSprite != null)
        {
            selectionSprite.SetActive(false);  // Start deselected
        }
    }

    public override void Select()
    {
        base.Select();
        if (selectionSprite != null)
        {
            selectionSprite.SetActive(true);
        }
    }

    public override void Deselect()
    {
        base.Deselect();
        if (selectionSprite != null)
        {
            selectionSprite.SetActive(false);
        }
    }
}
