using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField] private TileGrid tileGrid;
    private void Start() {
        tileGrid.CreateAndFillNewGrid();
    }
}