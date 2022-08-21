using TMPro;
using UnityEngine;

public class Score : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;

    private int scoreByTile = 100;
    private int score = 0;

    public static Score Instance;

    private void Awake() {
        if (!Instance) {
            Instance = this;
            scoreText.text = "0";
        }
    }

    public void AddScore(int count) {
        score += count * scoreByTile;
        scoreText.text = score.ToString();
    }
}