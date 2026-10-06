using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [Header("ボタン位置")]
    [SerializeField] private int currentPanelIndex = 0;
    [SerializeField] private int BeforeButtonPosition = -200;
    [SerializeField] private int TouchButtonPosition = -100;
    [SerializeField] private int AftterButtonPosition = 0;

    [Header("チュートリアル")]
    [SerializeField] private GameObject[] TTR;

    [Header("サウンド設定")]
    [SerializeField] private GameObject[] Parameters;

    [Header("音量Slider")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    [Header("終了確認")]
    [SerializeField] private GameObject[] ExitObj;

    [Header("その他")]
    [SerializeField] private GameObject blocker;

    [SerializeField] private bool isEscPanelOpen = false;

    [Header("TTR管理")]
    [SerializeField] private TTR ttr;


    private void Awake()
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
            if (parameter != null)
            {
                parameter.SetActive(false);
            }
        }

        foreach (GameObject exitobj in ExitObj)
        {
            if (exitobj != null)
            {
                exitobj.SetActive(false);
            }
        }

        foreach (GameObject obj in TTR)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    private void Start()
    {
        // AudioSetに保存されている値をSliderに反映
        masterSlider.SetValueWithoutNotify(AudioSet.Instance.MasterVolume);
        bgmSlider.SetValueWithoutNotify(AudioSet.Instance.BGMVolume);
        seSlider.SetValueWithoutNotify(AudioSet.Instance.SEVolume);
    }
    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isEscPanelOpen)
            {
                OpenEscPanel();
            }
            else
            {
                CloseEscPanel();
            }
        }
    }
    public void SetMaster(float value)
    {
        AudioSet.Instance.SetMaster(value);
    }

    public void SetBGM(float value)
    {
        AudioSet.Instance.SetBGM(value);
    }

    public void SetSE(float value)
    {
        AudioSet.Instance.SetSE(value);
    }
    private void OpenEscPanel()
    {
        blocker.SetActive(true);
        escPanel.SetActive(true);

        isEscPanelOpen = true;

        // チュートリアルを表示
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
            if (parameter != null)
            {
                parameter.SetActive(false);
            }
        }

        foreach (GameObject exitobj in ExitObj)
        {
            if (exitobj != null)
            {
                exitobj.SetActive(false);
            }
        }

        foreach (GameObject obj in TTR)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
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
        {
            SetButtonPosition(tutorialButton, TouchButtonPosition);
        }

        if (index == 1)
        {
            SetButtonPosition(soundButton, TouchButtonPosition);
        }

        if (index == 2)
        {
            SetButtonPosition(ExitButton, TouchButtonPosition);
        }
    }


    public void OnMouseExitButton(int index)
    {
        if (currentPanelIndex == index)
            return;

        if (index == 0)
        {
            SetButtonPosition(tutorialButton, BeforeButtonPosition);
        }

        if (index == 1)
        {
            SetButtonPosition(soundButton, BeforeButtonPosition);
        }

        if (index == 2)
        {
            SetButtonPosition(ExitButton, BeforeButtonPosition);
        }
    }


    private void ShowPanel(EscPanelIndex index)
    {
        tutorialButton.SetActive(true);
        soundButton.SetActive(true);
        ExitButton.SetActive(true);

        currentPanelIndex = (int)index;

        // いったん全部非表示
        tutorialPanel.SetActive(false);
        soundPanel.SetActive(false);
        exitPanel.SetActive(false);

        foreach (GameObject parameter in Parameters)
        {
            if (parameter != null)
            {
                parameter.SetActive(false);
            }
        }

        foreach (GameObject exitobj in ExitObj)
        {
            if (exitobj != null)
            {
                exitobj.SetActive(false);
            }
        }

        foreach (GameObject obj in TTR)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // ボタンを初期位置に戻す
        SetButtonPosition(tutorialButton, BeforeButtonPosition);
        SetButtonPosition(soundButton, BeforeButtonPosition);
        SetButtonPosition(ExitButton, BeforeButtonPosition);


        switch (index)
        {
            case EscPanelIndex.Tutorial:

                tutorialPanel.SetActive(true);

                SetButtonPosition(
                    tutorialButton,
                    AftterButtonPosition
                );

                // TTRを表示
                foreach (GameObject obj in TTR)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }

                // TTRのStartTutorialを実行
                if (ttr != null)
                {
                    ttr.StartTutorial();
                }

                break;


            case EscPanelIndex.Sound:

                soundPanel.SetActive(true);

                SetButtonPosition(
                    soundButton,
                    AftterButtonPosition
                );

                foreach (GameObject parameter in Parameters)
                {
                    if (parameter != null)
                    {
                        parameter.SetActive(true);
                    }
                }

                break;


            case EscPanelIndex.Exit:

                exitPanel.SetActive(true);

                SetButtonPosition(
                    ExitButton,
                    AftterButtonPosition
                );

                foreach (GameObject exitobj in ExitObj)
                {
                    if (exitobj != null)
                    {
                        exitobj.SetActive(true);
                    }
                }

                break;
        }
    }


    private void SetButtonPosition(GameObject button, int xPosition)
    {
        if (button == null)
            return;

        RectTransform rect = button.GetComponent<RectTransform>();

        if (rect == null)
            return;

        Vector2 position = rect.anchoredPosition;

        position.x = xPosition;

        rect.anchoredPosition = position;
    }
}
