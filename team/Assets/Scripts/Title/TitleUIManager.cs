using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class TitleUIManager : MonoBehaviour
{
    [Header("タイトル画面")]
    [SerializeField] private GameObject mainMenu;

    [Header("オプション画面")]
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private TitleVolumeBoard volumeBoard;

    [Header("終了確認画面")]
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private EXITWindow exitwindow;


    [Header("サウンド")]
    [SerializeField] private TitleSound TitleSound;
    public bool isOptionVisible = false;

    private void Start()
    {
        ShowMainMenu();
    }

    // メインメニューを表示
    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        optionPanel.SetActive(false);
        exitPanel.SetActive(false);
        isOptionVisible = false;
    }

    // オプションを表示
    public void ShowOption()
    {
        if (isOptionVisible)
            return;
        TitleSound.OnClick();
        mainMenu.SetActive(false);
        optionPanel.SetActive(true);
        exitPanel.SetActive(false);

        volumeBoard.Show();
        isOptionVisible = true;
    }

    public void HideOption()
    {
        TitleSound.OnClick();
        volumeBoard.Hide();

    }

    // 終了確認を表示
    public void ShowExit()
    {
        if (isOptionVisible)
            return;
        TitleSound.OnClick();
        mainMenu.SetActive(false);
        optionPanel.SetActive(false);
        exitPanel.SetActive(true);

        exitwindow.Show();
        isOptionVisible = true;

    }

    public void HideExit()
    {
        TitleSound.OnClick();
        exitwindow.Hide();
    }

    public void BackHome()
    {

        mainMenu.SetActive(false);
        optionPanel.SetActive(false);
        exitPanel.SetActive(false);
        isOptionVisible = false;
    }
    //一旦これ
}