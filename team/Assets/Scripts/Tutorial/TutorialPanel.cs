using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static Unity.Collections.AllocatorManager;

[System.Serializable]
public class TutorialData
{
    public Sprite image;

    [TextArea(2, 5)]
    public string text;
}

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private TutorialData[] tutorials;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private UIText uiText;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject Blocker;

    void Start()
    {
        Blocker.SetActive(false);
        backButton.SetActive(false);
        nextButton.SetActive(false);
        readyButton.SetActive(false);
    }

    private int currentIndex = 0;

    public void StartTutorial()
    {
        Blocker.SetActive(true);
        currentIndex = 0;
        ShowTutorial(0);
        nextButton.SetActive(true);
    }

    private void ShowTutorial(int index)
    {
        tutorialImage.sprite = tutorials[index].image;
        uiText.DrawText(tutorials[index].text);
    }


    public void NextTutorial()
    {
        if (uiText.playing)
            return;
        currentIndex++;
        ShowTutorial(currentIndex);
        backButton.SetActive(true);

        if (currentIndex == tutorials.Length-1)
        {
            nextButton.SetActive(false);
            readyButton.SetActive(true);
        }
    }

    public void BackTutorial()
    {
        if (uiText.playing)
            return;
        if (currentIndex <= 0)
            return;
        currentIndex--;
        ShowTutorial(currentIndex);
        nextButton.SetActive(true);
        if (currentIndex == 0)
        {
            backButton.SetActive(false);
        }

        if (currentIndex == tutorials.Length - 2)
        {
            readyButton.SetActive(false);
        }
    }
    public void EndTutorial()
    {
        gameObject.SetActive(false);
        Blocker.SetActive(false);
        backButton.SetActive(false);
        nextButton.SetActive(false);
        readyButton.SetActive(false);
    }

}