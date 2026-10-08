using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject image;
    [SerializeField] private GameObject texbox;
    [SerializeField] private TutorialSoundSound TutorialSoundSound;
    [SerializeField] private GameObject[] HPUI;
    public bool IsTutorialFinished { get; private set; }

    void Start()
    {
        IsTutorialFinished = false;
        Blocker.SetActive(false);
        backButton.SetActive(false);
        nextButton.SetActive(false);
        readyButton.SetActive(false);
        image.SetActive(false);
        foreach (GameObject obj in HPUI)
        {
            obj.SetActive(false);
        }
    }

    private int currentIndex = 0;

    public void StartTutorial()
    {
        Blocker.SetActive(true);
        currentIndex = 0;
        ShowTutorial(0);
        nextButton.SetActive(true);
        image.SetActive(true);
        texbox.SetActive(false);
    }

    private void ShowTutorial(int index)
    {
        tutorialImage.sprite = tutorials[index].image;
        //uiText.DrawText(tutorials[index].text);
    }


    public void NextTutorial()
    {

        currentIndex++;
        ShowTutorial(currentIndex);
        backButton.SetActive(true);
        TutorialSoundSound.TTR();

        if (currentIndex == tutorials.Length-1)
        {
            nextButton.SetActive(false);
            readyButton.SetActive(true);
        }
    }

    public void BackTutorial()
    {

        if (currentIndex <= 0)
            return;
        currentIndex--;
        ShowTutorial(currentIndex);
        nextButton.SetActive(true);
        TutorialSoundSound.TTR();
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
        
        IsTutorialFinished = true;
        image.SetActive(false);
        texbox.SetActive(true);
        foreach (GameObject obj in HPUI)
        {
            obj.SetActive(true);
        }
        TutorialSoundSound.OnClick();
    }

}