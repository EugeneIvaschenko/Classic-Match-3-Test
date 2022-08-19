using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    public int ColorType { get; private set; }
    public int x;
    public int y;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(int colorType) {
        ColorType = colorType;
        spriteRenderer.color = ColorTable.Instance.Colors[colorType];
    }
}