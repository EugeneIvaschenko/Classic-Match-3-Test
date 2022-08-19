using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour {
    [SerializeField, Min(3)] private int width;
    [SerializeField, Min(3)] private int height;
    [SerializeField] private Tile tilePrefab;

    private Tile[,] grid;

    public readonly static Vector2Int Left = new(-1, 0);
    public readonly static Vector2Int Right = new(1, 0);
    public readonly static Vector2Int Up = new(0, -1);
    public readonly static Vector2Int Down = new(0, 1);

    public void CreateAndFillNewGrid() {
        grid = new Tile[width, height];

        for (int x = 0; x < width; x += Right.x) {
            for (int y = 0; y < height; y += Down.y) {
                List<int> colors = ColorTable.Instance.GetColorKeys();
                RemoveDublicateColorFromColorList(x, y, Left, colors);
                RemoveDublicateColorFromColorList(x, y, Up, colors);
                CreateNewTile(x, y, colors);
            }
        }
    }

    private void RemoveDublicateColorFromColorList(int x, int y, Vector2Int dir, List<int> colors) {
        if (!TryGetNeighbor(x, y, dir, out Tile tile1))
            return;
        if (!TryGetNeighbor(tile1.x, tile1.y, dir, out Tile tile2))
            return;
        if(tile1.ColorType == tile2.ColorType) {
            if (colors.Contains(tile1.ColorType))
                colors.Remove(tile1.ColorType);
        }
    }

    private bool TryGetNeighbor(int x, int y, Vector2Int dir, out Tile tile) {
        int newX = x + dir.x;
        int newY = y + dir.y;
        if (newX >= width || newX < 0 || newY >= height || newY < 0) {
            tile = null;
            return false;
        } else {
            tile = grid[newX, newY];
            return true;
        }
    }

    private void CreateNewTile(int x, int y, List<int> colorKeys) {
        Tile newTile = Instantiate(tilePrefab, new Vector3(x , y ), Quaternion.identity);
        newTile.SetColor(colorKeys[Random.Range(0, colorKeys.Count)]);
        newTile.x = x;
        newTile.y = y;
        grid[x, y] = newTile;
    }

}