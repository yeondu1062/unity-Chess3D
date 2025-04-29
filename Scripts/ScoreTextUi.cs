using UnityEngine;
using TMPro;

public class ScoreTextUi : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public void TextUpdate()
    {
        scoreText.text = $"{ChessManager.instance.aliveWhite}/{ChessManager.instance.aliveBlack}";
    }
}
