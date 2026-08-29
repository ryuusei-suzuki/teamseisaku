using UnityEngine;

[System.Serializable]
public class GameOverData
{
    [TextArea(2, 5)]
    public string text;
}

public class GameOverTextWriter : MonoBehaviour
{
    [SerializeField] private GameOverData[] gameOverData;
    public GameOverText GameOverText;
    public GameObject text;

    private int currentIndex = 0;

    private void Awake()
    {
        text.SetActive(false);
    }

    private void ShowGameOverText(int index)
    {
        if (index < 0 || index >= gameOverData.Length)
        {
            Debug.LogError(
                $"GameOverDataÇÃîÕàÕäOÇ≈Ç∑ÅBindex={index}, Size={gameOverData.Length}"
            );
            return;
        }

        GameOverText.DrawText(gameOverData[index].text);
    }

    public void GotoTitle()
    {
        text.SetActive(true);

        currentIndex = 0;
        ShowGameOverText(currentIndex);
    }

    public void GotoBattle()
    {
        text.SetActive(true);

        currentIndex = 1;
        ShowGameOverText(currentIndex);
    }
}