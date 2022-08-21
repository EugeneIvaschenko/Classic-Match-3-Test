using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileGrid : MonoBehaviour {
    [SerializeField, Min(3)] private int width;
    [SerializeField, Min(3)] private int height;
    [SerializeField] private Tile tilePrefab;

    private Tile[,] grid;

    public Action<Tile, Tile> TileSwipe;
    public Action GridRefilled;

    public void Swap(Tile tile1, Tile tile2) {
        Vector2Int tempPos = new(tile1.x, tile1.y);
        SetTileTo(tile1, tile2.x, tile2.y);
        SetTileTo(tile2, tempPos.x, tempPos.y);
    }

    private void SetTileTo(Tile tile, int x, int y) {
        grid[x, y] = tile;
        tile.x = x;
        tile.y = y;
    }

    public void CreateAndFillNewGrid() {
        grid = new Tile[width, height];

        for (int x = 0; x < width; x += Vector2Int.right.x) {
            for (int y = 0; y < height; y += Vector2Int.up.y) {
                List<int> colors = ColorTable.Instance.GetColorKeys();
                RemoveDublicateColorFromColorList(x, y, Vector2Int.left, colors);
                RemoveDublicateColorFromColorList(x, y, Vector2Int.down, colors);
                CreateNewTile(x, y, colors);
            }
        }

        UpdateRect();
    }

    private void UpdateRect() {
        Rect gridRect = new() { width = width, height = height };
        float gridX = (Camera.main.rect.width - gridRect.width) / 2;
        float gridY = (Camera.main.rect.height - gridRect.height) / 2;
        transform.position = new(gridX, gridY);
    }

    public void RefillGrid() {
        List<Transform[]> colsOfNew = new();
        List<Tile> oldTiles = new();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if(grid[x, y])
                    oldTiles.Add(grid[x, y]);
            }

            colsOfNew.Add(RefillColumnAndGetNewTiles(x));
        }
        Match3Animation.FallTiles(oldTiles.ToArray(), colsOfNew.ToArray(), () => GridRefilled?.Invoke());
    }

    private Transform[] RefillColumnAndGetNewTiles(int x) {
        List<Transform> newGems = new();
        for (int y = 0; y < height; y++) {
            if (grid[x, y])
                continue;
            if(y == height - 1) {
                CreateNewTile(x, y, ColorTable.Instance.GetColorKeys());
                newGems.Add(grid[x, y].transform);
                continue;
            }
            Tile tile = TakeHigherTile(x, y + 1);
            if (tile) {
                SetTileTo(tile, x, y);
            } else {
                CreateNewTile(x, y, ColorTable.Instance.GetColorKeys());
                newGems.Add(grid[x, y].transform);
            }
        }
        return newGems.ToArray();
        ;
    }

    private Tile TakeHigherTile(int x, int y) {
        if (grid[x, y]) {
            Tile tile = grid[x, y];
            grid[x, y] = null;
            return tile;
        }
        if (y < height - 1)
            return TakeHigherTile(x, y + 1);
        return null;
    }

    public void DestroyTiles(List<Tile> tiles) {
        foreach (var tile in tiles) {
            if (tile == null || tile.gameObject == null)
                continue;
            tile.gameObject.SetActive(false);
            Destroy(tile.gameObject);
            grid[tile.x, tile.y] = null;
        }
    }

    public List<Tile> SearchMatchedTiles() {
        List<Tile> tiles = new();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Tile tile = grid[x, y];
                List<Tile> line = new() { tile };
                line.AddRange(GetMatchedLineFrom(tile, Vector2Int.left));
                line.AddRange(GetMatchedLineFrom(tile, Vector2Int.right));
                if (line.Count >= 3) {
                    if (!tiles.Contains(tile))
                        tiles.Add(tile);
                    continue;
                }

                line = new() { tile };
                line.AddRange(GetMatchedLineFrom(tile, Vector2Int.up));
                line.AddRange(GetMatchedLineFrom(tile, Vector2Int.down));
                if (line.Count >= 3) {
                    if (!tiles.Contains(tile))
                        tiles.Add(tile);
                    continue;
                }
            }
        }
        return tiles;
    }

    private List<Tile> GetMatchedLineFrom(Tile origTile, Vector2Int dir) {
        List<Tile> line = new();
        Tile lastTile = origTile;
        while (TryGetNeighbor(lastTile, dir, out Tile neighborTile) && lastTile.ColorType == neighborTile.ColorType) {
            line.Add(neighborTile);
            lastTile = neighborTile;
        }
        return line;
    }

    private void RemoveDublicateColorFromColorList(int x, int y, Vector2Int dir, List<int> colors) {
        if (!TryGetNeighbor(x, y, dir, out Tile tile1))
            return;
        if (!TryGetNeighbor(tile1.x, tile1.y, dir, out Tile tile2))
            return;
        if (tile1.ColorType == tile2.ColorType) {
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

    private bool TryGetNeighbor(Tile origTile, Vector2Int dir, out Tile tile) => TryGetNeighbor(origTile.x, origTile.y, dir, out tile);

    private void CreateNewTile(int x, int y, List<int> colorKeys) {
        Tile newTile = Instantiate(tilePrefab, transform);
        newTile.transform.localPosition = new(x, y);
        newTile.SetColor(colorKeys[Random.Range(0, colorKeys.Count)]);
        newTile.x = x;
        newTile.y = y;
        newTile.TileSwipe += OnTileSwipe;
        grid[x, y] = newTile;
    }

    private void OnTileSwipe(Tile tile) {
        Vector2 swipeVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - tile.transform.position;
        Vector2Int swipeVectorInt = GetSwipeDirection(swipeVector);
        if (TryGetNeighbor(tile, swipeVectorInt, out Tile neighborTile)) {
            TileSwipe?.Invoke(tile, neighborTile);
        }
    }

    private Vector2Int GetSwipeDirection(Vector2 swipeVector) {
        Vector2Int newVector = new();
        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y)) {
            newVector.x = (int)(1 * Mathf.Sign(swipeVector.x));
            newVector.y = 0;
        } else {
            newVector.y = (int)(1 * Mathf.Sign(swipeVector.y));
            newVector.x = 0;
        }
        return newVector;
    }
}