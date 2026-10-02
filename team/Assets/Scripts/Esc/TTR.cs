using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EscTTR
{
    public Sprite image;
}

public class TTR : MonoBehaviour
{
    [SerializeField] private EscTTR[] tutorials;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject Blocker;
    [SerializeField] private GameObject image;

    public bool IsTutorialFinished { get; private set; }

    //void Start()
        private void Awake()
    {
        IsTutorialFinished = false;
        Blocker.SetActive(false);
        backButton.SetActive(false);
        nextButton.SetActive(false);
        image.SetActive(false);
    }

    private int currentIndex = 0;

    public void StartTutorial()
    {
        Blocker.SetActive(true);
        currentIndex = 0;
        ShowTutorial(0);
        nextButton.SetActive(true);
        image.SetActive(true);
    }

    private void ShowTutorial(int index)
    {
        tutorialImage.sprite = tutorials[index].image;
    }


    public void NextTutorial()
    {
        currentIndex++;
        ShowTutorial(currentIndex);
        backButton.SetActive(true);

        if (currentIndex == tutorials.Length - 1)
        {
            nextButton.SetActive(false);
        }
    }

    public void BackTutorial()
    {
        if (currentIndex <= 0)
            return;
        currentIndex--;
        ShowTutorial(currentIndex);
        nextButton.SetActive(true);
        if (currentIndex == 0)
        {
            backButton.SetActive(false);
        }
    }
    public void EndTutorial()
    {
        gameObject.SetActive(false);
        Blocker.SetActive(false);
        backButton.SetActive(false);
        nextButton.SetActive(false);

        IsTutorialFinished = true;
        image.SetActive(false);
    }
}
