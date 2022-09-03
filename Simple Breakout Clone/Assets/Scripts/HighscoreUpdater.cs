using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class HighscoreUpdater : MonoBehaviour
{
    private static string _highScoreString = "HIGHSCORE: {0}";
    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        GameManager.Instance.HighscoreUpdateEvent += UpdateScore;
        UpdateScore(GameManager.Instance.Highscore);
    }

    private void UpdateScore(int score)
    {
        _text.SetText(_highScoreString, score);
    }

    private void OnDestroy()
    {
        GameManager.Instance.HighscoreUpdateEvent -= UpdateScore;
    }
}
