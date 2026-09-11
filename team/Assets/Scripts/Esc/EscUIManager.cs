using UnityEngine;
using UnityEngine.InputSystem;

public enum EscPanelIndex
{
    Tutorial = 0,
    Sound = 1,
    Exit = 2
}

public class EscUIManager : MonoBehaviour
{
    [Header("ESC全体")]
    [SerializeField] private GameObject escPanel;

    [Header("ボタン")]
    [SerializeField] private GameObject tutorialButton;
    [SerializeField] private GameObject soundButton;
    [SerializeField] private GameObject ExitButton;

    [Header("表示するパネル")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private GameObject exitPanel;

    [SerializeField] private int currentPanelIndex = 0;
    [SerializeField] private int BeforeButtonPosition = -200;
    [SerializeField] private int TouchButtonPosition = -100;
    [SerializeField] private int AftterButtonPosition = 0;

    [SerializeField] private GameObject[] Parameters;

    [SerializeField] private GameObject[] ExitObj;

    [SerializeField] private GameObject blocker;

    [SerializeField] private bool isEscPanelOpen = false;
    private void Start()
    {
        blocker.SetActive(false);
        tutorialButton.SetActive(false);
        soundButton.SetActive(false);
        ExitButton.SetActive(false);
        escPanel.SetActive(false);

        tutorialPanel.SetActive(false);
        soundPanel.SetActive(false);
        exitPanel.SetActive(false);
        foreach (GameObject parameter in Parameters)
        {
            parameter.SetActive(false);
        }
        foreach (GameObject exitobj in ExitObj)
        {
            exitobj.SetActive(false);
        }

    }


    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && !isEscPanelOpen)
        {
            OpenEscPanel();
        }
        else if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && isEscPanelOpen)
        {
            CloseEscPanel();
        }
    }

    private void OpenEscPanel()
    {
        blocker.SetActive(true);
        escPanel.SetActive(true);
        isEscPanelOpen = true;
        ShowPanel(EscPanelIndex.Tutorial);
    }

    public void CloseEscPanel()
    {
        isEscPanelOpen = false;
        blocker.SetActive(false);
        escPanel.SetActive(false);
        tutorialButton.SetActive(false);
        soundButton.SetActive(false);
        ExitButton.SetActive(false);
        tutorialPanel.SetActive(false);
        soundPanel.SetActive(false);
        exitPanel.SetActive(false);
        foreach (GameObject parameter in Parameters)
        {
            parameter.SetActive(false);
        }
        foreach (GameObject exitobj in ExitObj)
        {
            exitobj.SetActive(false);
        }
    }

    public void OnTutorialButton()
    {
        ShowPanel(EscPanelIndex.Tutorial);
    }

    public void OnSoundButton()
    {
        ShowPanel(EscPanelIndex.Sound);
    }

    public void OnExitButton()
    {
        ShowPanel(EscPanelIndex.Exit);
    }

    public void OnMouseEnterButton(int index)
    {
        if (currentPanelIndex == index)
            return;

        if (index == 0)
            SetButtonPosition(tutorialButton, TouchButtonPosition);

        if (index == 1)
            SetButtonPosition(soundButton, TouchButtonPosition);

        if (index == 2)
            SetButtonPosition(ExitButton, TouchButtonPosition);
    }

    public void OnMouseExitButton(int index)
    {
        if (currentPanelIndex == index)
            return;

        if (index == 0)
            SetButtonPosition(tutorialButton, BeforeButtonPosition);

        if (index == 1)
            SetButtonPosition(soundButton, BeforeButtonPosition);

        if (index == 2)
            SetButtonPosition(ExitButton, BeforeButtonPosition);
    }

    private void ShowPanel(EscPanelIndex index)
    {
        tutorialButton.SetActive(true);
        soundButton.SetActive(true);
        ExitButton.SetActive(true);

        currentPanelIndex = (int)index;

        tutorialPanel.SetActive(false);
        soundPanel.SetActive(false);
        exitPanel.SetActive(false);

        SetButtonPosition(tutorialButton, BeforeButtonPosition);
        SetButtonPosition(soundButton, BeforeButtonPosition);
        SetButtonPosition(ExitButton, BeforeButtonPosition);

        foreach (GameObject parameter in Parameters)
        {
            parameter.SetActive(false);
        }

        foreach (GameObject exitobj in ExitObj)
        {
            exitobj.SetActive(false);
        }

        switch (index)
        {
            case EscPanelIndex.Tutorial:
                tutorialPanel.SetActive(true);
                SetButtonPosition(tutorialButton, AftterButtonPosition);
                break;

            case EscPanelIndex.Sound:
                soundPanel.SetActive(true);
                SetButtonPosition(soundButton, AftterButtonPosition);

                foreach (GameObject parameter in Parameters)
                {
                    parameter.SetActive(true);
                }
                break;

            case EscPanelIndex.Exit:
                exitPanel.SetActive(true);
                SetButtonPosition(ExitButton, AftterButtonPosition);
                foreach (GameObject exitobj in ExitObj)
                {
                    exitobj.SetActive(true);
                }
                break;
        }
    }

    private void SetButtonPosition(GameObject button, int xPosition)
    {
        RectTransform rect = button.GetComponent<RectTransform>();

        Vector2 position = rect.anchoredPosition;
        position.x = xPosition;

        rect.anchoredPosition = position;
    }
}