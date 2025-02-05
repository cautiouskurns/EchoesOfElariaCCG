using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteGenerator : MonoBehaviour
{
    [Header("Sprite Settings")]
    [SerializeField] private int width = 256;      // Width of the sprite
    [SerializeField] private int height = 256;     // Height of the sprite
    [SerializeField] private Color fillColor = Color.green;  // Color to fill the sprite

    [Header("Shape Settings")]
    public ShapeType shape = ShapeType.Square;    // Shape type: Square or Circle

    private SpriteRenderer spriteRenderer;

    public enum ShapeType
    {
        Square,
        Circle
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GenerateSprite();
    }

    [ContextMenu("Generate Sprite")]
    public void GenerateSprite()
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;  // Makes edges sharp for pixel art
        texture.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = x + y * width;

                if (shape == ShapeType.Circle)
                {
                    float centerX = width / 2f;
                    float centerY = height / 2f;
                    float radius = Mathf.Min(width, height) / 2f;

                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                    pixels[index] = distance <= radius ? fillColor : Color.clear;
                }
                else // Square
                {
                    pixels[index] = fillColor;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        // Create the sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
        spriteRenderer.sprite = sprite;

        Debug.Log($"[SpriteGenerator] ✅ Sprite generated successfully ({shape}) with color {fillColor}.");
    }
}
