using System.Collections.Generic;
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
            Debug.Log("No have lines");
            //Do fail swap animation
        } else {
            Debug.Log("Have lines");
            DestroyMatchedTiles(tiles);
            //Do swap tile animation
        }
    }

    private void CheckAutoMatchingByRefilling() {
        List<Tile> tiles = tileGrid.SearchMatchedTiles();
        if (tiles.Count >= 3) {
            DestroyMatchedTiles(tiles);
        }
    }

    private void DestroyMatchedTiles(List<Tile> tiles) {
        //Increment score
        //Do tile destroy animation
        DestroyTiles(tiles);
    }

    private void DestroyTiles(List<Tile> tiles) {
        tileGrid.DestroyTiles(tiles);
        tileGrid.RefillGrid();
    }
}