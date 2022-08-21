using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField] private TileGrid tileGrid;
    private void Start() {
        tileGrid.CreateAndFillNewGrid();
        tileGrid.TileSwipe += TrySwapGems;
        tileGrid.GridRefilled += CheckAutoMatchingByRefilling;
    }

    private void TrySwapGems(Tile tile1, Tile tile2) {
        tileGrid.Swap(tile1, tile2);
        List<Tile> tiles = tileGrid.SearchMatchedTiles();
        if (tiles.Count < 3) {
            tileGrid.Swap(tile1, tile2);
            Match3Animation.FailSwapping(tile1.transform, tile2.transform);
        } else {
            Match3Animation.SwapTiles(tile1.transform, tile2.transform, () => DestroyMatchedTiles(tiles));
        }
    }

    private void CheckAutoMatchingByRefilling() {
        List<Tile> tiles = tileGrid.SearchMatchedTiles();
        if (tiles.Count >= 3) {
            DestroyMatchedTiles(tiles);
        }
    }

    private void DestroyMatchedTiles(List<Tile> tiles) {
        if (Score.Instance) {
            Score.Instance.AddScore(tiles.Count);
        }
        Match3Animation.DestroyTiles(tiles.Select(t => t.transform).ToArray(), () => DestroyTiles(tiles));
    }

    private void DestroyTiles(List<Tile> tiles) {
        tileGrid.DestroyTiles(tiles);
        tileGrid.RefillGrid();
    }
}