using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(CharacterCombat))]
public class PlayerUnit : BaseCharacter
{
    [SerializeField] private Material outlineMaterial;
    private Material defaultMaterial;
    private Renderer characterRenderer; // Changed from SpriteRenderer to Renderer to work with both 2D and 3D

    private CharacterStats stats;
    private CharacterCombat combat;

    protected override void Awake()
    {
        base.Awake();
        Name = "Player";

        // Get required components
        stats = GetComponent<CharacterStats>();
        combat = GetComponent<CharacterCombat>();
        
        // Look for renderer in children
        characterRenderer = GetComponentInChildren<Renderer>();

        if (characterRenderer != null)
        {
            defaultMaterial = characterRenderer.sharedMaterial;
            Debug.Log($"[PlayerUnit] Found {characterRenderer.GetType().Name} in {characterRenderer.gameObject.name} and stored default material: {defaultMaterial?.name ?? "null"}");
        }
        else
        {
            Debug.LogError("[PlayerUnit] ❌ No renderer component found in children! Check hierarchy for MeshRenderer or SpriteRenderer.");
        }

        if (outlineMaterial == null)
        {
            Debug.LogError("[PlayerUnit] ❌ Outline material not assigned in inspector!");
        }
    }
    
    public override void InitializeFromClass(CharacterClass characterClass)
    {
        base.InitializeFromClass(characterClass);  // Ensure BaseCharacter handles stat initialization

        Debug.Log($"[PlayerUnit] ✅ {characterClass.className} initialized.");
    }

    public override void Select()
    {
        // Validate components before selection
        if (characterRenderer == null)
        {
            Debug.LogError("[PlayerUnit] ❌ Cannot select: Missing renderer component!");
            return;
        }

        if (outlineMaterial == null)
        {
            Debug.LogError("[PlayerUnit] ❌ Cannot select: Missing outline material!");
            return;
        }

        if (!IsSelected)
        {
            characterRenderer.material = outlineMaterial;
            base.Select();
            Debug.Log($"[PlayerUnit] Applied outline material to {Name}");
        }
    }

    public override void Deselect()
    {
        if (IsSelected)  // Only proceed if currently selected
        {
            if (characterRenderer != null && defaultMaterial != null)
            {
                characterRenderer.material = defaultMaterial;
                base.Deselect();  // Call base.Deselect() after restoring material
                Debug.Log($"[PlayerUnit] Restored default material to {Name}");
            }
        }
    }

    // Add validation in inspector
    private void OnValidate()
    {
        if (outlineMaterial == null)
        {
            Debug.LogWarning("[PlayerUnit] ⚠️ Outline material needs to be assigned!");
        }
    }
}

