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
    public bool isDisplayed = true;

    private void Awake()
    {
        text.SetActive(false);
        isDisplayed = true;
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
        isDisplayed = false;
        text.SetActive(true);

        currentIndex = 0;
        ShowGameOverText(currentIndex);
    }

    public void GotoBattle()
    {
        isDisplayed = false;
        text.SetActive(true);

        currentIndex = 1;
        ShowGameOverText(currentIndex);
    }

    public void HideText()
    {
        isDisplayed = true;
        text.SetActive(false);
    }
}