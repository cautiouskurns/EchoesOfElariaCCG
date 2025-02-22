using UnityEngine;


[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(CharacterCombat))]
[RequireComponent(typeof(CharacterSelection))]
public class PlayerUnit : BaseCharacter
{
    [SerializeField] private Material outlineMaterial;
    private Material defaultMaterial;
    private Renderer characterRenderer;
    private CharacterSelection selection;

    [SerializeField] private int classIndex; // ✅ Assign in Inspector for each player unit

    protected override void Awake()
    {
        base.Awake();
        selection = GetComponent<CharacterSelection>();

        characterRenderer = GetComponentInChildren<Renderer>();

        if (characterRenderer != null)
        {
            defaultMaterial = characterRenderer.sharedMaterial;
        }
        else
        {
            Debug.LogError("[PlayerUnit] ❌ No renderer found!");
        }

        // ✅ Assign `classIndex` from PlayerUnit to CharacterSelection
        if (selection != null)
        {
            selection.GetType().GetField("classIndex").SetValue(selection, classIndex);
        }
    }

    public override void Select()  // ✅ Now correctly overriding BaseCharacter.Select()
    {
        if (Selection == null)
        {
            Debug.LogError("[PlayerUnit] ❌ Selection component is NULL!");
            return;
        }

        Debug.Log($"[PlayerUnit] Attempting to select {Name}...");

        if (!Selection.IsSelected)
        {
            Debug.Log($"[PlayerUnit] ✅ Selecting {Name}");

            if (characterRenderer != null && outlineMaterial != null)
            {
                characterRenderer.material = new Material(outlineMaterial); // ✅ Ensures a new material instance
            }
            else
            {
                Debug.LogWarning("[PlayerUnit] ⚠️ Outline material is missing!");
            }

            base.Select();  // ✅ Calls the base class `Select()` which delegates to `CharacterSelection`
        }
    }

    public override void Deselect()  // ✅ Now correctly overriding BaseCharacter.Deselect()
    {
        if (Selection == null)
        {
            Debug.LogError("[PlayerUnit] ❌ Selection component is NULL!");
            return;
        }

        Debug.Log($"[PlayerUnit] Attempting to deselect {Name}...");

        if (Selection.IsSelected)
        {
            Debug.Log($"[PlayerUnit] ❌ Deselecting {Name}");

            if (characterRenderer != null && defaultMaterial != null)
            {
                characterRenderer.material = defaultMaterial;
            }

            base.Deselect();  // ✅ Calls the base class `Deselect()` which delegates to `CharacterSelection`
        }
    }

}

