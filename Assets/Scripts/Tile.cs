using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    public int ColorType { get; private set; }
    public int x;
    public int y;
    public Action<Tile> TileSwipe;
    private bool isPressed = false;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(int colorType) {
        ColorType = colorType;
        spriteRenderer.color = ColorTable.Instance.Colors[colorType];
    }

    private void OnMouseDown() => isPressed = true;

    private void OnMouseUp() => isPressed = false;

    private void OnMouseExit() {
        if (isPressed)
            TileSwipe?.Invoke(this);
        isPressed = false;
    }

    private void OnDestroy() {
        TileSwipe = null;
    }
}